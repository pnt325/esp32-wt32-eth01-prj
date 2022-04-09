/*
 * connect.c
 *
 *  Created on: Apr 02, 2022
 *      Author: Phat.N
 */

#include <stdio.h>
#include "freertos/FreeRTOS.h"
#include "freertos/semphr.h"
#include "connect.h"
#include "wifi.h"
#include "eth.h"
#include "esp_log.h"
#include "ucfg.h"

typedef void(*connection_evt_t)(void);

static connection_evt_t connected_evt;
static connection_evt_t disconnected_evt;

#define CONNECT_TAG "CONNECT"

bool CONNECT_init(uint8_t connection)
{
    if(connection != CONNECTION_WIFI && connection != CONNECTION_ETH)
    {
        ESP_LOGE(CONNECT_TAG, "Connection invalid:%d", connection);
        return false;
    }

    if (connection == CONNECTION_WIFI)
    {
        uint8_t ssid[32] = {0};
        uint8_t len = 32;
        uint8_t pswd[64] = {0};
        
        if(UCFG_read_wifi_ssid(ssid, &len) == false)
        {
            ESP_LOGE(CONNECT_TAG, "Get wifi ssid failure");
            return false;
        }

        len = 64;
        if(UCFG_read_wifi_password(pswd, &len) == false)
        {
            ESP_LOGE(CONNECT_TAG, "Get wifi password failure");
            return false;
        }

        WIFI_start((const char*)ssid, (const char*)pswd);
    }
    else if (connection == CONNECTION_ETH)
    {
        ETH_start();
    }

    // xSemaphoreTake(cnn_notify, portMAX_DELAY);

    return true;
}

void CONNECT_sub_connected_event(void(*callback)(void))
{
    connected_evt = callback;
}

void CONNECT_sub_disconnected_event(void(*callback)(void))
{
    disconnected_evt = callback;
}

void CONNECT_evt(uint8_t status)
{
    if(status == CONNECTED)
    {
        ESP_LOGI(CONNECT_TAG, "Connected");
        if(connected_evt)
        {
            connected_evt();
        }
    }
    else    //!  DISCONECTED
    {
        ESP_LOGI(CONNECT_TAG, "Disconnected");
        if(disconnected_evt)
        {
            disconnected_evt();
        }
    }
}

