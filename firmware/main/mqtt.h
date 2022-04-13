/*
 * mqtt.h
 *
 *  Created on: Mar 06, 2022
 *      Author: Phat.N
 */

#ifndef _MQTT_H_
#define _MQTT_H_

#include <stdint.h>
#include <stdbool.h>

typedef void(*mqtt_event_cb_t)(uint8_t connect);

void MQTT_init(mqtt_event_cb_t event);
bool MQTT_publish(const char* topic, const char* data, uint16_t len);
void MQTT_clear_connnect(void);

#endif /*_MQTT_H_*/