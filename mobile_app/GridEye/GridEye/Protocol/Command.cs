using System;
using System.Collections.Generic;
using System.Text;

namespace GridEye.Protocol
{
    public enum Command
    {
        BLE_CMD_NONE = 0x00,
 
        // Command data:  keep it
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

        /** BLE SETUP COMMAND */
        BLE_CMD_INVALID,
        BLE_CMD_RF_SYNC,     /** Send command sync radio with main and smoke */
        BLE_CMD_TEMPERATURE_SCAN, /** Start stop scan temperature */
        BLE_CMD_MODE,             /** Get set mode state */
        BLE_CMD_STOVE_TYPE,       /** Stove type */
        BLE_CMD_SMOKE_SYNC,
        BLE_CMD_COOK_AREA,
        BLE_CMD_SAVE_CONFIG,

        /** BLE NORMAL COMMAND */
        BLE_CMD_SETUP,   /** Enter exit, setup state */
        BLE_CMD_MAIN_INFO,    /** Get main info */
        BLE_CMD_SENSOR_INFO,  /** Get sensor info */
        BLE_CMD_STOVE_LOCK,   /** Set stove lock to main */
        BLE_CMD_SAFE_PROFILE, /** Get safety profile */
        BLE_CMD_SN,

        BLE_CMD_MOVE_DATA, /** Movement data */
        BLE_CMD_TEMP_ROW_0,
        BLE_CMD_TEMP_ROW_1,
        BLE_CMD_TEMP_ROW_2,
        BLE_CMD_TEMP_ROW_3,
        BLE_CMD_TEMP_ROW_4,
        BLE_CMD_TEMP_ROW_5,
        BLE_CMD_TEMP_ROW_6,
        BLE_CMD_TEMP_ROW_7,
        BLE_CMD_TEMP_ROW_8,
        BLE_CMD_TEMP_ROW_9,
        BLE_CMD_TEMP_ROW_10,
        BLE_CMD_TEMP_ROW_11,
        BLE_CMD_TEMP_ROW_12,
        BLE_CMD_TEMP_ROW_13,
        BLE_CMD_TEMP_ROW_14,
        BLE_CMD_TEMP_ROW_15,
        BLE_CMD_SAFE_ENHANCE,
        BLE_CMD_HUMAN_AREA,
    }
}
