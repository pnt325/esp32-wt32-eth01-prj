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

enum 
{   
    LED_OFF,
    LED_ON,
    LED_BLINK
};

static void main_handle(void* param);
static void led_handle(void);
static void btn_handle(void);
static void temp_handle(void);

static void network_connected(void);
static void network_disconnected(void);
static void enter_config(void);
static void work_hour_commit(void);
static void mqtt_temp_alert(float* temp);
static void mqtt_work_hours(uint32_t* hours);

static bool on_config = false;
static uint32_t led_blink_period = 500;
static uint8_t led_ctrl = LED_BLINK;

static uint32_t work_hours[3];
static uint32_t work_hour_olds[3];
static float temp_offset;
static uint8_t temp_limit = 95;   // default value

static char mqtt_temp_alert_topic[32];
static char mqtt_work_hour_topic[32];
static char device_token[11];

void APP_init(void)
{
    DIO_init();
    NTC_init();
    UCFG_init();

    //! Token is device ID
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

    // Create task main app.
    xTaskCreatePinnedToCore(main_handle, "main_app", 4096, NULL, 25, NULL, APP_CPU_NUM);

    //! Enter config or run application
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

        // connection = CONNECTION_ETH;
        if (connection != CONNECTION_WIFI && connection != CONNECTION_ETH)
        {
            ESP_LOGE(APP_TAG, "Invalid connection Enter configure");
            enter_config();
        }
        else
        {
            if (CONNECT_init(connection))
            {
                led_blink_period = 500;
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

        // increate period in case of device in configure mode.
        if(on_config)
        {
            if(time >= 1000 && BLE_is_notify())
            {
                float temp[3] = {0};
                temp[0] = NTC_read(0);
                temp[1] = NTC_read(1);
                temp[2] = NTC_read(2);

                BLE_send_data(BLE_CMD_PROBE_TEMP, (uint8_t*)temp, sizeof(float)*3);

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
            if (time >= 10000)
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
    for(uint8_t i = 0; i < 3; i++)
    {
        work_hour_olds[i] = work_hours[i];
    }

    uint32_t one_sec_hold = esp_log_timestamp();
    uint32_t sec_count = 0;
    while(true)
    {
        //! count work hour
        uint32_t time = (uint32_t)(esp_log_timestamp() - one_sec_hold);
        if(time >= 1000)
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
                    // if((work_hours[i] % 3600) == 0)
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
            if((sec_count  % 15) == 0)
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
        led_blink_period = 500;
    }
}

static void network_connected(void)
{
    static uint8_t first_run = 0;
    if(first_run == 0)
    {
        first_run = 1;
        MQTT_init(mqtt_evt);
    }
}

static void network_disconnected(void)
{
    led_ctrl = LED_BLINK;
    led_blink_period = 500;
}

static void enter_config(void)
{
        on_config = true;
        led_blink_period = 200;
        BLE_start();
}

static void work_hour_commit(void)
{
    uint8_t i;

    //! Check the work hour is update
    for(i = 0; i < sizeof(work_hours); i++)
    {
        if(work_hour_olds[i] != work_hours[i])
        {
            break;
        }
    }

    if(i == sizeof(work_hours))
    {
        return;
    }

    // FIXME Just for test.
    ESP_LOGI(APP_TAG, "Update work-hours");
    return;

    //! Update old work-hour
    for(i = 0; i < sizeof(work_hours); i++)
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
    static uint8_t alarms[3] = {0};
    static uint8_t alarm_olds[3] = {0};
    static uint32_t temp_period[3];
    static uint8_t is_publish[3] = {0};

    float temps[3] = {25.0f};
    uint8_t publish = 0;

    ESP_LOGI(APP_TAG, "Function update temperature");

    for(uint8_t i = 0; i < 3; i++)
    {
        temps[i] = NTC_read(i) + temp_offset;

        // FIXME Just for test
        temp_limit = 20;

        if(temps[i] >= temp_limit)
        {
            alarms[i] = 1;
        }
        else
        {
            alarms[i] = 0;
            is_publish[i] = 0;
        }

        if(alarms[i] != alarm_olds[i] && alarms[i])
        {
            alarm_olds[i] = alarms[i];
            temp_period[i] = esp_log_timestamp();
        }

        if(alarms[i])
        {
            uint32_t time = (uint32_t)(esp_log_timestamp() - temp_period[i]);

            //! 5 min

            // FIXME Just for test
            // if(time >= (5*60*1000))
            if(time >= 5000)
            {
                if(is_publish[i] == 0)
                {
                    is_publish[i] = 1;
                    publish |= 1;
                }
            }
        }
    }

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

