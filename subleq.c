/* subleq.c - A SUBLEQ machine */
/* Project: SUBLEQ */
/* Date: Sunday, April 9, 2023 */
/* See also: [[https://github.com/howerj/subleq]] */
#include <stdio.h>
#include <unistd.h>

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
  short p=0, m[1<<16], *i = m;
  short a, b, c;
  /* Read in SUBLEQ source */
  FILE *f = fopen( argv[optind], "r" );
  while( fscanf( f, "%hd", i++ ) > 0 );
  /* SUBLEQ interpreter */
  for( ; p>=0; ){
    a = m[p++]; b=m[p++]; c=m[p++];
    a < 0 ? m[b] = getchar() :
      b < 0 ? putchar( m[a] ) :
      ( m[b] -= m[a] ) <= 0 ? p = c :
      0;
  }
  if( 1 == verbose ){
    for( i=m; 0 != *i && 0 != *(i+1) && 0 != *(i+2); i += 3 ){
      printf( "%d\t%d\t%d\n", *i, *(i+1), *(i+2) );
    }
  }
  return 0;
}
