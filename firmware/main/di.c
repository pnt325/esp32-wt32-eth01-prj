/*
 * di.h
 *
 *  Created on: Apr 03, 2022
 *      Author: Phat.N
 */


#include "di.h"
#include "driver/gpio.h"

#define GPIO_INPUT_IO_0     15
#define GPIO_INPUT_IO_1     14
#define GPIO_INPUT_IO_2     12

#define GPIO_INPUT_PIN_SEL  ((1ULL<<GPIO_INPUT_IO_0) | (1ULL<<GPIO_INPUT_IO_1) || (1ULL<<GPIO_INPUT_IO_2))

gpio_num_t gpio_channels[] = {GPIO_INPUT_IO_0,
                              GPIO_INPUT_IO_1,
                              GPIO_INPUT_IO_2};

void DI_init(void)
{
    gpio_config_t io_conf = {};
    io_conf.intr_type = GPIO_INTR_DISABLE;
    io_conf.pin_bit_mask = GPIO_INPUT_PIN_SEL;
    io_conf.mode = GPIO_MODE_INPUT;
    io_conf.pull_up_en = 1;
    gpio_config(&io_conf);
}

uint8_t DI_status(uint8_t channel)
{
    ESP_ERROR_CHECK(channel >= 3 ? ESP_FAIL : ESP_OK);
    return gpio_get_level(gpio_channels[channel]);
}