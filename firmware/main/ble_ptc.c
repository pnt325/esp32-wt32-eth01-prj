/*
 * blt_ptc.c
 *
 *  Created on: Apr 03, 2022
 *      Author: Phat.N
 */

 /**
  * BLE packet
  * +-----+-----+-------+
  * | cmd | len | datas |
  * +-----+-----+-------+
  * Max packet size: 128 bytes
  */

#include <string.h>
#include "ble_ptc.h"
#include "esp_log.h"

#define BLE_PTC_TAG "BLE_PTC"

bool BLE_PTC_parse(const uint8_t* datas, uint8_t len, ble_pack_t* pack)
{
    if(datas == NULL || pack == NULL){
        ESP_LOGE(BLE_PTC_TAG, "Input param null");
        return false;
    }

    memset(pack->datas, 0x00, BLE_PTC_DATA_SIZE);

    pack->cmd = datas[0];
    pack->len = datas[1];
    if((pack->len > BLE_PTC_DATA_SIZE) || (pack->len > (len - 3))){
        ESP_LOGE(BLE_PTC_TAG, "Data size invalid");
        return false;
    }

    uint8_t sum = 0xff;
    int i;
    
    for(i = 0; i < pack->len + 2; i++)
    {
        sum += datas[i];
    }

    if(sum  != datas[i])
    {
        ESP_LOGE(BLE_PTC_TAG, "Checksum invalid");
        return false;   
    }

    for(i = 0; i < pack->len;i++)
    {
        pack->datas[i] = datas[i + 2];
    }

    return true;
}

bool BLE_PTC_package(uint8_t cmd, const uint8_t* data, uint8_t len, uint8_t* out_buf, uint8_t* out_len)
{
    if((len != 0 && data == NULL) || (len >= BLE_PTC_DATA_SIZE) || (out_buf == NULL) || (out_len == NULL))
    {
        ESP_LOGE(BLE_PTC_TAG, "BLT_PTC_package input invalid");
        return false;
    }

    uint8_t sum = 0xff;
    uint8_t i;
    *out_len = 0;

    uint8_t index = 0;
    out_buf[index++] = cmd;
    out_buf[index++] = len;

    for(i = 0; i < len; i++)
    {
        out_buf[index++] = data[i];
    }

    for(i = 0; i < index; i++)
    {
        sum += out_buf[i];
    }

    out_buf[index] = sum;   
    index++;
    *out_len = index;

    return true;
}
