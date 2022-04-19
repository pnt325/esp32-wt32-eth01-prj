/*
 * mqtt.c
 *
 *  Created on: Mar 06, 2022
 *      Author: Phat.N
 */

#include "mqtt.h"

#include <stdio.h>
#include <stdint.h>
#include <stddef.h>
#include <string.h>
#include "freertos/FreeRTOS.h"
#include "freertos/semphr.h"
#include "esp_system.h"
#include "nvs_flash.h"
#include "esp_event.h"
#include "esp_netif.h"

#include "esp_log.h"
#include "mqtt_client.h"
#include "esp_tls.h"
#include "esp_ota_ops.h"
#include <sys/param.h>
#include "ucfg.h"

static const char *TAG = "MQTTS_EXAMPLE";

static esp_mqtt_client_handle_t mqtt_client;
static SemaphoreHandle_t publish_block;
static mqtt_event_cb_t mqtt_event;

static bool mqtt_connect = false;

static const uint8_t mqtt_ca[] = "-----BEGIN CERTIFICATE-----\n"                                        \
                                 "MIIEBTCCAu2gAwIBAgIUA8HYfkSeRx2l6i3Lhj1fKYRXIbowDQYJKoZIhvcNAQEL\n"    \
                                 "BQAwgZExCzAJBgNVBAYTAlZOMRQwEgYDVQQIDAtIbyBDaGkgTWluaDEQMA4GA1UE\n"    \
                                 "BwwHVGh1IER1YzEPMA0GA1UECgwGQ0EgUG50MQ0wCwYDVQQLDARUZXN0MRYwFAYD\n"    \
                                 "VQQDDA0xOTIuMTY4LjAuMTA1MSIwIAYJKoZIhvcNAQkBFhNwaGF0Lm50QGhvdG1h\n"    \
                                 "aWwuY29tMB4XDTIyMDQwOTE3MTMxN1oXDTMyMDQwNjE3MTMxN1owgZExCzAJBgNV\n"    \
                                 "BAYTAlZOMRQwEgYDVQQIDAtIbyBDaGkgTWluaDEQMA4GA1UEBwwHVGh1IER1YzEP\n"    \
                                 "MA0GA1UECgwGQ0EgUG50MQ0wCwYDVQQLDARUZXN0MRYwFAYDVQQDDA0xOTIuMTY4\n"    \
                                 "LjAuMTA1MSIwIAYJKoZIhvcNAQkBFhNwaGF0Lm50QGhvdG1haWwuY29tMIIBIjAN\n"    \
                                 "BgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAqcp7wq3YRaeA5RHaVWQEoyC0GwFo\n"    \
                                 "Ahn5RNKQvreUgDqn0His6qbHLjNGv1wwMeLLBpnuuPlP+SssliEpp2jAWL564zC7\n"    \
                                 "muxOcxWG6q7HvzNkAXNlBmCSBUaCwKvg+F5kA8QI3WiQlGwyAqqK9KS/QswZUEs0\n"    \
                                 "LQQhJqh9MK1tGTn8qblWQh6ZgXVO0PjxGSsFnNyCQc0Surukoba5sm3PDG/sdCc+\n"    \
                                 "w6BodQiFjFdlaeYyM17l1bkXH5b35Knje9rtVwiTb1sf8MXHppSh2xZpqu2m6SI/\n"    \
                                 "Fr2ZWGlq2grsTp6lfvXJr1u/G4KhdsKWZpvBmVZIX1RGWXRU864yw0VIywIDAQAB\n"    \
                                 "o1MwUTAdBgNVHQ4EFgQUCLDafQSD7l7SsUsGMvNHjjKsabAwHwYDVR0jBBgwFoAU\n"    \
                                 "CLDafQSD7l7SsUsGMvNHjjKsabAwDwYDVR0TAQH/BAUwAwEB/zANBgkqhkiG9w0B\n"    \
                                 "AQsFAAOCAQEANZQbhOIiAtE1H1BFsTFMwNmH7QQT5Vndc1yGOC2hLzKmzDGrup+3\n"    \
                                 "S5W8DMGxyyDH0jSG1I0HNY+xYAGyR/fuTBOp+lwaMQAOS0P8YAo1OcE/Exl8ZfXJ\n"    \
                                 "0dcM9lb/xPYQxKFyfTOeHMCa2QUAXh5iSr24yJo5Pf/Yxy7LP0dJYZrAeFiLY4pb\n"    \
                                 "ZCjRb7GdgNT64GjxBHkX93dqVX+lX1Z/BgLJp2mWoFqdOjjJD4cbZYfm1SqiFFC8\n"    \
                                 "j6U8I6aBJx9iHFZug/1FQIWO5KeX++bhLH8VlcQ5/sCK+ENArlHRsk88JvWs9578\n"    \
                                 "2mzGHMYBWaUYMYtvPsQ1AFSIhPO0S/9PMw==\n"                                \
                                 "-----END CERTIFICATE-----";

static void mqtt_event_handler(void *handler_args, esp_event_base_t base, int32_t event_id, void *event_data)
{
    ESP_LOGD(TAG, "Event dispatched from event loop base=%s, event_id=%d", base, event_id);
    esp_mqtt_event_handle_t event = event_data;
    // esp_mqtt_client_handle_t client = event->client;

    // int msg_id;
    switch ((esp_mqtt_event_id_t)event_id) {
    case MQTT_EVENT_CONNECTED:
        ESP_LOGI(TAG, "MQTT_EVENT_CONNECTED");
        mqtt_connect = true;
        if(mqtt_event)
        {
            mqtt_event(mqtt_connect);
        }
        break;
    case MQTT_EVENT_DISCONNECTED:
        ESP_LOGI(TAG, "MQTT_EVENT_DISCONNECTED");
        mqtt_connect = false;
        if(mqtt_event)
        {
            mqtt_event(mqtt_connect);
        }
        break;

    case MQTT_EVENT_SUBSCRIBED:
        ESP_LOGI(TAG, "MQTT_EVENT_SUBSCRIBED, msg_id=%d", event->msg_id);
        break;
    case MQTT_EVENT_UNSUBSCRIBED:
        ESP_LOGI(TAG, "MQTT_EVENT_UNSUBSCRIBED, msg_id=%d", event->msg_id);
        break;
    case MQTT_EVENT_PUBLISHED:
        ESP_LOGI(TAG, "MQTT_EVENT_PUBLISHED, msg_id=%d", event->msg_id);
        break;
    case MQTT_EVENT_DATA:
        ESP_LOGI(TAG, "MQTT_EVENT_DATA");
        printf("TOPIC=%.*s\r\n", event->topic_len, event->topic);
        printf("DATA=%.*s\r\n", event->data_len, event->data);
        break;
    case MQTT_EVENT_ERROR:
        ESP_LOGI(TAG, "MQTT_EVENT_ERROR");
        if (event->error_handle->error_type == MQTT_ERROR_TYPE_TCP_TRANSPORT) {
            ESP_LOGI(TAG, "Last error code reported from esp-tls: 0x%x", event->error_handle->esp_tls_last_esp_err);
            ESP_LOGI(TAG, "Last tls stack error number: 0x%x", event->error_handle->esp_tls_stack_err);
            ESP_LOGI(TAG, "Last captured errno : %d (%s)",  event->error_handle->esp_transport_sock_errno,
                     strerror(event->error_handle->esp_transport_sock_errno));
        } else if (event->error_handle->error_type == MQTT_ERROR_TYPE_CONNECTION_REFUSED) {
            ESP_LOGI(TAG, "Connection refused error: 0x%x", event->error_handle->connect_return_code);
        } else {
            ESP_LOGW(TAG, "Unknown error type: 0x%x", event->error_handle->error_type);
        }

        if(mqtt_event)
        {
            mqtt_event(0);
        }
        break;
    default:
        ESP_LOGI(TAG, "Other event id:%d", event->event_id);
        break;
    }
}

void MQTT_init(mqtt_event_cb_t event)
{
    //! Create semaphore
    publish_block = xSemaphoreCreateMutex();

    const esp_mqtt_client_config_t mqtt_cfg = {
        .uri = "mqtts://192.168.0.105:8883",    // TODO Change it
        .cert_pem = (const char *)mqtt_ca,      // TODO Change it
        .username = "uwt32",                    // TODO change it
        .password = "wt32"                      // TODO change it
    };

    ESP_LOGI(TAG, "[APP] Free memory: %d bytes", esp_get_free_heap_size());
    mqtt_client = esp_mqtt_client_init(&mqtt_cfg);
    /* The last argument may be used to pass data to the event handler, in this example mqtt_event_handler */
    esp_mqtt_client_register_event(mqtt_client, ESP_EVENT_ANY_ID, mqtt_event_handler, NULL);
    esp_mqtt_client_start(mqtt_client);

    mqtt_event = event;
}

bool MQTT_publish(const char* topic, const char* data, uint16_t len)
{
    if(mqtt_connect == false)
    {
        ESP_LOGW(TAG, "Publish reject: mqtt disconnected");
        return false;
    }

    if(topic == NULL || data == NULL || len == 0)
    {
        ESP_ERROR_CHECK(ESP_FAIL);
    }

    xSemaphoreTake(publish_block, portMAX_DELAY);
    ESP_LOGI(TAG, "Publish: %s: %s", topic, data);
    esp_mqtt_client_publish(mqtt_client, topic, data, len, 0, 0);
    xSemaphoreGive(publish_block);

    return true;
}

void MQTT_clear_connnect(void)
{
    mqtt_connect = false;
}
