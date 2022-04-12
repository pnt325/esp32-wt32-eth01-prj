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
    UCFG_test();
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
    if(ret == ESP_OK)
    {
        ret = nvs_commit(nvs_cfg);
    }

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
    if (ret == ESP_OK)
    {
        ret = nvs_commit(nvs_cfg);
    }

    return (ret == ESP_OK);
}

bool UCFG_read_wifi_ssid(uint8_t* data, uint8_t* len)
{
    if(data == NULL || len  == NULL || *len < 32)
    {
        ESP_ERROR_CHECK(ESP_FAIL);
    }

    esp_err_t ret = nvs_get_str(nvs_cfg, UCFG_WIFI_SSID_STR, (char*)data, (size_t*)len);

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
    if(ret == ESP_OK)
    {
        ret = nvs_commit(nvs_cfg);
    }
    return (ret == ESP_OK);
}

bool UCFG_read_wifi_password(uint8_t* data, uint8_t* len)
{
    if(data == NULL || len == NULL || *len < 64)
    {
        ESP_ERROR_CHECK(ESP_FAIL);
    }

    esp_err_t ret = nvs_get_str(nvs_cfg, UCFG_WIFI_PSWD_STR, (char*)data, (size_t*)len);
    return (ret == ESP_OK);
}

bool UCFG_write_mqtt_port(uint16_t port)
{
    esp_err_t ret = nvs_set_u16(nvs_cfg, UCFG_MQTT_PORT_STR, port);
    if(ret == ESP_OK)
    {
        ret = nvs_commit(nvs_cfg);
    }
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

    if(strlen((const char*)data) != len)
    {
        ESP_ERROR_CHECK(ESP_FAIL);
    }

    esp_err_t ret = nvs_set_str(nvs_cfg, UCFG_MQTT_HOST_STR, ( const char*)data);
    if(ret == ESP_OK)
    {
        ret = nvs_commit(nvs_cfg);
    }
    return (ret == ESP_OK);
}

bool UCFG_read_mqtt_host(uint8_t* data, uint8_t* len)
{
    if(data ==  NULL || len == NULL || *len < 32)
    {
        ESP_ERROR_CHECK(ESP_FAIL);
    }

    esp_err_t ret = nvs_get_str(nvs_cfg, UCFG_MQTT_HOST_STR, (char*)data, (size_t*)len);
    return (ret == ESP_OK);
}

bool UCFG_write_temp_offset(float value)
{   
    size_t len = sizeof(float);   
    esp_err_t ret = nvs_set_blob(nvs_cfg, UCFG_TEMP_OFFSET_STR, &value, len);
    if(ret == ESP_OK)
    {
        ret = nvs_commit(nvs_cfg);
    }
    return (ret == ESP_OK);
}

bool UCFG_read_temp_offset(float* value)
{
    size_t len = sizeof(float);
    if (value == NULL)
    {
        ESP_ERROR_CHECK(ESP_FAIL);
    }
    esp_err_t ret = nvs_get_blob(nvs_cfg, UCFG_TEMP_OFFSET_STR, value, &len);

    return (ret == ESP_OK);
}

bool UCFG_write_temp_limit(uint8_t value)
{
    esp_err_t ret = nvs_set_u8(nvs_cfg, UCFG_TEMP_LIMIT_STR, value);
    if(ret == ESP_OK)
    {
        ret = nvs_commit(nvs_cfg);
    }
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
    esp_err_t ret = nvs_set_u8(nvs_cfg, UCFG_CONNECTION_STR, value);
    if(ret == ESP_OK)
    {
        ret = nvs_commit(nvs_cfg);
    }
    return (ret == ESP_OK);
}

bool UCFG_read_connection(uint8_t* value)
{
    if (value == NULL)
    {
        ESP_ERROR_CHECK(ESP_FAIL);
    }
    esp_err_t ret = nvs_get_u8(nvs_cfg, UCFG_CONNECTION_STR, value);
    return (ret == ESP_OK);
}

bool UCFG_write_mqtt_ca(uint8_t* data, uint16_t len)
{
    ESP_LOGI(UCFG_TAG, "CA data_len: %d, len: %d", strlen((const char*)data), len);
    if(data == NULL)
    {
        ESP_ERROR_CHECK(ESP_FAIL);
    }

    esp_err_t ret =  nvs_set_str(nvs_cfg, UCFG_MQTT_CA_STR, (const char*)data);
    if(ret == ESP_OK)
    {
        ret = nvs_commit(nvs_cfg);
    }
    return (ret == ESP_OK);
}

bool UCFG_read_mqtt_ca(uint8_t* data, uint16_t* len)
{
    esp_err_t ret = nvs_get_str(nvs_cfg, UCFG_MQTT_CA_STR, (char*)data, (size_t*)len);

    return (ret ==  ESP_OK);
}

void UCFG_test(void)
{
    uint8_t temp_limit = 0;

    if (UCFG_read_temp_limit(&temp_limit))
    {
        ESP_LOGI(UCFG_TAG, "Get temp limit: %d", temp_limit);
    }
    else
    {
        ESP_LOGI(UCFG_TAG, "Get temp Error: %d", temp_limit);
    }

    // Work hour
    uint32_t hours[3] = {0};
    if(UCFG_read_work_hour(hours))
    {
        ESP_LOGI(UCFG_TAG, "Get work hours: %d, %d, %d", hours[0], hours[1], hours[2]);
    }
    else
    {
        ESP_LOGI(UCFG_TAG, "Get work hours failure");
    }

    uint8_t wifi_ssid[32];
    uint8_t len = 32;
    if(UCFG_read_wifi_ssid(wifi_ssid, &len))
    {
        ESP_LOGI(UCFG_TAG, "Get wifi ssid: %s", wifi_ssid);
    }
    else 
    {
        ESP_LOGI(UCFG_TAG, "Get wifi ssid failure");
    }

    uint8_t wifi_pass[64];
    len = 64;
    if(UCFG_read_wifi_password(wifi_pass, &len))
    {
        ESP_LOGI(UCFG_TAG, "Get wifi passworkd: %s", wifi_pass);
    }
    else 
    {
        ESP_LOGI(UCFG_TAG, "Get wifi password failure");
    }

    uint16_t mqtt_port;
    if(UCFG_read_mqtt_port(&mqtt_port))
    {
        ESP_LOGI(UCFG_TAG, "Get mqtt port: %d", mqtt_port);
    }
    else 
    {
        ESP_LOGI(UCFG_TAG, "Get mqtt port failure");
    }

    uint8_t mqtt_host[32];
    len = 32;
    if(UCFG_read_mqtt_host(mqtt_host, &len))
    {
        ESP_LOGI(UCFG_TAG, "Get mqtt host: %s", mqtt_host);
    }
    else 
    {
        ESP_LOGI(UCFG_TAG, "Get mqtt host failure");
    }

    float temp_offset = 0.5f;
    if(UCFG_write_temp_offset(temp_offset))
    {
        ESP_LOGI(UCFG_TAG, "Get temp offset success");
    }
    else 
    {
        ESP_LOGI(UCFG_TAG, "Get temp offset failure");
    }

    uint8_t connection = 0;
    if(UCFG_read_connection(&connection))
    {
        ESP_LOGI(UCFG_TAG, "Get connection: %d", connection);
    }
    else 
    {
        ESP_LOGI(UCFG_TAG, "Get connection failure");
    }
}

