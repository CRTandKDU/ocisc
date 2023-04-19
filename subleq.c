/* subleq.c - A SUBLEQ machine */
/* Project: SUBLEQ */
/* Date: Sunday, April 9, 2023 */
/* See also: [[https://github.com/howerj/subleq]] */
#include <stdio.h>
#include <unistd.h>
#include <errno.h>

#define NEG_USED_FOR_IO
#define MAX_SOURCE_LINE_LENGTH 64

void dump_mem( short *m ){
  short *i;
  for( i=m; ; i++ ){
    printf( "%d\n", *i );
    if( (i-m)>2 ){
      if( 0 == *(i-2) && 0 == *(i-1) && -1 == *i ) break;
    }
  }
}

int read_file( char *fn, short *m ){
  short *i = m;
  char buf[MAX_SOURCE_LINE_LENGTH];
  FILE *f = fopen( fn, "r" );
  int nums_now;
  int bytes_now;
  int bytes_consumed = 0, nums_read = 0;
  if( NULL == f ){
    fprintf( stderr, "Error %d in fopen.\n", errno );
    return errno;
  }
  (void) fgets( buf, MAX_SOURCE_LINE_LENGTH, f );
  while( !feof(f) ){
    /* printf( "READ> %s\n", buf ); */
    /* Comments begin with `(' at the beginning of a line, extend to EOL */
    if( '(' == buf[0] ) goto skip;
    bytes_consumed = 0; nums_read = 0;
    while( (nums_now =
	    sscanf( buf + bytes_consumed, "%hd%n", i++, &bytes_now )) > 0 ){
      bytes_consumed += bytes_now;
      nums_read += nums_now;
      /* printf( "\tFIND> %d=%d\n", bytes_now, *(i-1) ); */
    }
    i--;

  skip:
      (void) fgets( buf, 64, f );
  }
  /* if( 1 == verbose ) dump_mem( m ); */
  return 0;
}

int main( int argc, char **argv ){
  int opt	= 0;
  short verbose = 0;
  /* Handle command line */
  opterr = 0;
  while((opt = getopt(argc, argv, "v")) != -1){
    switch( opt ){
    case 'v':
      /* Use subleq -v <.slq source file> to dump memory on exit */
      verbose = 1;
      break;
    default:
      abort();
    }
  }
	
  /* short is 2 bytes long (16b)   */
  short p=0, m[1<<16];
  short a, b, c;

  /* Read in SUBLEQ source */
  if( read_file( argv[optind], m ) ) abort();

  /* SUBLEQ interpreter */
#ifdef NEG_USED_FOR_IO
  for( ; p>=0; ){
    a = m[p++]; b=m[p++]; c=m[p++];
    a < 0 ? m[b] = getchar() :
      b < 0 ? putchar( m[a] ) :
      ( m[b] -= m[a] ) <= 0 ? p = c :
      0;
  }
#else
  for( ; p>=0; ){
    a = m[p++]; b=m[p++]; c=m[p++];
    ( m[b] -= m[a] ) <= 0 ? p = c : 0;
  }
#endif
  if( 1 == verbose ) dump_mem( m );
  return 0;
}
