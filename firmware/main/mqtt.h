/*
 * mqtt.h
 *
 *  Created on: Mar 06, 2022
 *      Author: Phat.N
 */

#ifndef _MQTT_H_
#define _MQTT_H_

#include <stdint.h>

typedef void(*mqtt_event_cb_t)(uint8_t connect);

void MQTT_init(mqtt_event_cb_t event);
void MQTT_publish(uint8_t* topic, uint8_t* data, uint16_t len);

#endif /*_MQTT_H_*/