/* sl_monitor.c - A SUBLEQ machine monitor for debugging */
/* Project: SUBLEQ */
/* Date: Sunday, April 30, 2023 */
/* See also: [[https://github.com/howerj/subleq]] */
#include <stdio.h>
#include <unistd.h>
#include <errno.h>
#include "sl_monitor.h"

#define reg_W 13
#define reg_X reg_W+1
#define reg_T reg_W+2
#define reg_H reg_W+3

#define reg_COLD 28
#define reg_LAST reg_COLD+1

#define reg_IP	31
#define reg_TOS reg_IP+1
#define reg_RP0 reg_IP+2
#define reg_RP	reg_IP+3
#define reg_SP0 reg_IP+4
#define reg_SP	reg_IP+5

void mon_state( short *m ){
  printf( "IP: %6d\tTOS: %6d\n", m[ reg_IP ], m[ reg_TOS ] );
  printf( "RP0: %6d\tRP: %6d\n", m[ reg_RP0 ], m[ reg_RP ] );
  printf( "SP0: %6d\tSP: %6d\n", m[ reg_SP0 ], m[ reg_SP ] );
  printf( "COLD: %6d\tLAST: %6d\n", m[ reg_COLD ], m[ reg_LAST ] );
  printf( "W: %6d\tX: %6d\tT: %6d\tH: %6d\n",
	  m[ reg_W ], m[ reg_X ], m[ reg_T ], m[ reg_H ] );
}

void mon_step( short *m, short p, short a, short b, short c ){
  char sel[32], *res;
  printf( "%4d> %4hd\t%4hd\t%4hd\n", p, a, b, c );
  printf( "Type: <RET>\t\t to step\
\n      m <addr>\t\t to read \
\n      w <addr> <val>\t to write \
	  \n" );
 ask:
  res = gets( sel );
  if( 0 == sel[0] ) return;

  int addr, val, n;
  switch( sel[0] ){
  case 'm':
    sscanf( sel, "%*s %d", &addr );
    printf( "%4d> %6d\n", addr, m[addr] );
    break;
  case 'w':
    n = sscanf( sel, "%*s %d %d", &addr, &val );
    if( 2 == n ){
      printf( "%4hd> %6hd\t", addr, m[addr] );
      m[addr] = val;
      printf( "-> %6d\n", m[addr] );
    }
    else{ printf( "Can't read!\n" ); }
    break;
  }
  goto ask;
}
