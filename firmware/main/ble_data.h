/*
 * blc_data.h
 *
 *  Created on: Apr 09, 2022
 *      Author: Phat.N
 */

 #ifndef _BLE_DATA_H_
 #define _BLE_DATA_H_

 #include <stdint.h>

enum {
    BLE_CMD_NONE,
    BLE_CMD_ACK,                    // ACK : CMD
    BLE_CMD_NACK,                   // NACK : CMD
    BLE_CMD_CONFIG_COMMIT,          // Save all configure to nvs
    BLE_CMD_WIFI_SSID,              // wifi ssid data
    BLE_CMD_WIFI_PASSWORD,          // wifi password data
    BLE_CMD_MQTT_PORT,              // mqtt port data 
    BLE_CMD_MQTT_HOST,              // mqtt host data
    BLE_CMD_MQTT_CA_BEGIN,          // mqtt CA write begin
    BLE_CMD_MQTT_CA_DATA,           // mqtt CA data 
    BLE_CMD_MQTT_CA_END,            // mqtt CA write finish
    BLE_CMD_TEMP_OFFSET,            // temperature offset value
    BLE_CMD_TEMP_LIMIT,             // temperature limit value
    BLE_CMD_CONNECTION,             // connecion type value.
    BLE_CMD_PROBE_TEMP,             // temperature
    BLE_CMD_PROBE_DI,               // digital status
    BLE_CMD_WORK_HOUR,              // Set new work-hour
    BLE_CMD_SYNC_ENABLE,
    BLE_CMD_DEVICE_ID,
};

 #endif /*_BLE_DATA_H_*/