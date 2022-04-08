/*
 * ucfg.c
 *
 *  Created on: Mar 06, 2022
 *      Author: Phat.N
 */

#include "ucfg.h"
#include "esp_err.h"
#include <stdio.h>
#include <string.h>
#include "freertos/FreeRTOS.h"
#include "freertos/task.h"
#include "esp_system.h"
#include "nvs_flash.h"
#include "nvs.h"
#include "esp_log.h"

static nvs_handle_t nvs_cfg;

#define UCFG_TAG "UCFG"

void UCFG_init(void)
{
    ESP_ERROR_CHECK(nvs_open(UCFG_NAMESPACE_STR, NVS_READWRITE, &nvs_cfg));
}

bool UCFG_write_work_hour(uint32_t* hours)
{
    /**
     * Work hour is array of uint32_t, 3 element
     */

    if(hours == NULL)
    {
        ESP_ERROR_CHECK(ESP_FAIL);
    }

    esp_err_t ret = nvs_set_blob(nvs_cfg, UCFG_WORK_HOUR_STR, hours, sizeof(uint32_t)*3);
    return (ret == ESP_OK);
}

bool UCFG_read_work_hour(uint32_t* hours)
{
    size_t data_size = sizeof(uint32_t)*3;

    if (hours == NULL)
    {
        ESP_ERROR_CHECK(ESP_FAIL);
    }

    esp_err_t ret = nvs_get_blob(nvs_cfg, UCFG_WORK_HOUR_STR, hours, &data_size);
    return (ret == ESP_OK);
}

bool UCFG_write_wifi_ssid(uint8_t* data, uint8_t len)
{
    /**
     * The the data should be null terminal
     */
    if(data == NULL || len  ==  0)
    {
        ESP_ERROR_CHECK(ESP_FAIL);
    }

    if(strlen((const char*)data) != len)
    {
        ESP_LOGE(UCFG_TAG, "data len invalid");
        return false;
    }

    esp_err_t ret = nvs_set_str(nvs_cfg, UCFG_WIFI_SSID_STR, (const char*)data);

    return (ret == ESP_OK);
}

bool UCFG_read_wifi_ssid(uint8_t* data, uint8_t len)
{
    static uint8_t ssid[32];
    size_t data_len = sizeof(ssid);
    if(data == NULL)
    {
        ESP_ERROR_CHECK(ESP_FAIL);
    }

    esp_err_t ret = nvs_get_str(nvs_cfg, UCFG_WIFI_SSID_STR, (const char*)ssid, &data_len);
    data = &ssid[0];

    return (ret == ESP_OK);
}

bool UCFG_write_wifi_password(uint8_t* data, uint8_t len)
{
    if(data == NULL || len  ==  0)
    {
        ESP_ERROR_CHECK(ESP_FAIL);
    }

    if(strlen((const char*)data) != len)
    {
        ESP_LOGE(UCFG_TAG, "data len invalid");
        return false;
    }

    esp_err_t ret = nvs_set_str(nvs_cfg, UCFG_WIFI_PSWD_STR, (const char*)data);
    return (ret == ESP_OK);
}

bool UCFG_read_wifi_password(uint8_t* data, uint8_t len)
{
    static uint8_t pass[64];
    size_t data_size = sizeof(pass);
    if(data == NULL)
    {
        ESP_ERROR_CHECK(ESP_FAIL);
    }

    esp_err_t ret = nvs_get_str(nvs_cfg, UCFG_WIFI_SSID_STR, (const char*)pass, &data_size);
    data = &pass[0];

    return (ret == ESP_OK);
}

bool UCFG_write_mqtt_port(uint16_t port)
{
    esp_err_t ret = nvs_set_u16(nvs_cfg, UCFG_MQTT_PORT_STR, port);

    return (ret == ESP_OK);
}

bool UCFG_read_mqtt_port(uint16_t* port)
{
    if(port == NULL)
    {
        ESP_ERROR_CHECK(ESP_FAIL);
    }

    esp_err_t ret = nvs_get_u16(nvs_cfg, UCFG_MQTT_PORT_STR, port);
    return (ret == ESP_OK);
}

bool UCFG_write_mqtt_host(uint8_t* data, uint8_t len)
{
    if(data == NULL || len == 0)
    {
        ESP_ERROR_CHECK(ESP_FAIL);
    }

    if(strlen((const char*) data) != len)
    {
        ESP_ERROR_CHECK(ESP_FAIL);
    }

    esp_err_t ret = nvs_set_str(nvs_cfg, UCFG_MQTT_HOST_STR, ( const char*)data);

    return (ret == ESP_OK);
}

bool UCFG_read_mqtt_host(uint8_t* data, uint8_t* len)
{
    static uint8_t mqtt_host[32];
    size_t data_size = sizeof(mqtt_host);
    
    if(data == NULL)
    {
        ESP_ERROR_CHECK(ESP_FAIL);
    }   

    esp_err_t ret = nvs_get_str(nvs_cfg, UCFG_MQTT_HOST_STR, (const char*)mqtt_host, &data_size);

    return (ret == ESP_OK);
}

bool UCFG_write_temp_offset(int8_t value)
{
    esp_err_t ret = nvs_set_i8(nvs_cfg, UCFG_TEMP_OFFSET_STR, value);
    return (ret == ESP_OK);
}

bool UCFG_read_temp_offset(int8_t* value)
{
    if (value == NULL)
    {
        ESP_ERROR_CHECK(ESP_FAIL);
    }
    esp_err_t ret = nvs_get_i8(nvs_cfg, UCFG_TEMP_OFFSET_STR, value);

    return (ret == ESP_OK);
}

bool UCFG_write_temp_limit(uint8_t value)
{
    esp_err_t ret = nvs_set_u8(nvs_cfg, UCFG_TEMP_LIMIT_STR, value);
    return (ret == ESP_OK);
}

bool UCFG_read_temp_limit(uint8_t* value)
{
    if(value == NULL)
    {
        ESP_ERROR_CHECK(ESP_FAIL);
    }
    esp_err_t ret = nvs_get_u8(nvs_cfg, UCFG_TEMP_LIMIT_STR, value);
    return (ret ==  ESP_OK);
}

bool UCFG_write_connection(uint8_t value)
{
    esp_err_t ret = nvs_set_u8(nvs_cfg, UCFG_TEMP_LIMIT_STR, value);
    return (ret == ESP_OK);
}

bool UCFG_read_connection(uint8_t* value)
{
    if (value == NULL)
    {
        ESP_ERROR_CHECK(ESP_FAIL);
    }
    esp_err_t ret = nvs_get_u8(nvs_cfg, UCFG_TEMP_LIMIT_STR, value);
    return (ret == ESP_OK);
}

bool UCFG_write_mqtt_ca(uint8_t* data, uint8_t len)
{
    if(data == NULL || strlen((const char*)data) != len)
    {
        ESP_ERROR_CHECK(ESP_FAIL);
    }

    esp_err_t ret =  nvs_set_str(nvs_cfg, UCFG_MQTT_CA_STR, (const char*)data);

    return (ret == ESP_OK);
}

bool UCFG_read_mqtt_ca(uint8_t* data, uint8_t* len)
{
    static uint8_t mqtt_ca[2048];
    size_t data_size = sizeof(mqtt_ca);

    esp_err_t ret = nvs_get_str(nvs_cfg, UCFG_MQTT_CA_STR, (const char*)data, &data_size);

    return (ret ==  ESP_OK);
}

void UCFG_test(void)
{
    uint8_t temp_limit = 0;

    while(true){
        vTaskDelay(pdTICKS_TO_MS(100));

        if(UCFG_read_temp_limit(&temp_limit))
        {
            ESP_LOGI(UCFG_TAG, "Get temp limit: %d", temp_limit);
        }
        else
        {
            ESP_LOGI(UCFG_TAG, "Get temp Error: %d", temp_limit);
        }
    }
}

