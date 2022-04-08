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

#include "ble_ptc.h"
#include "esp_log.h"

bool BLE_PTC_parse(const uint8_t* datas, uint8_t len, ble_pack_t* pack)
{
    if(datas == NULL || pack == NULL){
        ESP_LOGE(BLE_PTC_TAG, "Input param null");
        return false;
    }

    pack->cmd = datas[0];
    pack->len = datas[1];
    if((pack->len >= BLE_PTC_DATA_SIZE) || (pack->len >= (len - 2))){
        ESP_LOGE(BLE_PTC_TAG, "Data size invalid");
        return false;
    }

    uint8_t sum = 0xff;
    sum += pack->cmd;
    sum += pack->len;
    for(uint8_t i = 0; i < pack->len;i++)
    {
        pack->datas[i] = datas[i + 2];
        sum += datas[i + 2];
    }

    if(sum != datas[i + 2])
    {
        ESP_LOGE(BLE_PTC_TAG, "Checksum invalid");
        return false;
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
    *out_len = 0;
    out_buf[*out_len] = cmd;
    sum += cmd;
    out_len++;
    out_buf[*out_len] = len;
    sum += len;
    out_len++;
    for(uint8_t i = 0; i < len; i++)
    {
        out_buf[*out_len] = data[i];
        out_len++;
        sum += data[i];
    }

    out_buf[*out_len] = sum;
    *out_len++;

    return true;
}
