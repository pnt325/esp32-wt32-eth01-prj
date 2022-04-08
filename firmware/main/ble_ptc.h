/*
 * ble.c
 *
 *  Created on: Apr 03, 2022
 *      Author: Phat.N
 */

#ifndef _BLE_PTC_H_
#define _BLE_PTC_H_

#include <stdint.h>
#include <stdbool.h>
#include <stdio.h>

#include "esp_log.h"

#define BLE_PTC_DATA_SIZE       128

#define BLE_PTC_TAG "BLE_PTC"

typedef struct ble_pack_t
{
    uint8_t cmd;
    uint8_t len;
    uint8_t datas[BLE_PTC_DATA_SIZE];
};

bool BLE_PTC_parse(const uint8_t* datas, ble_pack_t* pack)
{
    if(datas == NULL || pack){
        ESP_LOGE(BLE_PTC_TAG, "Input param null");
        return false;
    }

    pack->cmd = datas[0];
    pack->len = datas[1];
    if(pack->len >= BLE_PTC_DATA_SIZE){
        ESP_LOGE(BLE_PTC_TAG, "Data size invalid");
        return false;
    }

    uint8_t sum = 0xff;
    sum += pack->len;
    sum += pack->cmd;
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

#endif /*_BLE_PTC_H_*/
