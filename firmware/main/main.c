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
#include "dio.h"
#include "connect.h"
#include "ucfg.h"

#include "app.h"

#define TAG "MAIN"

static void app_test(void);

static void task_test(void* param)
{

  char* data = (char*)param;
  while(true)
  {
    ESP_LOGI(TAG, "%s", data);
    vTaskDelay(100);
  }
}

void app_main(void)
{
  // Initialize NVS
  esp_err_t ret = nvs_flash_init();
  if (ret == ESP_ERR_NVS_NO_FREE_PAGES || ret == ESP_ERR_NVS_NEW_VERSION_FOUND)
  {
    ESP_ERROR_CHECK(nvs_flash_erase());
    ret = nvs_flash_init();
  }
  ESP_ERROR_CHECK(ret);

  APP_init();
  // app_test();
}

static void app_test(void)
{
  return;
// #define TEST_DI
#ifdef TEST_DI
  DIO_init();
  DIO_test();
#endif

// #define TEST_NTC
#ifdef TEST_NTC 
  NTC_init(); 
  NTC_Test();
#endif

// #define TEST_UCFG
#ifdef TEST_UCFG
  UCFG_init();
  UCFG_test();
#endif 

}