/*
 * blc_ptc.h
 *
 *  Created on: Apr 03, 2022
 *      Author: Phat.N
 */

#ifndef _BLE_PTC_H_
#define _BLE_PTC_H_

#include <stdint.h>
#include <stdbool.h>
#include <stdio.h>

#define BLE_PTC_DATA_SIZE       128

typedef struct 
{
    uint8_t cmd;
    uint8_t len;
    uint8_t datas[BLE_PTC_DATA_SIZE];
}ble_pack_t;

bool BLE_PTC_parse(const uint8_t* datas, uint8_t len, ble_pack_t* pack);
bool BLE_PTC_package(uint8_t cmd, const uint8_t* data, uint8_t len, uint8_t* out_buf, uint8_t* out_len);

#endif /*_BLE_PTC_H_*/
