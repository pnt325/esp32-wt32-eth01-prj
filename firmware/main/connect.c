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

static uint8_t is_conn_evt = 0;
SemaphoreHandle_t cnn_notify;

#define CONNECT_TAG "CONNECT"

void CONNECT_init(void)
{
    cnn_notify = xSemaphoreCreateBinary();
    esp_err_t ret = cnn_notify != NULL ? ESP_OK : ESP_FAIL;
    ESP_ERROR_CHECK(ret);

    // TODO check what is connection selected
    WIFI_start("Phatwifi", "a523456789");
    xSemaphoreTake(cnn_notify, portMAX_DELAY);      //! Wait for connection establish
}

void CONNECT_evt(uint8_t status)
{
    if(status == CONNECTED)
    {
        ESP_LOGI(CONNECT_TAG, "Connected");
        if(is_conn_evt == 0)
        {
            xSemaphoreGive(cnn_notify);
            is_conn_evt = 1;    
        }
    }
    else    //!  DISCONECTED
    {
        ESP_LOGI(CONNECT_TAG, "Disconnected");
        // TODO Trigger blink LED
    }
}

