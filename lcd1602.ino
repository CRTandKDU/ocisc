// LCD1602 to Arduino Uno connection example

#include <LiquidCrystal.h>

LiquidCrystal lcd(12, 11, 10, 9, 8, 7);

// SUBLEQ VM
int16_t M[100];
int16_t PC;

#define RUNNING 1
#define HALTED 0
int16_t SL_STATE = RUNNING;

#define SL_HALT -1
#define SL_OUT -2
#define SL_IN  -3


void subleq_initfill(){
  short i = 0;
  M[i++] = 3;  M[i++] = 4;  M[i++] = 6;
  M[i++] = 7;  M[i++] = 7;  M[i++] = 7;
  M[i++] = 3;  M[i++] = 4;  M[i++] = 9;
  M[i++] = -2;  M[i++] = 72;  M[i++] = 12;
  M[i++] = -2;  M[i++] = 105;  M[i++] = 15;
  M[i++] = -1;  M[i++] = 0;  M[i++] = 0;
  // Program counter
  PC = 0;
}

void subleq_dump(){
  short i;
  for( i=0; i< 30; i += 3 ){
    Serial.print( M[i] );
    Serial.print(" ");
    Serial.print( M[i+1] );
    Serial.print(" ");
    Serial.print( M[i+2] );
    Serial.println(" ");
  }
}

void subleq_put(){
  char c = M[PC+1] & 0xff;
  lcd.print(c);
  PC += 3;
}

void subleq_get(){
  PC += 3;
}

int16_t subleq_step(){
  int16_t A, B, C;
  if( SL_HALT == M[PC] ){
    subleq_dump();
    return HALTED;
  }
  if( SL_OUT == M[PC] ){
    subleq_put();
    return RUNNING;
  }
  if( SL_IN == M[PC] ){
    subleq_get();
    return RUNNING;
  }
  // SUBLEQ A B C
  A = M[PC]; B = M[PC+1]; C = M[PC+2];
  M[B] = M[B] - M[A];
  if( M[B] <= 0 ){
    PC = C;
  }
  else{
    PC += 3;
  }
  return RUNNING;
}

void setup() {
  Serial.begin(115200);
  lcd.begin(16, 2);
  // you can now interact with the LCD, e.g.:
  lcd.print("Hello World!");
  lcd.setCursor(0, 1);
  subleq_initfill();
}

void loop() {
  // ...
  if( RUNNING == SL_STATE ) SL_STATE = subleq_step();
  delay( 10 );
}