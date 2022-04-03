/*
 * wifi.h
 *
 *  Created on: Apr 02, 2022
 *      Author: Phat.N
 */

#ifndef _WIFI_H_
#define _WIFI_H_

#include <stdint.h>

void WIFI_start(const char* ssid, const char* pswd);
void WIFI_stop(void);

#endif /*_WIFI_H_*/