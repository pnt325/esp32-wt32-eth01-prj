/*
 * ntc.h
 *
 *  Created on: Apr 03, 2022
 *      Author: Phat.N
 */

#ifndef _NTC_H_
#define _NTC_H_

#include <stdint.h>

void NTC_init(void);
int16_t NTC_read(uint8_t channel);

#endif /*_NTC_H_*/
