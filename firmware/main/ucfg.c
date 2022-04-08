/*
 * ucfg.c
 *
 *  Created on: Mar 06, 2022
 *      Author: Phat.N
 */

#include "ucfg.h"

void UCFG_init(void)
{

}

bool UCFG_load(void)
{
    return true;
}

void UCFG_work_hour_set(uint8_t index, uint32_t wh)
{

}


bool UCFG_write_wifi_ssid(uint8_t* data, uint8_t len)
{
    // TODO implement write ssid to nvs flash
    return true;
}

bool UCFG_read_wifi_ssid(uint8_t* data, uint8_t len)
{
    return true;
}

bool UCFG_write_wifi_password(uint8_t* data, uint8_t len)
{
    return true;
}

bool UCFG_read_wifi_password(uint8_t* data, uint8_t len)
{
    return true;
}

bool UCFG_write_mqtt_port(uint16_t port)
{
    return true;
}

bool UCFG_read_mqtt_port(uint16_t* port)
{
    return true;
}

bool UCFG_write_mqtt_host(uint8_t* data, uint8_t len)
{
    return true;
}

bool UCFG_read_mqtt_host(uint8_t* data, uint8_t* len)
{
    return true;
}

bool UCFG_write_temp_offet(int8_t value)
{

}

bool UCFG_read_temp_offset(int8_t* value)
{

}

bool UCFG_write_temp_limit(uint8_t value)
{

}

bool UCFG_read_temp_limit(uint8_t* value)
{

}

bool UCFG_write_connection(uint8_t value)
{

}

bool UCFG_read_connection(uint8_t* value)
{
    
}

bool UCFG_write_mqtt_ca(uint8_t* data, uint8_t len)
{

}

bool UCFG_read_mqtt_ca(uint8_t* data, uint8_t* len)
{
    
}

bool UCFG_save(void)
{
    
}

