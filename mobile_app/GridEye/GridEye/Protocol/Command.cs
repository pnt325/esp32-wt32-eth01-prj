﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WT32EHT01.Protocol
{
    public enum Command
    {
        BLE_CMD_NONE,
        BLE_CMD_ACK,                    // ACK : CMD
        BLE_CMD_NACK,                   // NACK : CMD
        BLE_CMD_CONFIG_COMMIT,          // Save all configure to nvs
        BLE_CMD_WIFI_SSID,              // wifi ssid data
        BLE_CMD_WIFI_PASSWORD,          // wifi password data
        BLE_CMD_TEMP_OFFSET,            // temperature offset value
        BLE_CMD_TEMP_LIMIT,             // temperature limit value
        BLE_CMD_CONNECTION,             // connecion type value.
        BLE_CMD_PROBE_TEMP,             // temperature
        BLE_CMD_PROBE_DI,               // digital status
        BLE_CMD_WORK_HOUR,              // Set new work-hour
        BLE_CMD_SYNC_ENABLE,
        BLE_CMD_DEVICE_ID,
        BLE_CMD_DEVICE_TOKEN_1,
        BLE_CMD_DEVICE_TOKEN_2,
        BLE_CMD_DEVICE_TOKEN_3,
        BLE_CMD_DEVICE_ENABLE,
    }
}
