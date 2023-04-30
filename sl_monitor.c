/* sl_monitor.c - A SUBLEQ machine monitor for debugging */
/* Project: SUBLEQ */
/* Date: Sunday, April 30, 2023 */
/* See also: [[https://github.com/howerj/subleq]] */
#include <stdio.h>
#include <unistd.h>
#include <errno.h>
#include "sl_monitor.h"

#define reg_W 12
#define reg_X 13
#define reg_T 14
#define reg_H 15

#define reg_COLD 16
#define reg_LAST 17

#define reg_IP	18
#define reg_TOS 19
#define reg_RP0 20
#define reg_RP	21
#define reg_SP0 22
#define reg_SP	23

void mon_state( short *m ){
  printf( "IP: %6d\tTOS: %6d\n", m[ reg_IP ], m[ reg_TOS ] );
  printf( "RP0: %6d\tRP: %6d\n", m[ reg_RP0 ], m[ reg_RP ] );
  printf( "SP0: %6d\tSP: %6d\n", m[ reg_SP0 ], m[ reg_SP ] );
  printf( "COLD: %6d\tLAST: %6d\n", m[ reg_COLD ], m[ reg_LAST ] );
  printf( "W: %6d\tX: %6d\tT: %6d\tH: %6d\n",
	  m[ reg_W ], m[ reg_X ], m[ reg_T ], m[ reg_H ] );
}

void mon_step( short p, short a, short b, short c ){
  char sel;
  printf( "%4d> %4hd\t%4hd\t%4hd\nType s(tep) to continue: ", p, a, b, c );
  scanf( "%c", &sel );
}

