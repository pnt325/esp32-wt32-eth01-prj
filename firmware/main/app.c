/*
 * app.c
 *
 *  Created on: Apr 09, 2022
 *      Author: Phat.N
 */

#include "freertos/FreeRTOS.h"
#include "freertos/task.h"

#include "app.h"
#include "dio.h"
#include "ucfg.h"
#include "connect.h"
#include "ble.h"
#include "esp_log.h"
#include "mqtt.h"

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

static void network_connected(void);
static void network_disconnected(void);

static bool on_config = false;
static uint32_t led_blink_period = 500;
static uint8_t led_ctrl = LED_BLINK;

void APP_init(void)
{
    DIO_init();
    UCFG_init();
    // xTaskCreate(main_handle, "btn_led", 1024, NULL, 25, NULL);
    xTaskCreatePinnedToCore(main_handle, "btn_led", 1024, NULL, 25, NULL, APP_CPU_NUM);

    if(DIO_button_state() == BUTTON_PRESSED)
    {
        on_config = true;
        ESP_LOGI(APP_TAG, "Button Enter configure");
        BLE_start();
    }
    else
    {
        uint8_t connection = CONNECTION_NONE;
        if (UCFG_read_connection(&connection) == false)
        {
            connection = CONNECTION_NONE;
        }

        // FIXME Just for test, enter ethernet connection
        connection = CONNECTION_ETH;
        if (connection != CONNECTION_WIFI && connection != CONNECTION_ETH)
        {
            on_config = true;
            ESP_LOGI(APP_TAG, "Invalid connection Enter configure");
            BLE_start();
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
                on_config = true;
                ESP_LOGI(APP_TAG, "Enter configure");
                BLE_start();
                
            }
        }
    }

    APP_run();
}

void APP_run(void)
{

        while (true)
    {
        led_handle();
        btn_handle();

        // DIO_test();

        vTaskDelay(pdMS_TO_TICKS(50));
    }
}

static void main_handle(void* param)
{
    // ESP_LOGI(APP_TAG, "Task main app run on core: %d", xPortGetCoreID());
    while(true)
    {
        // ESP_LOGI(APP_TAG, "Task main");
        // DIO_led_off();
        // vTaskDelay(pdMS_TO_TICKS(500));
        // DIO_led_on();
        vTaskDelay(pdMS_TO_TICKS(500));
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
        uint32_t tick = xTaskGetTickCount() - period_count;
        if(pdTICKS_TO_MS(tick) >= led_blink_period)
        {
            period_count = xTaskGetTickCount();
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