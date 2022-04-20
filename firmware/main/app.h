/*
 * app.h
 *
 *  Created on: Apr 09, 2022
 *      Author: Phat.N
 */

#ifndef _APP_H_
#define _APP_H_

#include <stdint.h>

#define NUMBER_OF_CHANNEL           3

extern char        device_token_1[11];
extern char        device_token_2[11];
extern char        device_token_3[11];
extern uint8_t     device_enable[4];

extern uint32_t work_hours[NUMBER_OF_CHANNEL];      //! Current work-hour count, sec
extern float    temp_offset[3];                     //! Temperature offset value  
extern uint8_t  temp_limit[3];                      //! Temperaure limit to take alarm value.

void APP_init(void);
void APP_run(void);

#endif /*_APP_H_*/