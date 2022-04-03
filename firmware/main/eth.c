/*
 * eth.c
 *
 *  Created on: Mar 31, 2022
 *      Author: Phat.N
 */

#include <string.h>

#include "eth.h"

#include "freertos/FreeRTOS.h"
#include "freertos/semphr.h"
#include "esp_netif.h"
#include "esp_eth.h"
#include "esp_event.h"
#include "esp_log.h"
#include "driver/gpio.h"
#include "sdkconfig.h"
#include "connect.h"

#include "ping/ping_sock.h"

static const char *TAG = "ETH";

//! The pin enanle external OSC for LAN8720 and ESP ETH clock source
#define ETH_EMAC_ESC_ENABLE_IO              16
#define ETH_EMAC_ESC_ENABLE_IO_PIN_SEL      (1ULL << ETH_EMAC_ESC_ENABLE_IO)

//! ETH handle
static esp_eth_handle_t eth_handle = NULL;
// static SemaphoreHandle_t get_ip_notify;

extern void CONNECT_evt(uint8_t status);

static void eth_event_handler(void *arg, esp_event_base_t event_base,
                              int32_t event_id, void *event_data)
{
    uint8_t mac_addr[6] = {0};
    /* we can get the ethernet driver handle from event data */
    esp_eth_handle_t eth_handle = *(esp_eth_handle_t *)event_data;

    switch (event_id) {
    case ETHERNET_EVENT_CONNECTED:
        esp_eth_ioctl(eth_handle, ETH_CMD_G_MAC_ADDR, mac_addr);
        ESP_LOGI(TAG, "Ethernet Link Up");
        ESP_LOGI(TAG, "Ethernet HW Addr %02x:%02x:%02x:%02x:%02x:%02x",
                 mac_addr[0], mac_addr[1], mac_addr[2], mac_addr[3], mac_addr[4], mac_addr[5]);
        
        break;
    case ETHERNET_EVENT_DISCONNECTED:
        ESP_LOGI(TAG, "Ethernet Link Down");
        CONNECT_evt(DISCONNECTED);
        break;
    case ETHERNET_EVENT_START:
        ESP_LOGI(TAG, "Ethernet Started");
        break;
    case ETHERNET_EVENT_STOP:
        ESP_LOGI(TAG, "Ethernet Stopped");
        break;
    default:
        break;
    }
}

/** Event handler for IP_EVENT_ETH_GOT_IP */
static void got_ip_event_handler(void *arg, esp_event_base_t event_base,
                                 int32_t event_id, void *event_data)
{
    ip_event_got_ip_t *event = (ip_event_got_ip_t *) event_data;
    const esp_netif_ip_info_t *ip_info = &event->ip_info;

    ESP_LOGI(TAG, "Ethernet Got IP Address");
    ESP_LOGI(TAG, "ETHIP   :" IPSTR, IP2STR(&ip_info->ip));
    ESP_LOGI(TAG, "ETHMASK :" IPSTR, IP2STR(&ip_info->netmask));
    ESP_LOGI(TAG, "ETHGW   :" IPSTR, IP2STR(&ip_info->gw));

    CONNECT_evt(CONNECTED);
    // xSemaphoreGive(get_ip_notify);
}

void ETH_start()
{
    gpio_config_t io_conf = {};
    io_conf.intr_type    = GPIO_INTR_DISABLE;
    io_conf.mode         = GPIO_MODE_OUTPUT;
    io_conf.pin_bit_mask = ETH_EMAC_ESC_ENABLE_IO_PIN_SEL;
    io_conf.pull_down_en = 0;
    io_conf.pull_up_en = 0;
    gpio_config(&io_conf);
    gpio_set_level(ETH_EMAC_ESC_ENABLE_IO, 1);
    
    ESP_ERROR_CHECK(esp_netif_init());                //! Initialize TCP/IP network interface
    ESP_ERROR_CHECK(esp_event_loop_create_default()); //! Create default event loop that running in background

    //! Create new default instance of esp-netif for Ethernet
    esp_netif_config_t cfg = ESP_NETIF_DEFAULT_ETH();
    esp_netif_t *eth_netif = esp_netif_new(&cfg);

    //! Init MAC and PHY configs to default
    eth_mac_config_t mac_config = ETH_MAC_DEFAULT_CONFIG();
    eth_phy_config_t phy_config = ETH_PHY_DEFAULT_CONFIG();

    phy_config.phy_addr          = CONFIG_EXAMPLE_ETH_PHY_ADDR;
    phy_config.reset_gpio_num    = CONFIG_EXAMPLE_ETH_PHY_RST_GPIO;
    mac_config.smi_mdc_gpio_num  = CONFIG_EXAMPLE_ETH_MDC_GPIO;
    mac_config.smi_mdio_gpio_num = CONFIG_EXAMPLE_ETH_MDIO_GPIO;
    esp_eth_mac_t *mac           = esp_eth_mac_new_esp32(&mac_config);
    esp_eth_phy_t *phy           = esp_eth_phy_new_lan87xx(&phy_config);
    esp_eth_config_t config      = ETH_DEFAULT_CONFIG(mac, phy);
    ESP_ERROR_CHECK(esp_eth_driver_install(&config, &eth_handle));

    //! Attach Ethernet driver to TCP/IP stack
    ESP_ERROR_CHECK(esp_netif_attach(eth_netif, esp_eth_new_netif_glue(eth_handle)));

    // Register user defined event handers
    ESP_ERROR_CHECK(esp_event_handler_register(ETH_EVENT, ESP_EVENT_ANY_ID, &eth_event_handler, NULL));
    ESP_ERROR_CHECK(esp_event_handler_register(IP_EVENT, IP_EVENT_ETH_GOT_IP, &got_ip_event_handler, NULL));

    ESP_LOGI(TAG, "Init");

    ESP_ERROR_CHECK(esp_eth_start(eth_handle));
    ESP_LOGI(TAG, "Start");
}

void ETH_stop()
{
    ESP_ERROR_CHECK(esp_eth_stop(eth_handle));
    ESP_LOGI(TAG, "Stop");
}
