/*
 * ntc.c
 *
 *  Created on: Apr 03, 2022
 *      Author: Phat.N
 */

#include "ntc.h"

#include <stdio.h>
#include <stdlib.h>
#include "freertos/FreeRTOS.h"
#include "freertos/task.h"
#include "driver/gpio.h"
#include "driver/adc.h"
#include "soc/adc_channel.h"
#include "esp_adc_cal.h"
#include "esp_log.h"
#include "ntc_table.h"


/*           
 *       3.3V
 *        |    
 *       +-+
 *       | | R 4k7 Ohm
 *       +-+
 *        |______Vout, R = ((3.3 - Vout)/Vout)*4700
 *        |
 *       +-+
 *       | | R_NTC
 *       +-+ 
 *        |
 *      Ground
 * Vout        -> IO39
 * ADC         -> ADC1
 * ADC_CHANNEL -> ADC_CHANNEL_3
 */

#define TAG "NTC"

#define DEFAULT_VREF        3300.0f     //! 3.3V
#define DEFUALT_PULLUP_R    4.7f        //! kOhm

static esp_adc_cal_characteristics_t *adc_chars;
static const adc_channel_t channel  = ADC1_GPIO39_CHANNEL;
static const adc_bits_width_t width = ADC_WIDTH_BIT_12;
static const adc_atten_t atten      = ADC_ATTEN_DB_11;
static const adc_unit_t unit        = ADC_UNIT_1;


static void check_efuse(void)
{
    //Check if TP is burned into eFuse
    if (esp_adc_cal_check_efuse(ESP_ADC_CAL_VAL_EFUSE_TP) == ESP_OK) {
        ESP_LOGI(TAG, "eFuse Two Point: Supported");
    } else {
        ESP_LOGE(TAG, "eFuse Two Point: NOT supported");
    }

    //Check Vref is burned into eFuse
    if (esp_adc_cal_check_efuse(ESP_ADC_CAL_VAL_EFUSE_VREF) == ESP_OK) {
        ESP_LOGI(TAG, "eFuse Vref: Supported");
    } else {
        ESP_LOGE(TAG, "eFuse Vref: NOT supported");
    }
}


static void print_char_val_type(esp_adc_cal_value_t val_type)
{
    if (val_type == ESP_ADC_CAL_VAL_EFUSE_TP) {
        ESP_LOGI(TAG, "Characterized using Two Point Value");
    } else if (val_type == ESP_ADC_CAL_VAL_EFUSE_VREF) {
        ESP_LOGI(TAG, "Characterized using eFuse Vref");
    } else {
        ESP_LOGI(TAG, "Characterized using Default Vref");
    }
}

static int16_t ntc_lookup(float r)
{
    int i;
    if (r >= NTC_TABLE_res[0])
    {
        return NTC_TABLE_temp[0];
    }

    if (r < NTC_TABLE_res[sizeof(NTC_TABLE_res) - 1])
    {
        return NTC_TABLE_temp[sizeof(NTC_TABLE_res) - 1];
    }

    for(i = 0; i < sizeof(NTC_TABLE_res) - 1; i++)
    {
        if(r >= NTC_TABLE_res[i + 1] && r < NTC_TABLE_res[i])
        {
            break;
        }
    }

    float per = (NTC_TABLE_res[i] - r)/(NTC_TABLE_res[i] - NTC_TABLE_res[i + 1]);
    float temp = NTC_TABLE_temp[i] + per*(NTC_TABLE_temp[i + 1] - NTC_TABLE_temp[i]);
    
    return temp;
}


void NTC_init(void)
{
    check_efuse();
    
    //Configure ADC
    if (unit == ADC_UNIT_1) {
        adc1_config_width(width);
        adc1_config_channel_atten(channel, atten);
    } else {
        adc2_config_channel_atten((adc2_channel_t)channel, atten);
    }

    //Characterize ADC
    adc_chars = calloc(1, sizeof(esp_adc_cal_characteristics_t));
    esp_adc_cal_value_t val_type = esp_adc_cal_characterize(unit, atten, width, DEFAULT_VREF, adc_chars);
    print_char_val_type(val_type);
}

int16_t NTC_read(void)
{
    uint32_t adc_reading = adc1_get_raw((adc1_channel_t)channel);
    float voltage        = esp_adc_cal_raw_to_voltage(adc_reading, adc_chars);
    
    /**
     *  (3.3-v)/4.7 = v/r => r = (4.7*v)/(3.3-v)
     */
    float r              = (DEFUALT_PULLUP_R*voltage)/(DEFAULT_VREF - voltage);
    ESP_LOGI(TAG, "V = %d mV, R = %f Ohm", (int)voltage, r);

    return ntc_lookup(r);
}