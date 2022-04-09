/*
 * dio.h
 *
 *  Created on: Apr 03, 2022
 *      Author: Phat.N
 */


#include "dio.h"
#include "esp_log.h"
#include "driver/gpio.h"

#include "freertos/FreeRTOS.h"
#include "freertos/task.h"

#define GPIO_INPUT_IO_0     15
#define GPIO_INPUT_IO_1     14
#define GPIO_INPUT_IO_2     5

#define GPIO_BUTTON_1       17
#define GPIO_LED            4

#define GPIO_INPUT_PIN_SEL  ((1ULL<<GPIO_INPUT_IO_0) | (1ULL<<GPIO_INPUT_IO_1) || (1ULL<<GPIO_INPUT_IO_2) || (1ULL<<GPIO_BUTTON_1))

gpio_num_t gpio_channels[] = {GPIO_INPUT_IO_0,
                              GPIO_INPUT_IO_1,
                              GPIO_INPUT_IO_2};
static uint8_t led_state = 0;

void DIO_init(void)
{
    gpio_config_t io_conf = {};
    io_conf.intr_type = GPIO_INTR_DISABLE;
    io_conf.pin_bit_mask = GPIO_INPUT_PIN_SEL;
    io_conf.mode = GPIO_MODE_INPUT;
    io_conf.pull_up_en = 1;
    gpio_config(&io_conf);

    // gpio_config_t io_conf = {};
    io_conf.intr_type    = GPIO_INTR_DISABLE;
    io_conf.mode         = GPIO_MODE_OUTPUT;
    io_conf.pin_bit_mask = (1ULL << GPIO_LED);
    io_conf.pull_down_en = 0;
    io_conf.pull_up_en = 0;
    gpio_config(&io_conf);

    gpio_set_level(GPIO_LED, 1);
}

uint8_t DIO_status(uint8_t channel)
{
    ESP_ERROR_CHECK(channel >= 3 ? ESP_FAIL : ESP_OK);
    return gpio_get_level(gpio_channels[channel]);
}

uint8_t DIO_button_state(void)
{
    return gpio_get_level(GPIO_BUTTON_1);
}

void DIO_led_on(void)
{
    gpio_set_level(GPIO_LED, 0);
    led_state = 1;
}

void DIO_led_off(void)
{
    gpio_set_level(GPIO_LED, 1);
    led_state = 0;
}

void DIO_led_toggle(void)
{
    if(led_state)
    {
        DIO_led_off();
    }
    else
    {
        DIO_led_on();
    }
}

void DIO_test(void)
{
    static uint8_t btn_state[4] = {0};
    static uint8_t btn_state_old[4] = {0};

    // btn_state[0] = DIO_status(0);
    // btn_state[1] = DIO_status(1);
    // btn_state[2] = DIO_status(2);
    // btn_state[3] = DIO_button_state();

    // // Copy last status
    // for(uint8_t i = 0; i < sizeof(btn_state); i++)
    // {
    //     btn_state_old[i] = btn_state[i];
    // }

    // ESP_LOGI("DI_TEST", "status channel: %d, %d, %d, Button: %s", btn_state[0], btn_state[1], btn_state[2], btn_state[3] ? "Release" : "Pressed");

    // while(true)
    // {
        btn_state[0] = DIO_status(0);
        btn_state[1] = DIO_status(1);
        btn_state[2] = DIO_status(2);
        btn_state[3] = DIO_button_state();

        uint8_t i;
        for(i = 0; i < sizeof(btn_state); i++)
        {
            if(btn_state[i] != btn_state_old[i])
            {
                break;
            }
        }

        if(i != 4)
        {
            ESP_LOGI("DI_TEST", "status channel: %d, %d, %d, Button: %s", btn_state[0], btn_state[1], btn_state[2], btn_state[3] ? "Release" : "Pressed");
            for (uint8_t i = 0; i < sizeof(btn_state); i++)
            {
                btn_state_old[i] = btn_state[i];
            }

            if(btn_state[3])
            {
                    gpio_set_level(GPIO_LED, 1);
            }   
            else{
                
                gpio_set_level(GPIO_LED, 0);
            }
        }

        // vTaskDelay(pdMS_TO_TICKS(100));
    // }
}
