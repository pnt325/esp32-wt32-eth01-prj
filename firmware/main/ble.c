/*
 * ble.c
 *
 *  Created on: Apr 03, 2022
 *      Author: Phat.N
 */

#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include "freertos/FreeRTOS.h"
#include "freertos/task.h"
#include "freertos/semphr.h"
#include "freertos/event_groups.h"
#include "esp_system.h"
#include "esp_log.h"
#include "nvs_flash.h"
#include "esp_bt.h"

#include "esp_gap_ble_api.h"
#include "esp_gatts_api.h"
#include "esp_bt_defs.h"
#include "esp_bt_main.h"
#include "esp_gatt_common_api.h"

#include "sdkconfig.h"
#include "ble_ptc.h"
#include "ble_data.h"
#include "ucfg.h"
#include "app.h"

#define GATTS_TAG "BLE"

static void gatts_profile_a_event_handler(esp_gatts_cb_event_t event, esp_gatt_if_t gatts_if, esp_ble_gatts_cb_param_t *param);

#define GATTS_SERVICE_UUID_TEST_A   0x00FF
#define GATTS_CHAR_UUID_TEST_A      0xFF01
#define GATTS_DESCR_UUID_TEST_A     0x3333
#define GATTS_NUM_HANDLE_TEST_A     4

#define TEST_DEVICE_NAME            "WT32-ETH01"
#define TEST_MANUFACTURER_DATA_LEN  17

#define GATTS_DEMO_CHAR_VAL_LEN_MAX 0x40
#define PREPARE_BUF_MAX_SIZE        140

static bool                 is_sync_config      = false;
static bool                 sync_enable         = false;
static uint8_t              connection_type     = CONNECTION_NONE;
static bool                 is_notify           = 0;
static SemaphoreHandle_t    send_block;
static uint8_t              ble_data[BLE_PTC_DATA_SIZE];
static uint8_t              char1_str[]         = {0x11, 0x22, 0x33};
static esp_gatt_char_prop_t a_property          = 0;
static uint8_t              adv_config_done     = 0;

static esp_attr_value_t gatts_demo_char1_val = {
    .attr_max_len = GATTS_DEMO_CHAR_VAL_LEN_MAX,
    .attr_len = sizeof(char1_str),
    .attr_value = char1_str,
};

#define adv_config_flag         (1 << 0)
#define scan_rsp_config_flag    (1 << 1)

#ifdef CONFIG_SET_RAW_ADV_DATA
static uint8_t raw_adv_data[] = {
    0x02, 0x01, 0x06,
    0x02, 0x0a, 0xeb, 0x03, 0x03, 0xab, 0xcd};
static uint8_t raw_scan_rsp_data[] = {
    0x0f, 0x09, 0x45, 0x53, 0x50, 0x5f, 0x47, 0x41, 0x54, 0x54, 0x53, 0x5f, 0x44,
    0x45, 0x4d, 0x4f};
#else

static uint8_t adv_service_uuid128[16] = {
    0xfb,
    0x34,
    0x9b,
    0x5f,
    0x80,
    0x00,
    0x00,
    0x80,
    0x00,
    0x10,
    0x00,
    0x00,
    0xEE,
    0x00,
    0x00,
    0x00,
};

static esp_ble_adv_data_t adv_data = {
    .set_scan_rsp = false,
    .include_name = true,
    .include_txpower = false,
    .min_interval = 0x0006,
    .max_interval = 0x0010,
    .appearance = 0x00,
    .manufacturer_len = 0,
    .p_manufacturer_data = NULL,
    .service_data_len = 0,
    .p_service_data = NULL,
    .service_uuid_len = sizeof(adv_service_uuid128),
    .p_service_uuid = adv_service_uuid128,
    .flag = (ESP_BLE_ADV_FLAG_GEN_DISC | ESP_BLE_ADV_FLAG_BREDR_NOT_SPT),
};
// scan response data
static esp_ble_adv_data_t scan_rsp_data = {
    .set_scan_rsp = true,
    .include_name = true,
    .include_txpower = true,
    .appearance = 0x00,
    .manufacturer_len = 0,
    .p_manufacturer_data = NULL,
    .service_data_len = 0,
    .p_service_data = NULL,
    .service_uuid_len = sizeof(adv_service_uuid128),
    .p_service_uuid = adv_service_uuid128,
    .flag = (ESP_BLE_ADV_FLAG_GEN_DISC | ESP_BLE_ADV_FLAG_BREDR_NOT_SPT),
};

#endif /* CONFIG_SET_RAW_ADV_DATA */

static esp_ble_adv_params_t adv_params = {
    .adv_int_min = 0x20,
    .adv_int_max = 0x40,
    .adv_type = ADV_TYPE_IND,
    .own_addr_type = BLE_ADDR_TYPE_PUBLIC,
    .channel_map = ADV_CHNL_ALL,
    .adv_filter_policy = ADV_FILTER_ALLOW_SCAN_ANY_CON_ANY,
};

#define PROFILE_NUM 1
#define PROFILE_A_APP_ID 0

struct gatts_profile_inst
{
    esp_gatts_cb_t gatts_cb;
    uint16_t gatts_if;
    uint16_t app_id;
    uint16_t conn_id;
    uint16_t service_handle;
    esp_gatt_srvc_id_t service_id;
    uint16_t char_handle;
    esp_bt_uuid_t char_uuid;
    esp_gatt_perm_t perm;
    esp_gatt_char_prop_t property;
    uint16_t descr_handle;
    esp_bt_uuid_t descr_uuid;
};

static struct gatts_profile_inst gl_profile_tab[PROFILE_NUM] = {
    [PROFILE_A_APP_ID] = {
        .gatts_cb = gatts_profile_a_event_handler,
        .gatts_if = ESP_GATT_IF_NONE,
    }};

typedef struct
{
    uint8_t prepare_buf[PREPARE_BUF_MAX_SIZE];
    int prepare_len;
} prepare_type_env_t;

static prepare_type_env_t a_prepare_write_env;

static void received_handle(uint8_t* data, uint8_t len);
static bool ble_send(uint8_t* data, uint8_t len);
static bool ble_send_data(uint8_t cmd, uint8_t* data, uint8_t len);

void example_write_event_env(esp_gatt_if_t gatts_if, prepare_type_env_t *prepare_write_env, esp_ble_gatts_cb_param_t *param);
void example_exec_write_event_env(prepare_type_env_t *prepare_write_env, esp_ble_gatts_cb_param_t *param);

static void gap_event_handler(esp_gap_ble_cb_event_t event, esp_ble_gap_cb_param_t *param)
{
    switch (event)
    {
#ifdef CONFIG_SET_RAW_ADV_DATA
    case ESP_GAP_BLE_ADV_DATA_RAW_SET_COMPLETE_EVT:
        adv_config_done &= (~adv_config_flag);
        if (adv_config_done == 0)
        {
            esp_ble_gap_start_advertising(&adv_params);
        }
        break;
    case ESP_GAP_BLE_SCAN_RSP_DATA_RAW_SET_COMPLETE_EVT:
        adv_config_done &= (~scan_rsp_config_flag);
        if (adv_config_done == 0)
        {
            esp_ble_gap_start_advertising(&adv_params);
        }
        break;
#else
    case ESP_GAP_BLE_ADV_DATA_SET_COMPLETE_EVT:
        adv_config_done &= (~adv_config_flag);
        if (adv_config_done == 0)
        {
            esp_ble_gap_start_advertising(&adv_params);
        }
        break;
    case ESP_GAP_BLE_SCAN_RSP_DATA_SET_COMPLETE_EVT:
        adv_config_done &= (~scan_rsp_config_flag);
        if (adv_config_done == 0)
        {
            esp_ble_gap_start_advertising(&adv_params);
        }
        break;
#endif
    case ESP_GAP_BLE_ADV_START_COMPLETE_EVT:
        if (param->adv_start_cmpl.status != ESP_BT_STATUS_SUCCESS)
        {
            ESP_LOGE(GATTS_TAG, "Advertising start failed\n");
        }
        break;
    case ESP_GAP_BLE_ADV_STOP_COMPLETE_EVT:
        if (param->adv_stop_cmpl.status != ESP_BT_STATUS_SUCCESS)
        {
            ESP_LOGE(GATTS_TAG, "Advertising stop failed\n");
        }
        else
        {
            ESP_LOGI(GATTS_TAG, "Stop adv successfully\n");
        }
        break;
    case ESP_GAP_BLE_UPDATE_CONN_PARAMS_EVT:
        ESP_LOGI(GATTS_TAG, "update connection params status = %d, min_int = %d, max_int = %d,conn_int = %d,latency = %d, timeout = %d",
                 param->update_conn_params.status,
                 param->update_conn_params.min_int,
                 param->update_conn_params.max_int,
                 param->update_conn_params.conn_int,
                 param->update_conn_params.latency,
                 param->update_conn_params.timeout);
        break;
    default:
        break;
    }
}

void example_write_event_env(esp_gatt_if_t gatts_if, prepare_type_env_t *prepare_write_env, esp_ble_gatts_cb_param_t *param)
{
    esp_gatt_status_t status = ESP_GATT_OK;
    if (param->write.need_rsp)
    {
        if (param->write.is_prep)
        {
            if (param->write.offset > PREPARE_BUF_MAX_SIZE)
            {
                status = ESP_GATT_INVALID_OFFSET;
            }
            else if ((param->write.offset + param->write.len) > PREPARE_BUF_MAX_SIZE)
            {
                status = ESP_GATT_INVALID_ATTR_LEN;
            }

            esp_gatt_rsp_t *gatt_rsp = (esp_gatt_rsp_t *)malloc(sizeof(esp_gatt_rsp_t));
            gatt_rsp->attr_value.len = param->write.len;
            gatt_rsp->attr_value.handle = param->write.handle;
            gatt_rsp->attr_value.offset = param->write.offset;
            gatt_rsp->attr_value.auth_req = ESP_GATT_AUTH_REQ_NONE;
            memcpy(gatt_rsp->attr_value.value, param->write.value, param->write.len);
            esp_err_t response_err = esp_ble_gatts_send_response(gatts_if, param->write.conn_id, param->write.trans_id, status, gatt_rsp);
            if (response_err != ESP_OK)
            {
                ESP_LOGE(GATTS_TAG, "Send response error\n");
            }
            free(gatt_rsp);
            if (status != ESP_GATT_OK)
            {
                return;
            }
            memcpy(prepare_write_env->prepare_buf + param->write.offset,
                   param->write.value,
                   param->write.len);
            prepare_write_env->prepare_len += param->write.len;
        }
        else
        {
            esp_ble_gatts_send_response(gatts_if, param->write.conn_id, param->write.trans_id, status, NULL);
        }
    }
}

void example_exec_write_event_env(prepare_type_env_t *prepare_write_env, esp_ble_gatts_cb_param_t *param)
{
    if (param->exec_write.exec_write_flag == ESP_GATT_PREP_WRITE_EXEC)
    {
        received_handle(prepare_write_env->prepare_buf, prepare_write_env->prepare_len);
    }
    else
    {
        ESP_LOGI(GATTS_TAG, "ESP_GATT_PREP_WRITE_CANCEL");
    }
    prepare_write_env->prepare_len = 0;
}

static void gatts_profile_a_event_handler(esp_gatts_cb_event_t event, esp_gatt_if_t gatts_if, esp_ble_gatts_cb_param_t *param)
{
    switch (event)
    {
    case ESP_GATTS_REG_EVT:
        ESP_LOGI(GATTS_TAG, "REGISTER_APP_EVT, status %d, app_id %d\n", param->reg.status, param->reg.app_id);
        gl_profile_tab[PROFILE_A_APP_ID].service_id.is_primary = true;
        gl_profile_tab[PROFILE_A_APP_ID].service_id.id.inst_id = 0x00;
        gl_profile_tab[PROFILE_A_APP_ID].service_id.id.uuid.len = ESP_UUID_LEN_16;
        gl_profile_tab[PROFILE_A_APP_ID].service_id.id.uuid.uuid.uuid16 = GATTS_SERVICE_UUID_TEST_A;
        
        esp_err_t set_dev_name_ret = esp_ble_gap_set_device_name((TEST_DEVICE_NAME));
        if (set_dev_name_ret)
        {
            ESP_LOGE(GATTS_TAG, "set device name failed, error code = %x", set_dev_name_ret);
        }
#ifdef CONFIG_SET_RAW_ADV_DATA
        esp_err_t raw_adv_ret = esp_ble_gap_config_adv_data_raw(raw_adv_data, sizeof(raw_adv_data));
        if (raw_adv_ret)
        {
            ESP_LOGE(GATTS_TAG, "config raw adv data failed, error code = %x ", raw_adv_ret);
        }
        adv_config_done |= adv_config_flag;
        esp_err_t raw_scan_ret = esp_ble_gap_config_scan_rsp_data_raw(raw_scan_rsp_data, sizeof(raw_scan_rsp_data));
        if (raw_scan_ret)
        {
            ESP_LOGE(GATTS_TAG, "config raw scan rsp data failed, error code = %x", raw_scan_ret);
        }
        adv_config_done |= scan_rsp_config_flag;
#else
        // config adv data
        esp_err_t ret = esp_ble_gap_config_adv_data(&adv_data);
        if (ret)
        {
            ESP_LOGE(GATTS_TAG, "config adv data failed, error code = %x", ret);
        }
        adv_config_done |= adv_config_flag;
        // config scan response data
        ret = esp_ble_gap_config_adv_data(&scan_rsp_data);
        if (ret)
        {
            ESP_LOGE(GATTS_TAG, "config scan response data failed, error code = %x", ret);
        }
        adv_config_done |= scan_rsp_config_flag;

#endif
        esp_ble_gatts_create_service(gatts_if, &gl_profile_tab[PROFILE_A_APP_ID].service_id, GATTS_NUM_HANDLE_TEST_A);
        break;
    case ESP_GATTS_READ_EVT:
    {
        ESP_LOGI(GATTS_TAG, "GATT_READ_EVT, conn_id %d, trans_id %d, handle %d\n", param->read.conn_id, param->read.trans_id, param->read.handle);
        esp_gatt_rsp_t rsp;
        memset(&rsp, 0, sizeof(esp_gatt_rsp_t));
        rsp.attr_value.handle = param->read.handle;
        rsp.attr_value.len = 4;
        rsp.attr_value.value[0] = 0xde;
        rsp.attr_value.value[1] = 0xed;
        rsp.attr_value.value[2] = 0xbe;
        rsp.attr_value.value[3] = 0xef;
        esp_ble_gatts_send_response(gatts_if, param->read.conn_id, param->read.trans_id,
                                    ESP_GATT_OK, &rsp);
        break;
    }
    case ESP_GATTS_WRITE_EVT:
    {
        uint8_t user_data = 0;
        ESP_LOGI(GATTS_TAG, "GATT_WRITE_EVT, conn_id %d, trans_id %d, handle %d", param->write.conn_id, param->write.trans_id, param->write.handle);
        if (!param->write.is_prep)
        {
            ESP_LOGI(GATTS_TAG, "GATT_WRITE_EVT, value len %d, value :", param->write.len);
            if (gl_profile_tab[PROFILE_A_APP_ID].descr_handle == param->write.handle && param->write.len == 2)
            {
                uint16_t descr_value = param->write.value[1] << 8 | param->write.value[0];
                if (descr_value == 0x0001)
                {
                    if (a_property & ESP_GATT_CHAR_PROP_BIT_NOTIFY)
                    {
                        is_notify = true;
                        ESP_LOGI(GATTS_TAG, "notify enable");
                        uint8_t notify_data[15];
                        for (int i = 0; i < sizeof(notify_data); ++i)
                        {
                            notify_data[i] = i % 0xff;
                        }

                        // the size of notify_data[] need less than MTU size
                        esp_ble_gatts_send_indicate(gatts_if, param->write.conn_id, gl_profile_tab[PROFILE_A_APP_ID].char_handle,
                                                    sizeof(notify_data), notify_data, false);
                    }
                }
                else if (descr_value == 0x0002)
                {
                    if (a_property & ESP_GATT_CHAR_PROP_BIT_INDICATE)
                    {
                        ESP_LOGI(GATTS_TAG, "indicate enable");
                        uint8_t indicate_data[15];
                        for (int i = 0; i < sizeof(indicate_data); ++i)
                        {
                            indicate_data[i] = i % 0xff;
                        }
                        // the size of indicate_data[] need less than MTU size
                        esp_ble_gatts_send_indicate(gatts_if, param->write.conn_id, gl_profile_tab[PROFILE_A_APP_ID].char_handle,
                                                    sizeof(indicate_data), indicate_data, true);
                    }
                }
                else if (descr_value == 0x0000)
                {
                    is_notify = false;
                    ESP_LOGI(GATTS_TAG, "notify/indicate disable ");
                }
                else
                {
                    ESP_LOGE(GATTS_TAG, "unknown descr value");
                    esp_log_buffer_hex(GATTS_TAG, param->write.value, param->write.len);
                }
            }
            else
            {
                ESP_LOGI(GATTS_TAG, "GATT_WRITE_EVT, user data, value len %d, value :", param->write.len);
                user_data = 1;
            }
        }
        example_write_event_env(gatts_if, &a_prepare_write_env, param);

        if(user_data)
        {
            received_handle(param->write.value, param->write.len);
        }

        break;
    }
    case ESP_GATTS_EXEC_WRITE_EVT:
        ESP_LOGI(GATTS_TAG, "ESP_GATTS_EXEC_WRITE_EVT");
        esp_ble_gatts_send_response(gatts_if, param->write.conn_id, param->write.trans_id, ESP_GATT_OK, NULL);
        example_exec_write_event_env(&a_prepare_write_env, param);
        break;
    case ESP_GATTS_MTU_EVT:
        ESP_LOGI(GATTS_TAG, "ESP_GATTS_MTU_EVT, MTU %d", param->mtu.mtu);
        break;
    case ESP_GATTS_UNREG_EVT:
        break;
    case ESP_GATTS_CREATE_EVT:
        ESP_LOGI(GATTS_TAG, "CREATE_SERVICE_EVT, status %d,  service_handle %d\n", param->create.status, param->create.service_handle);
        gl_profile_tab[PROFILE_A_APP_ID].service_handle = param->create.service_handle;
        gl_profile_tab[PROFILE_A_APP_ID].char_uuid.len = ESP_UUID_LEN_16;
        gl_profile_tab[PROFILE_A_APP_ID].char_uuid.uuid.uuid16 = GATTS_CHAR_UUID_TEST_A;

        esp_ble_gatts_start_service(gl_profile_tab[PROFILE_A_APP_ID].service_handle);
        a_property = ESP_GATT_CHAR_PROP_BIT_WRITE | ESP_GATT_CHAR_PROP_BIT_NOTIFY;
        esp_err_t add_char_ret = esp_ble_gatts_add_char(gl_profile_tab[PROFILE_A_APP_ID].service_handle, &gl_profile_tab[PROFILE_A_APP_ID].char_uuid,
                                                        ESP_GATT_PERM_READ | ESP_GATT_PERM_WRITE,
                                                        a_property,
                                                        &gatts_demo_char1_val, NULL);
        if (add_char_ret)
        {
            ESP_LOGE(GATTS_TAG, "add char failed, error code =%x", add_char_ret);
        }
        break;
    case ESP_GATTS_ADD_INCL_SRVC_EVT:
        break;
    case ESP_GATTS_ADD_CHAR_EVT:
    {
        uint16_t length = 0;
        const uint8_t *prf_char;

        ESP_LOGI(GATTS_TAG, "ADD_CHAR_EVT, status %d,  attr_handle %d, service_handle %d\n",
                 param->add_char.status, param->add_char.attr_handle, param->add_char.service_handle);
        gl_profile_tab[PROFILE_A_APP_ID].char_handle = param->add_char.attr_handle;
        gl_profile_tab[PROFILE_A_APP_ID].descr_uuid.len = ESP_UUID_LEN_16;
        gl_profile_tab[PROFILE_A_APP_ID].descr_uuid.uuid.uuid16 = ESP_GATT_UUID_CHAR_CLIENT_CONFIG;
        esp_err_t get_attr_ret = esp_ble_gatts_get_attr_value(param->add_char.attr_handle, &length, &prf_char);
        if (get_attr_ret == ESP_FAIL)
        {
            ESP_LOGE(GATTS_TAG, "ILLEGAL HANDLE");
        }

        ESP_LOGI(GATTS_TAG, "the gatts demo char length = %x\n", length);
        for (int i = 0; i < length; i++)
        {
            ESP_LOGI(GATTS_TAG, "prf_char[%x] =%x\n", i, prf_char[i]);
        }
        esp_err_t add_descr_ret = esp_ble_gatts_add_char_descr(gl_profile_tab[PROFILE_A_APP_ID].service_handle, &gl_profile_tab[PROFILE_A_APP_ID].descr_uuid,
                                                               ESP_GATT_PERM_READ | ESP_GATT_PERM_WRITE, NULL, NULL);
        if (add_descr_ret)
        {
            ESP_LOGE(GATTS_TAG, "add char descr failed, error code =%x", add_descr_ret);
        }
        break;
    }
    case ESP_GATTS_ADD_CHAR_DESCR_EVT:
        gl_profile_tab[PROFILE_A_APP_ID].descr_handle = param->add_char_descr.attr_handle;
        ESP_LOGI(GATTS_TAG, "ADD_DESCR_EVT, status %d, attr_handle %d, service_handle %d\n",
                 param->add_char_descr.status, param->add_char_descr.attr_handle, param->add_char_descr.service_handle);
        break;
    case ESP_GATTS_DELETE_EVT:
        break;
    case ESP_GATTS_START_EVT:
        ESP_LOGI(GATTS_TAG, "SERVICE_START_EVT, status %d, service_handle %d\n",
                 param->start.status, param->start.service_handle);
        break;
    case ESP_GATTS_STOP_EVT:
        break;
    case ESP_GATTS_CONNECT_EVT:
    {
        esp_ble_conn_update_params_t conn_params = {0};
        memcpy(conn_params.bda, param->connect.remote_bda, sizeof(esp_bd_addr_t));
        /* For the IOS system, please reference the apple official documents about the ble connection parameters restrictions. */
        conn_params.latency = 0;
        conn_params.max_int = 0x20; // max_int = 0x20*1.25ms = 40ms
        conn_params.min_int = 0x10; // min_int = 0x10*1.25ms = 20ms
        conn_params.timeout = 400;  // timeout = 400*10ms = 4000ms
        ESP_LOGI(GATTS_TAG, "ESP_GATTS_CONNECT_EVT, conn_id %d, remote %02x:%02x:%02x:%02x:%02x:%02x:",
                 param->connect.conn_id,
                 param->connect.remote_bda[0], param->connect.remote_bda[1], param->connect.remote_bda[2],
                 param->connect.remote_bda[3], param->connect.remote_bda[4], param->connect.remote_bda[5]);
        gl_profile_tab[PROFILE_A_APP_ID].conn_id = param->connect.conn_id;
        // start sent the update connection parameters to the peer device.
        esp_ble_gap_update_conn_params(&conn_params);
        break;
    }
    case ESP_GATTS_DISCONNECT_EVT:
        ESP_LOGI(GATTS_TAG, "ESP_GATTS_DISCONNECT_EVT, disconnect reason 0x%x", param->disconnect.reason);
        esp_ble_gap_start_advertising(&adv_params);
        is_notify = false;
        break;
    case ESP_GATTS_CONF_EVT:
        // ESP_LOGI(GATTS_TAG, "ESP_GATTS_CONF_EVT, status %d attr_handle %d", param->conf.status, param->conf.handle);
        if (param->conf.status != ESP_GATT_OK)
        {
            esp_log_buffer_hex(GATTS_TAG, param->conf.value, param->conf.len);
        }
        break;
    case ESP_GATTS_OPEN_EVT:
    case ESP_GATTS_CANCEL_OPEN_EVT:
    case ESP_GATTS_CLOSE_EVT:
    case ESP_GATTS_LISTEN_EVT:
    case ESP_GATTS_CONGEST_EVT:
    default:
        break;
    }
}

static void gatts_event_handler(esp_gatts_cb_event_t event, esp_gatt_if_t gatts_if, esp_ble_gatts_cb_param_t *param)
{
    /* If event is register event, store the gatts_if for each profile */
    if (event == ESP_GATTS_REG_EVT)
    {
        if (param->reg.status == ESP_GATT_OK)
        {
            gl_profile_tab[param->reg.app_id].gatts_if = gatts_if;
        }
        else
        {
            ESP_LOGI(GATTS_TAG, "Reg app failed, app_id %04x, status %d\n",
                     param->reg.app_id,
                     param->reg.status);
            return;
        }
    }

    /* If the gatts_if equal to profile A, call profile A cb handler,
     * so here call each profile's callback */
    do
    {
        int idx;
        for (idx = 0; idx < PROFILE_NUM; idx++)
        {
            if (gatts_if == ESP_GATT_IF_NONE || /* ESP_GATT_IF_NONE, not specify a certain gatt_if, need to call every profile cb function */
                gatts_if == gl_profile_tab[idx].gatts_if)
            {
                if (gl_profile_tab[idx].gatts_cb)
                {
                    gl_profile_tab[idx].gatts_cb(event, gatts_if, param);
                }
            }
        }
    } while (0);
}

bool BLE_start(void)
{
    esp_err_t ret;

    send_block = xSemaphoreCreateMutex();
    ESP_ERROR_CHECK(esp_bt_controller_mem_release(ESP_BT_MODE_CLASSIC_BT));

    esp_bt_controller_config_t bt_cfg = BT_CONTROLLER_INIT_CONFIG_DEFAULT();
    ret = esp_bt_controller_init(&bt_cfg);

    if (ret)
    {
        ESP_LOGE(GATTS_TAG, "%s initialize controller failed: %s\n", __func__, esp_err_to_name(ret));
        return false;
    }

    ret = esp_bt_controller_enable(ESP_BT_MODE_BLE);
    if (ret)
    {
        ESP_LOGE(GATTS_TAG, "%s enable controller failed: %s\n", __func__, esp_err_to_name(ret));
        return false;
    }
    ret = esp_bluedroid_init();
    if (ret)
    {
        ESP_LOGE(GATTS_TAG, "%s init bluetooth failed: %s\n", __func__, esp_err_to_name(ret));
        return false;
    }
    ret = esp_bluedroid_enable();
    if (ret)
    {
        ESP_LOGE(GATTS_TAG, "%s enable bluetooth failed: %s\n", __func__, esp_err_to_name(ret));
        return false;
    }

    ret = esp_ble_gatts_register_callback(gatts_event_handler);
    if (ret)
    {
        ESP_LOGE(GATTS_TAG, "gatts register error, error code = %x", ret);
        return false;
    }
    ret = esp_ble_gap_register_callback(gap_event_handler);
    if (ret)
    {
        ESP_LOGE(GATTS_TAG, "gap register error, error code = %x", ret);
        return false;
    }
    ret = esp_ble_gatts_app_register(PROFILE_A_APP_ID);
    if (ret)
    {
        ESP_LOGE(GATTS_TAG, "gatts app register error, error code = %x", ret);
        return false;
    }

    esp_err_t local_mtu_ret = esp_ble_gatt_set_local_mtu(500);
    if (local_mtu_ret)
    {
        ESP_LOGE(GATTS_TAG, "set local  MTU failed, error code = %x", local_mtu_ret);
        return false;
    }

    return true;
}

bool BLE_send_data(uint8_t cmd, uint8_t* data, uint8_t len)
{
    return ble_send_data(cmd, data, len);
}

bool BLE_is_notify(void)
{
    return is_notify;
}

bool BLE_is_sync_config(void)
{
    bool sync = is_sync_config;
    is_sync_config = false; 
    return sync;
}

static void ble_res_success(uint8_t cmd)
{
    ESP_LOGI(GATTS_TAG, "Response success: %d", cmd);
    if(ble_send_data(BLE_CMD_ACK, &cmd, 1) == false)
    {
        ESP_ERROR_CHECK(ESP_FAIL);
    }
}

static void ble_res_failure(uint8_t cmd)
{
    ESP_LOGI(GATTS_TAG, "Response failure: %d", cmd);
    ble_send_data(BLE_CMD_NACK, &cmd, 1);
}

static void received_handle(uint8_t* data, uint8_t len)
{
    ble_pack_t pack;
    if(BLE_PTC_parse(data, len, &pack) == false){
        return;
    }

    ESP_LOGI(GATTS_TAG, "Received packet, cmd: %d, len: %d", pack.cmd, pack.len);

    if(pack.cmd == BLE_CMD_WORK_HOUR)
    {
        ESP_LOGI(GATTS_TAG, "Write work _hour: %s", pack.datas);
        if(pack.len < 12)
        {
            ble_res_failure(pack.cmd);
            return;
        }
        uint32_t* wh = (uint32_t*)pack.datas;
        ESP_LOGI(GATTS_TAG, "%u, %u, %u", wh[0], wh[1], wh[2]);
        if(UCFG_write_work_hour(wh) == false)
        {
            for(uint8_t i = 0; i < NUMBER_OF_CHANNEL; i++)
            {
                work_hours[i] = wh[i];
            }
            ble_res_failure(pack.cmd);
        }
        else
        {
            ble_res_success(pack.cmd);
        }
        return;
    }

    if(pack.cmd == BLE_CMD_WIFI_SSID)
    {
        ESP_LOGI(GATTS_TAG, "Write WIFI ssid: %s", pack.datas);
        if(UCFG_write_wifi_ssid(pack.datas, pack.len))
        {
            ble_res_success(pack.cmd);
        }
        else
        {
            ble_res_failure(pack.cmd);
        }
        return;
    }

    if(pack.cmd == BLE_CMD_WIFI_PASSWORD)
    {
        ESP_LOGI(GATTS_TAG, "Write WIFI pswd: %s", pack.datas);
        if(UCFG_write_wifi_password(pack.datas, pack.len))
        {
            ble_res_success(pack.cmd);
        }
        else
        {
            ble_res_failure(pack.cmd);
        }

        return;
    }

    if(pack.cmd == BLE_CMD_TEMP_OFFSET)
    {
        if(pack.len == 0 || pack.len < 12){
            ESP_LOGE(GATTS_TAG, "CMD: %d, len: %d invalid", pack.cmd, pack.len);
            ble_res_failure(pack.cmd);
            return;
        }

        float* value = (float*)&pack.datas[0];
        for(uint8_t i = 0; i < NUMBER_OF_CHANNEL; i++)
        {
            if((value[i] < -5.0f) || (value[i] > 5.0f))
            {
                ble_res_failure(pack.cmd);
                break;
            }
        }

        ESP_LOGI(GATTS_TAG, "Write temp offset: %f, %f, %f", value[0], value[1], value[2]);
        if(UCFG_write_temp_offset(value))
        {
            for (uint8_t i = 0; i < NUMBER_OF_CHANNEL; i++)
            {
                temp_offset[i] = value[i];
            }
            ble_res_success(pack.cmd);
        }
        else
        {
            ble_res_failure(pack.cmd);
        }

        return;
    }

    if(pack.cmd == BLE_CMD_TEMP_LIMIT)
    {
        if(pack.len < 3){
            ESP_LOGE(GATTS_TAG, "CMD: %d, len: %d invalid", pack.cmd, pack.len);
            ble_res_failure(pack.cmd);
            return;
        }

        ESP_LOGI(GATTS_TAG, "Write temp limit: %d", pack.datas[0]);
        if(UCFG_write_temp_limit(pack.datas))
        {
            for(uint8_t i = 0; i < NUMBER_OF_CHANNEL; i++)
            {
                temp_limit[i] = pack.datas[i];
            }
            ble_res_success(pack.cmd);
        }
        else
        {
            ble_res_failure(pack.cmd);
        }

        return;
    }

    if(pack.cmd == BLE_CMD_CONNECTION)
    {
        if(pack.len == 0){
            ESP_LOGE(GATTS_TAG, "CMD: %d, len: %d invalid", pack.cmd, pack.len);
            ble_res_failure(pack.cmd);
            return;
        }

        if( pack.datas[0] != CONNECTION_WIFI && 
            pack.datas[0] != CONNECTION_ETH)
        {
            ESP_LOGE(GATTS_TAG, "CMD: %d, data: %d invalid", pack.cmd, pack.len);
            ble_res_failure(pack.cmd);
            return;
        }

        ESP_LOGI(GATTS_TAG, "Write connection: %s", pack.datas[0] == 1 ? "WIFI" : pack.datas[0] == 2 ? "ETH" : "Unknown");
        connection_type = pack.datas[0];
        ble_res_success(pack.cmd);
        return;
    }

    if(pack.cmd == BLE_CMD_CONFIG_COMMIT)
    {
        ESP_LOGI(GATTS_TAG, "Write connection: %s", connection_type == 1 ? "WIFI" : connection_type == 2 ? "ETH" : "Unknown");
        if(UCFG_write_connection(connection_type))
        {
            ble_res_success(pack.cmd);
        }
        else
        {
            ble_res_failure(pack.cmd);
        }
        return;
    }

    if(pack.cmd == BLE_CMD_SYNC_ENABLE)
    {
        if(pack.len < 1){
            ble_res_failure(pack.cmd);
        }
        else
        {
            if(pack.datas[0])
            {
                sync_enable = true;
            }
            else
            {
                sync_enable = false;
            }
        }

        return;
    }

    if(pack.cmd == BLE_CMD_DEVICE_ID)
    {
        BLE_send_data(BLE_CMD_DEVICE_TOKEN_1, (uint8_t*)device_token_1, strlen((const char*)device_token_1));
        BLE_send_data(BLE_CMD_DEVICE_TOKEN_2, (uint8_t*)device_token_2, strlen((const char*)device_token_2));
        BLE_send_data(BLE_CMD_DEVICE_TOKEN_3, (uint8_t*)device_token_3, strlen((const char*)device_token_3));
        BLE_send_data(BLE_CMD_DEVICE_ENABLE, device_enable, 3);

        // send work-hourt
        BLE_send_data(BLE_CMD_WORK_HOUR, (uint8_t*)work_hours, sizeof(uint32_t)*3);

        // send temp offset
        BLE_send_data(BLE_CMD_TEMP_LIMIT, temp_limit, 3);
        BLE_send_data(BLE_CMD_TEMP_OFFSET, (uint8_t*)temp_offset, 12);

        return;
    }

    if(pack.cmd == BLE_CMD_DEVICE_TOKEN_1)
    {
        if(pack.len != 10)
        {
            ESP_LOGE(GATTS_TAG, "CMD: %d, len: %d invalid", pack.cmd, pack.len);
            ble_res_failure(pack.cmd);
            return;
        }
        ESP_LOGI(GATTS_TAG, "Device token 1: %s", pack.datas);
        
        if(UCFG_write_device_token(0, pack.datas))
        {
            for (size_t i = 0; i < pack.len; i++)
            {
                device_token_1[i] = pack.datas[i];
            }
            ble_res_success(pack.cmd);
        }
        else
        {
            ble_res_failure(pack.cmd);
        }
        return;
    }

    if (pack.cmd == BLE_CMD_DEVICE_TOKEN_2)
    {
        if(pack.len != 10)
        {
            ESP_LOGE(GATTS_TAG, "CMD: %d, len: %d invalid", pack.cmd, pack.len);
            ble_res_failure(pack.cmd);
            return;
        }

        ESP_LOGI(GATTS_TAG, "Device token 2: %s", pack.datas);
        if (UCFG_write_device_token(1, pack.datas))
        {
            for (size_t i = 0; i < pack.len; i++)
            {
                device_token_2[i] = pack.datas[i];
            }
            ble_res_success(pack.cmd);
        }
        else
        {
            ble_res_failure(pack.cmd);
        }
        return;
    }

    if (pack.cmd == BLE_CMD_DEVICE_TOKEN_3)
    {
        if(pack.len != 10)
        {
            ESP_LOGE(GATTS_TAG, "CMD: %d, len: %d invalid", pack.cmd, pack.len);
            ble_res_failure(pack.cmd);
            return;
        }

        ESP_LOGI(GATTS_TAG, "Device token 3: %s", pack.datas);
        if (UCFG_write_device_token(2, pack.datas))
        {
            for (size_t i = 0; i < pack.len; i++)
            {
                device_token_3[i] = pack.datas[i];
            }
            ble_res_success(pack.cmd);
        }
        else
        {
            ble_res_failure(pack.cmd);
        }
        return;
    }

    if(pack.cmd == BLE_CMD_DEVICE_ENABLE)
    {
        if(pack.len < 3)
        {
            ESP_LOGE(GATTS_TAG, "CMD: %d, len: %d invalid", pack.cmd, pack.len);
            ble_res_failure(pack.cmd);
            return;
        }

        if(UCFG_write_device_enable(pack.datas))
        {
            for(uint8_t i = 0; i < NUMBER_OF_CHANNEL; i++)
            {
                device_enable[i] = pack.datas[i];
            }
            ble_res_success(pack.cmd);
        }
        else
        {
            ble_res_failure(pack.cmd);
        }
        return;
    }
}   

static bool ble_send(uint8_t* data, uint8_t len)
{
    if(data == NULL || len == 0){
        ESP_LOGE(GATTS_TAG, "ble_send param invalid");
        return false;
    }

    esp_err_t err = esp_ble_gatts_send_indicate(gl_profile_tab[0].gatts_if, gl_profile_tab[0].conn_id, gl_profile_tab[0].char_handle, len, data, false);
    if(err)
    {
        ESP_LOGE(GATTS_TAG, "Gatt send");
        return false;
    }

    return true;
}

static bool ble_send_data(uint8_t cmd, uint8_t* data, uint8_t len)
{
    uint8_t buf_len = 0;
    if(is_notify == false)
    {
        return false;
    }

    xSemaphoreTake(send_block, portMAX_DELAY);
    if(BLE_PTC_package(cmd, data, len, ble_data, &buf_len))
    {
        ble_send(ble_data, buf_len);
        xSemaphoreGive(send_block);
        return true;
    }
    xSemaphoreGive(send_block);
    return false;
}

