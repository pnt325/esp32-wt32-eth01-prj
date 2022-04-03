/* Ethernet Basic Example

   This example code is in the Public Domain (or CC0 licensed, at your option.)

   Unless required by applicable law or agreed to in writing, this
   software is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
   CONDITIONS OF ANY KIND, either express or implied.
*/
#include <stdio.h>
#include <string.h>
#include "freertos/FreeRTOS.h"
#include "freertos/task.h"
#include "esp_netif.h"
#include "esp_eth.h"
#include "esp_event.h"
#include "esp_log.h"
#include "driver/gpio.h"
#include "sdkconfig.h"
#include "nvs_flash.h"
#include "eth.h"
#include "wifi.h"
#include "ntc.h"
#include "ble.h"
#include "connect.h"

#define TAG "MAIN"

void app_main(void)
{
    //Initialize NVS
    esp_err_t ret = nvs_flash_init();
    if (ret == ESP_ERR_NVS_NO_FREE_PAGES || ret == ESP_ERR_NVS_NEW_VERSION_FOUND) {
      ESP_ERROR_CHECK(nvs_flash_erase());
      ret = nvs_flash_init();
    }
    ESP_ERROR_CHECK(ret);

    CONNECT_init();
    ESP_LOGI(TAG, "Main connected");

// #define NTC
// #define BLE

// #if defined ETH
//     ETH_start();
// #elif defined WIF
//     WIFI_start("Phatwifi", "a523456789")
// #elif defined NTC
//     NTC_init();
//     while(1)
//     {
//         int temp = NTC_read();
//         ESP_LOGI(TAG, "NTC Temperature: %d (degree)", temp);
//         vTaskDelay(pdMS_TO_TICKS(1000));
//     }
// #elif defined BLE
//     if(BLE_start() == false)
//     {
//         ESP_LOGE(TAG, "BLE Init error");
//         return;
//     }
// #endif
}
