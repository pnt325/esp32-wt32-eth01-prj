/*
 * app.c
 *
 *  Created on: Apr 09, 2022
 *      Author: Phat.N
 */

#include <string.h>
#include "freertos/FreeRTOS.h"
#include "freertos/task.h"

#include "app.h"
#include "dio.h"
#include "ucfg.h"
#include "connect.h"
#include "ble.h"
#include "esp_log.h"
#include "mqtt.h"
#include "ntc.h"

#define APP_TAG  "APP"

#define NUMBER_OF_CHANNEL           3
#define LED_BLINK_PERIOD_ON_CONFIG  100         //! ms
#define LED_BLINK_PERIOD_ON_APP     500         //! ms
#define TEMPERATURE_SCAN_PERIOD     10000       //! ms, 10 sec
#define TEMPERATURE_DELAY_ALERT     (5*60*1000) //! 5 min
#define WORK_HOUR_STORE_PERIOD      15          //! sec
#define WORK_HOUR_UPDATE_PERIOD     1000        //! ms, 1 sec
#define WORK_HOUR_SYNC_PERIOD       3600        //! sec, 1h

enum 
{   
    LED_OFF,
    LED_ON,
    LED_BLINK
};

static void main_handle(void* param);           //! App handle
static void led_handle(void);                   //! Control LED
static void btn_handle(void);                   //! Button long press handle.   
static void temp_handle(void);                  //! Temperature alarm handle

static void network_connected(void);            //! network conected callback
static void network_disconnected(void);         //! network disconnected callback
static void enter_config(void);                 //! initialize to enter configuration mode
static void work_hour_commit(void);             //! Save work-hour to flash
static void mqtt_temp_alert(float* temp);       //! Publish temperature alert to MQTT broker
static void mqtt_work_hours(uint32_t* hours);   //! Update the work-hour as period to MQTT broker

static bool     on_config        = false;                       //! ON configuration mode run with bluetooth only
static uint32_t led_blink_period = LED_BLINK_PERIOD_ON_CONFIG;  //! LED blink period
static uint8_t  led_ctrl         = LED_BLINK;                   //! LED control state

static uint32_t work_hours[NUMBER_OF_CHANNEL];      //! Current work-hour count, sec
static uint32_t work_hour_olds[NUMBER_OF_CHANNEL];  //! Last work-hour count, sec
static float    temp_offset;                        //! Temperature offset value  
static uint8_t  temp_limit;                         //! Temperaure limit to take alarm value.

static char mqtt_temp_alert_topic[32];  //! MQTT alert topic
static char mqtt_work_hour_topic[32];   //! MQTT work-hour topic
char        device_token[11];           //! Device ID, use as token on publish topic to MQTT broker

void APP_init(void)
{
    DIO_init();     //! Initialze GPIO use on APP
    NTC_init();     //! NTC sensor initialize
    UCFG_init();    //! User configure initialize

    //! Get MAC address as device token
    uint8_t mac_address[6] = {0};
    ESP_ERROR_CHECK(esp_read_mac(mac_address, ESP_MAC_WIFI_STA));
    memset(mqtt_temp_alert_topic, 0x00, sizeof(mqtt_temp_alert_topic));
    memset(mqtt_work_hour_topic, 0x00, sizeof(mqtt_work_hour_topic));
    memset(device_token, 0x00, sizeof(device_token));

    snprintf(device_token, sizeof(device_token), "%02X%02X%2X%02X%02X",
             mac_address[1],
             mac_address[2],
             mac_address[3],
             mac_address[4],
             mac_address[5]);

    snprintf(mqtt_temp_alert_topic, sizeof(mqtt_temp_alert_topic), "alert/discharge_temp/%s", device_token);
    snprintf(mqtt_work_hour_topic , sizeof(mqtt_work_hour_topic) , "update/hours/%s", device_token);

    ESP_LOGI(APP_TAG, "topic: %s", mqtt_temp_alert_topic);
    ESP_LOGI(APP_TAG, "topic: %s", mqtt_work_hour_topic);

    //! First boot application if button pressed enter configuration mode.
    if(DIO_button_state() == BUTTON_PRESSED)
    {
        ESP_LOGI(APP_TAG, "Button Enter configure");
        enter_config();
    }
    else
    {
        uint8_t connection = CONNECTION_NONE;
        if (UCFG_read_connection(&connection) == false)
        {
            connection = CONNECTION_NONE;
        }

        if (connection != CONNECTION_WIFI && connection != CONNECTION_ETH)
        {
            ESP_LOGE(APP_TAG, "Invalid connection Enter configure");
            enter_config();
        }
        else
        {
            if (CONNECT_init(connection))
            {
                led_blink_period = LED_BLINK_PERIOD_ON_APP;
                CONNECT_sub_connected_event(network_connected);
                CONNECT_sub_disconnected_event(network_disconnected);
            }
            else
            {
                enter_config();
                ESP_LOGI(APP_TAG, "Enter configure, cause connection failure");
            }
        }
    }

    //! Create task handle without
    if(on_config == false)
    {
        xTaskCreatePinnedToCore(main_handle, "main_app", 4096, NULL, 25, NULL, APP_CPU_NUM);
    }

    APP_run();
}

void APP_run(void)
{
    if(UCFG_read_temp_offset(&temp_offset) == false)
    {
        ESP_ERROR_CHECK(ESP_FAIL);
    }

    ESP_LOGI(APP_TAG, "Temp offset: %f", temp_offset);

    if(UCFG_read_temp_limit(&temp_limit) == false)
    {
        ESP_ERROR_CHECK(ESP_FAIL);
    }
    ESP_LOGI(APP_TAG, "Temp limit: %d", temp_limit);

    uint32_t temp_period = esp_log_timestamp();

    while (true)
    {
        uint32_t time = (uint32_t)(esp_log_timestamp() - temp_period);

        if(on_config)
        {
            if (time >= 1000 && BLE_is_notify())
            {
                float temp[3] = {0};
                temp[0] = NTC_read(0);
                temp[1] = NTC_read(1);
                temp[2] = NTC_read(2);

                BLE_send_data(BLE_CMD_PROBE_TEMP, (uint8_t *)temp, sizeof(float) * 3);

                uint8_t di[3] = {0};
                di[0] = DIO_status(0);
                di[1] = DIO_status(1);
                di[2] = DIO_status(2);
                BLE_send_data(BLE_CMD_PROBE_DI, di, 3);

                temp_period = esp_log_timestamp();
            }
        }
        else 
        {
            if (time >= TEMPERATURE_SCAN_PERIOD)
            {
                temp_handle();
                temp_period = esp_log_timestamp();
            }
        }
        

        led_handle();
        btn_handle();

        vTaskDelay(pdMS_TO_TICKS(50));
    }
}

static void main_handle(void* param)
{
    (void)param;

    //! Update last work_hour
    if(UCFG_read_work_hour(work_hours) == false)
    {
        ESP_ERROR_CHECK(ESP_FAIL); 
    }

    ESP_LOGI(APP_TAG, "Last work-hour: %d, %d, %d", work_hours[0], work_hours[1], work_hours[2]);
    for(uint8_t i = 0; i < NUMBER_OF_CHANNEL; i++)
    {
        work_hour_olds[i] = work_hours[i];
    }

    uint32_t one_sec_hold = esp_log_timestamp();
    uint32_t sec_count = 0;
    while(true)
    {
        //! count work hour
        uint32_t time = (uint32_t)(esp_log_timestamp() - one_sec_hold);
        if(time >= WORK_HOUR_UPDATE_PERIOD)
        {
            uint8_t publish_flag = 0;
            for(uint8_t i = 0; i < 3; i++)
            {
                if(DIO_status(i) == STATUS_ACTIVE)
                {
                    work_hours[i]++;

                    ESP_LOGI(APP_TAG, "DI Channel %i active", i);

                    // FIXME just for test
                    if((work_hours[i] % 5) == 0)
                    // if((work_hours[i] % WORK_HOUR_SYNC_PERIOD) == 0)
                    {
                        publish_flag |= 1;
                    }
                }
            }       

            if(publish_flag)
            {
                mqtt_work_hours(work_hours);
            }

            sec_count++;
            if((sec_count  % WORK_HOUR_STORE_PERIOD) == 0)
            {
                ESP_LOGI(APP_TAG, "[%d]Save work-hour", sec_count);
                work_hour_commit();
            }

            one_sec_hold = esp_log_timestamp();
        }

        vTaskDelay(pdMS_TO_TICKS(50));
    }
}

static void btn_handle(void)
{
    static uint8_t btn_state = 1;
    static uint8_t btn_state_old = 1;
    static uint8_t hold_handle = 0;
    static uint32_t btn_count;
    static uint8_t rise_event = 0;

    if(on_config)
    {
        return;
    }

    btn_state = DIO_button_state();
    if(btn_state != btn_state_old)
    {
        btn_state_old = btn_state;

        if(btn_state == 0)
        {
            
            btn_count = esp_log_timestamp();
            hold_handle = 1;
            rise_event = 0;
            ESP_LOGI(APP_TAG, "Btn pressed");
        }
        else
        {
            hold_handle = 0;
            ESP_LOGI(APP_TAG, "Btn release");
        }
    }   

    if(hold_handle && rise_event == 0)
    {
        uint32_t count_ms = esp_log_timestamp() - btn_count;
        if(count_ms >= 5000)
        {
            ESP_LOGI(APP_TAG, "Hold 5sec");
            rise_event = 1;
            esp_restart();
        }
    }
}

static void led_handle(void)
{
    if(led_ctrl == LED_OFF)
    {
        DIO_led_off();
    }
    else if(led_ctrl == LED_ON)
    {
        DIO_led_on();
    }
    else if(led_ctrl == LED_BLINK)
    {
        static uint32_t period_count = 0;
        uint32_t p = esp_log_timestamp() - period_count;
        if(p >= led_blink_period)
        {
            period_count = esp_log_timestamp();
            DIO_led_toggle();   
        }
    }
}

static void mqtt_evt(uint8_t connect)
{
    if(connect)
    {
        ESP_LOGI(APP_TAG, "MQTT connected");
        led_ctrl =  LED_ON;
    }
    else 
    {
        ESP_LOGI(APP_TAG, "MQTT disconnected");
        led_ctrl = LED_BLINK;
        led_blink_period = LED_BLINK_PERIOD_ON_APP;
    }
}

static void network_connected(void)
{
    static bool first_run = false;
    if(first_run == false)
    {
        first_run = true;
        MQTT_init(mqtt_evt);
    }
}

static void network_disconnected(void)
{
    led_ctrl = LED_BLINK;
    led_blink_period = LED_BLINK_PERIOD_ON_APP;

    MQTT_clear_connnect();
}

static void enter_config(void)
{
    on_config = true;
    led_blink_period = LED_BLINK_PERIOD_ON_CONFIG;
    BLE_start();
}

static void work_hour_commit(void)
{
    uint8_t i;

    //! Check the work hour is update
    for(i = 0; i < NUMBER_OF_CHANNEL; i++)
    {
        if(work_hour_olds[i] != work_hours[i])
        {
            break;
        }
    }

    if(i == NUMBER_OF_CHANNEL)
    {
        //! Nothing update
        return;
    }

    // FIXME Just for test.
    ESP_LOGI(APP_TAG, "Update work-hours");
    return;

    //! Update old work-hour
    for(i = 0; i < NUMBER_OF_CHANNEL; i++)
    {
        work_hour_olds[i] = work_hours[i];
    }

    //! storage
    if(UCFG_write_work_hour(work_hours) == false)
    {
        ESP_ERROR_CHECK(ESP_FAIL);   
    }
}

static void temp_handle(void)
{   
    static uint8_t alarms[NUMBER_OF_CHANNEL]        = {0};
    static uint8_t alarm_olds[NUMBER_OF_CHANNEL]    = {0};
    static uint32_t temp_period[NUMBER_OF_CHANNEL]  = {0};
    static uint8_t is_publish[NUMBER_OF_CHANNEL]    = {0};

    float temps[NUMBER_OF_CHANNEL] = {25.0f};
    uint8_t publish = 0;
    
    for(uint8_t i = 0; i < NUMBER_OF_CHANNEL; i++)
    {
        temps[i] = NTC_read(i) + temp_offset;

        if(temps[i] >= temp_limit)
        {
            alarms[i] = 1;
        }
        else
        {
            alarms[i]     = 0;
            is_publish[i] = 0;
        }

        if((alarms[i] != alarm_olds[i]) && alarms[i])
        {
            alarm_olds[i] = alarms[i];
            temp_period[i] = esp_log_timestamp();
        }

        if(alarms[i])
        {
            uint32_t time = (uint32_t)(esp_log_timestamp() - temp_period[i]);

            //! 5 min

            // FIXME Just for test
            // if(time >= TEMPERATURE_DELAY_ALERT)
            if(time >= 5000)    // Test for 5 sec
            {
                if(is_publish[i] == 0)
                {
                    is_publish[i] = 1;
                    publish |= 1;
                }
            }
        }
    }

    ESP_LOGI(APP_TAG, "Temperature update: %f, %f, %f", temps[0], temps[1], temps[2]);

    if(publish && on_config == 0)
    {
        mqtt_temp_alert(temps);
    }
}

static void mqtt_temp_alert(float *temp)
{
    char buff[64] = {0};
    //{"alert": [1, 0, 0],"value": [123, 123, 123]}
    uint8_t di[3] = {0};
    for(uint8_t i = 0; i < 3; i++)
    {
        di[i] = (temp[i] >= temp_limit) ? 1 : 0;
    }

    snprintf(buff, sizeof(buff), "{\"alert\": [%d, %d, %d],\"value\": [%0.2f, %0.2f, %0.2f]}", di[0], di[1], di[2], temp[0], temp[1], temp[2]);
    if(MQTT_publish(mqtt_temp_alert_topic, buff, strlen(buff)) == false)
    {
        ESP_LOGE(APP_TAG, "Send temp alert failure");
    }
    else 
    {
        ESP_LOGI(APP_TAG, "Send temp alert success");
    }
}

static void mqtt_work_hours(uint32_t *hours)
{
    //{"value": [123, 123, 123]}

    if(hours == NULL)
    {
        ESP_ERROR_CHECK(ESP_FAIL);
    }

    char buff[64] = {0};
    snprintf(buff, sizeof(buff), "{\"value\": [%u, %u, %u]}", hours[0], hours[1], hours[2]);
    if (MQTT_publish(mqtt_work_hour_topic, buff, strlen(buff)) == false)
    {
        ESP_LOGE(APP_TAG, "Send temp alert failure");
    }
    else
    {
        ESP_LOGI(APP_TAG, "Send temp alert success");
    }
}

