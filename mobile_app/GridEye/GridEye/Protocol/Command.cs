using System;
using System.Collections.Generic;
using System.Text;

namespace GridEye.Protocol
{
    public enum Command
    {
        BLE_CMD_NONE = 0x00,
        BLE_CMD_ACK = 1,
        BLE_CMD_NACK,
        BLE_CMD_CONFIG_COMMIT,
        BLE_CMD_WIFI_SSID,
        BLE_CMD_WIFI_PASSWORD,
        BLE_CMD_MQTT_PORT,
        BLE_CMD_MQTT_HOST,
        BLE_CMD_MQTT_CA_BEGIN,
        BLE_CMD_MQTT_CA_DATA,
        BLE_CMD_MQTT_CA_END,
        BLE_CMD_TEMP_OFFSET,
        BLE_CMD_TEMP_LIMIT,
        BLE_CMD_CONNECTION,
        BLE_CMD_PROBE_TEMP,
        BLE_CMD_PROBE_DI,
        BLE_CMD_WORK_HOUR,
        BLE_CMD_SYNC_ENABLE,
        BLE_CMD_DEVICE_ID
    }
}
