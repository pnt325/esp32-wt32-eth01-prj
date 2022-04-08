/*
 * dio.h
 *
 *  Created on: Apr 03, 2022
 *      Author: Phat.N
 */

#ifndef _DI_H_
#define _DI_H_

#include <stdint.h>

void DIO_init(void);
uint8_t DIO_status(uint8_t channel);
uint8_t DIO_button_state(void);
void DIO_led_on(void);
void DIO_led_off(void);
void DIO_test(void);

#endif /*_DI_H_*/