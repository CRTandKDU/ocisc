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
#define reg_HERE reg_COLD+2

#define reg_R0  16
#define reg_R1  reg_R0+1
#define reg_R2  reg_R0+2
#define reg_R3  reg_R0+3
#define reg_R4  reg_R0+4
#define reg_R5  reg_R0+5
#define reg_R6  reg_R0+6
#define reg_R7  reg_R0+7

#define reg_IP	31
#define reg_TOS reg_IP+1
#define reg_RP0 reg_IP+2
#define reg_RP	reg_IP+3
#define reg_SP0 reg_IP+4
#define reg_SP	reg_IP+5

#define reg_STATE 38

void mon_state( short *m ){
  short i;
  printf( "IP: %6d\tTOS: %6d\n", m[ reg_IP ], m[ reg_TOS ] );
  printf( "RP0: %6d\tRP: %6d\n", m[ reg_RP0 ], m[ reg_RP ] );
  printf( "SP0: %6d\tSP: %6d\n", m[ reg_SP0 ], m[ reg_SP ] );
  printf( "STATE: %2d\tCOLD: %6d\tLAST: %6d\tHERE: %6d\n", m[ reg_STATE ], m[ reg_COLD ], m[ reg_LAST ], m[ reg_HERE ] );
  printf( "W: %6d\tX: %6d\tT: %6d\tH: %6d\n",
	  m[ reg_W ], m[ reg_X ], m[ reg_T ], m[ reg_H ] );
  for( i=0; i<8; i++ ){
    if( i>0 && 0==(i%4) ) printf("\n" );
    printf( "R%d: %6d\t", i, m[reg_R0+i] );
  }
  printf("\nSTACK:\n");
  for( i=m[reg_SP]; i<=m[reg_SP0]; i++ ){
    printf( "%5d\t", m[i] );
  }
  printf("\n");
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
