/* sl_instrument.c - A SUBLEQ machine instrumentation for debugging */
/* Project: SUBLEQ */
/* Date: Friday, June 16, 2023 */
/* See also: [[https://github.com/howerj/subleq]] */
#include <stdio.h>
#include <errno.h>
#include "sl_instrument.h"

#define RAM (1<<16)

int heatmap[RAM];
int clks;

void instrument_init(){
  int i;
  for( i=0; i < RAM; heatmap[i++]=0 );
  clks = 0L;
  printf( "Instrument: Inited\n" );
}

void instrument_datapoint( short p ){
  clks += 1;
  if( p >= 0) heatmap[p] += 1;
  if( p+1 >= 0) heatmap[p+1] += 1;
  if( p+2 >= 0) heatmap[p+2] += 1;
}

void instrument_output( const char *fn ){
  printf( "\nInstrument:\t Clock T: %9d\n", clks );
  FILE *f = fopen( fn, "w" );
  char buf[ 32 ];
  int i;
  if( NULL == f ){
    fprintf( stderr, "Error %d in fopen.\n", errno );
    return;
  }
  for( i=0; i< (1<<16); i++ ){
    sprintf( buf, "%9d, %9d\n", i, heatmap[i] );
    fputs( buf, f );
  }
  fclose( f );
}



