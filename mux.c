#include <stdio.h>
#include <stdint.h>
#include <stdbool.h>
#include <stdlib.h>

int16_t r0, r1, r2, r3, r4, r5, r6, r7 ;

void trace_step(){
  printf( "R0: %5d\tR1: %5d\tR6: %5d\n", r0, r1, r6 );
  printf( "R3: %5d\tR4: %5d\tR5: %5d\tR7: %5d\n\n", r3, r4, r5, r7 );
}

int16_t combine(int16_t x1, int16_t x2, int16_t mask) {
  r1 = 0;
  r0 = 16;
  r3 = x2;
  r4 = x1;
  while( r0 ){
    trace_step();
    r1 += r1;
    r5 = mask; r6 = 0;
    if( r5<0 ) r6 = -1;
    r5 += 1; if( r5<0 ) r6 = -1;
    if( r6<0 ){
      r7 = r4; r5 = 0;
      if( r7<0 ) r5 = 1;
      r7 += 1; if( r7<0 ) r5 = 1;
      r1 += r5;
    }
    r6 += 1;
    if( r6 != 0 ){
      r7 = r3; r5 = 0;
      if( r7<0 ) r5 = 1;
      r7 += 1; if( r7<0 ) r5 = 1;
      r1 += r5;
    }
    mask += mask;
    r3 += r3;
    r4 += r4;
    r0 -= 1;
  }
  trace_step();
  return r1;
}


/* uint16_t combine(uint16_t x1, uint16_t x2, uint16_t mask) { */
/*     uint16_t result = 0; */
/*     for (int i = 0; i < 16; i++) { */
/*         uint8_t msb = (x1 >> i) & 1; */
/*         uint8_t lsb = (x2 >> i) & 1; */
/*         uint8_t msk = (mask >> i) & 1; */

/*         if (msb == 1 && msk == 1) { */
/*             result |= (1 << i); */
/*         } else if (lsb == 1 && msk == 0) { */
/*             result |= (1 << i); */
/*         } */
/*     } */
/*     return result; */
/* } */

/* uint16_t combine(uint16_t x1, uint16_t x2, uint16_t mask) { */
/*     uint16_t result = 0; */
/*     for (int i=0; i<16; ++i) { */
/*         bool bit_match = false; */
/*         int temp1 = x1 >> i; */
/*         int temp2 = x2 >> i; */
        
/*         if (temp1 && (temp2 & mask)) { */
/*             bit_match = true; */
/*         } */
        
/*         result <<= 1; */
/*         if (bit_match) { */
/*             result |= 1; */
/*         } */
/*     } */
/*     return result; */
/* } */

int main( int argc, char *argv[] ) {
  // Example usage
  int16_t x1 = atoi( argv[1] );
  int16_t x2 = atoi( argv[2] );
  int16_t mask = atoi( argv[3] );
  
  int16_t combined = combine(x1, x2, mask);
  printf("Combination: %d\t0x%X\n", combined, combined);   // Output: combination: FD
  
  return 0;
}
