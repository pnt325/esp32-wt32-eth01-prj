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
#define UCFG_MQTT_CONFIG_STR    "mqtt_config"
#define UCFG_MQTT_HOST_STR      "mqtt_host"
#define UCFG_MQTT_PORT_STR      "mqtt_port"
#define UCFG_MQTT_CA_STR        "mqtt_ca"
#define UCFG_WORK_HOUR_STR      "work_hours"
#define UCFG_CONNECTION_STR     "connection"
#define UCFG_TEMP_OFFSET_STR    "temp_offset"
#define UCFG_TEMP_LIMIT_STR     "temp_limit"

#define CONNECTION_NONE 0
#define CONNECTION_WIFI 1
#define CONNECTION_ETH  2

#define MQTT_CONFIG_NONE    0
#define MQTT_CONFIG_ACTIVE  1

void UCFG_init(void);
bool UCFG_load(void);
void UCFG_work_hour_set(uint8_t index, uint32_t wh);

bool UCFG_write_wifi_ssid(uint8_t* data, uint8_t len);
bool UCFG_read_wifi_ssid(uint8_t* data, uint8_t len);
bool UCFG_write_wifi_password(uint8_t* data, uint8_t len);
bool UCFG_read_wifi_password(uint8_t* data, uint8_t len);
bool UCFG_write_mqtt_port(uint16_t port);
bool UCFG_read_mqtt_port(uint16_t* port);
bool UCFG_write_mqtt_host(uint8_t* data, uint8_t len);
bool UCFG_read_mqtt_host(uint8_t* data, uint8_t* len);
bool UCFG_write_temp_offet(int8_t value);
bool UCFG_read_temp_offset(int8_t* value);
bool UCFG_write_temp_limit(uint8_t value);
bool UCFG_read_temp_limit(uint8_t* value);
bool UCFG_write_connection(uint8_t value);
bool UCFG_read_connection(uint8_t* value);
bool UCFG_write_mqtt_ca(uint8_t* data, uint8_t len);
bool UCFG_read_mqtt_ca(uint8_t* data, uint8_t* len);
bool UCFG_save(void);

#endif /*_UCFG_H_*/