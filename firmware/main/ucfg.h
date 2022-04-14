/*
 * ucfg.h
 *
 *  Created on: Mar 06, 2022
 *      Author: Phat.N
 */

#ifndef _UCFG_H_
#define _UCFG_H_

#include <stdint.h>
#include <stdbool.h>

#define UCFG_NAMESPACE_STR      "user_config"
#define UCFG_WIFI_SSID_STR      "wifi_ssid"
#define UCFG_WIFI_PSWD_STR      "wifi_pswd"
#define UCFG_WORK_HOUR_STR      "work_hours"
#define UCFG_CONNECTION_STR     "connection"
#define UCFG_TEMP_OFFSET_STR    "temp_offset"
#define UCFG_TEMP_LIMIT_STR     "temp_limit"
#define UCFG_DEVICE_TOKEN_1     "device_token_1"
#define UCFG_DEVICE_TOKEN_2     "device_token_2"
#define UCFG_DEVICE_TOKEN_3     "device_token_3"
#define UCFG_DEVICE_ENABLE      "device_en"
#define UCFG_DEVICE_ENABLE_OLD  "device_en_old"

#define CONNECTION_NONE 0
#define CONNECTION_WIFI 1
#define CONNECTION_ETH  2

#define MQTT_CONFIG_NONE    0
#define MQTT_CONFIG_ACTIVE  1

extern uint8_t mqtt_ca_file[2048];

void UCFG_init(void);
bool UCFG_write_work_hour(uint32_t* hours);
bool UCFG_read_work_hour(uint32_t* hours);
bool UCFG_write_wifi_ssid(uint8_t* data, uint8_t len);
bool UCFG_read_wifi_ssid(uint8_t* data, uint8_t* len);
bool UCFG_write_wifi_password(uint8_t* data, uint8_t len);
bool UCFG_read_wifi_password(uint8_t* data, uint8_t* len);
bool UCFG_write_temp_offset(float* value);
bool UCFG_read_temp_offset(float* value);
bool UCFG_write_temp_limit(uint8_t* value);
bool UCFG_read_temp_limit(uint8_t* value);
bool UCFG_write_connection(uint8_t value);
bool UCFG_read_connection(uint8_t* value);
bool UCFG_write_device_token(uint8_t channel, uint8_t* data);
bool UCFG_read_device_token(uint8_t channel, uint8_t* data);
bool UCFG_write_device_enable(uint8_t* data);
bool UCFG_read_device_enable(uint8_t* data);
bool UCFG_write_device_enable_old(uint8_t* data);
bool UCFG_read_device_enable_old(uint8_t* data);

void UCFG_test(void);

#endif /*_UCFG_H_*/