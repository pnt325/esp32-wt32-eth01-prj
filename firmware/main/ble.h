/*
 * ble.h
 *
 *  Created on: Apr 03, 2022
 *      Author: Phat.N
 */

#ifndef _BLE_H_
#define _BLE_H_

#include <stdint.h>

bool BLE_start(void);
bool BLE_send_data(uint8_t cmd, uint8_t* data, uint8_t len);

#endif /*_BLE_H_*/