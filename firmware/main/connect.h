/*
 * connect.h
 *
 *  Created on: Apr 02, 2022
 *      Author: Phat.N
 */

#ifndef _CONNECT_H_
#define _CONNECT_H_

#include <stdint.h>
#include <stdbool.h>

#define CONNECTED    1
#define DISCONNECTED 0

bool CONNECT_init(uint8_t connections);
void CONNECT_sub_connected_event(void(*callback)(void));
void CONNECT_sub_disconnected_event(void(*callback)(void));

#endif /*_CONNECT_H_*/