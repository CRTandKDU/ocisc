( -*- mode: forth; -*-  )
( Load after image.fs howe.fs )
( Target FORTH system baed on Jones' FORTH )
( STACK )
:t drop opDROP ;t
:t swap opSWAP ;t
:t dup opDUP ;t
:t ?dup opDUP opIF opDUP opTHEN ;t
:t over opOVER ;t
:t rot opTOR opSWAP opFROMR opSWAP ;t
:t -rot rot rot ;t
:t 2drop opDROP opDROP ;t
:t 2dup opOVER opOVER ;t
:t nip opSWAP opDROP ;t
:t tuck opSWAP opOVER ;t
:t >r opTOR ;t
:t r> opFROMR ;t
( ARITHMETIC )
:t 1+ op1+ ;t
:t 1- op1- ;t
:t + op+ ;t
:t - op- ;t
( Missing multiplication here )
:t /mod opDIVMOD ;t
:t 2/ op2/ ;t
:t 2* op2* ;t
:t abs opDUP op0< opIF 0 lit opSWAP op- opTHEN ;t 
( COMPARISON )
:t = op- op0= ;t
:t <> = op0= ;t
:t > op> ;t
:t < op< ;t
:t >= op< op0= ;t
:t <= op> op0= ;t
:t 0= op0= ;t
:t 0<> op0= op0= ;t
:t 0> op0> ;t
:t 0< op0< ;t
:t 0>= op0< op0= ;t
:t 0<= op0> op0= ;t
:t min 2dup op> opIF nip opELSE drop opTHEN ;t
:t max 2dup op< opIF nip opELSE drop opTHEN ;t
( BIT MANIP )
:t invert opINVERT ;t
:t or opOVER opMUX ;t
:t and 0 lit opSWAP opMUX ;t
:t xor opTOR opDUP opINVERT opSWAP opFROMR opMUX ;t
( LOAD/STORE WORDS )
:t ! op2/ [!] ;t
:t @ op2/ [@] ;t
:t +! 2/ tuck [@] op+ opSWAP [!] ;t
:t lshift opBEGIN ?dup opWHILE op1- opSWAP op2* opSWAP opREPEAT ;t 
:t c@ opDUP @ opSWAP lsb opIF 8 lit rshift opELSE 255 lit and opTHEN ;t
:t c! opSWAP 255 lit and opDUP 8 lit lshift or opSWAP
tuck opDUP @ opSWAP lsb op0= 255 lit xor
opTOR opOVER xor opFROMR and xor opSWAP ! ;t
:t aligned opDUP lsb 0<> 1 lit and + ;t
:t align {here} lit @ aligned {here} lit ! ;t
:t allot {here} lit +! ;t
:t , align {here} lit ! 2 lit allot ;t 
( OUTPUT )
:t cr 10 lit opEMIT ;t
:t space 32 lit opEMIT ;t
( NUMERIC OUTPUT )
:t digit 9 lit opOVER op< opIF 7 lit op+ opTHEN 48 lit op+ ;t
:t [.] abs {base} lit @ opDIVMOD ?dup opIF [.] opTHEN digit opEMIT ;t
( STATE WORDS )
:t ] -1 lit {state} lit ! ;t
:t [ 0 lit {state} lit ! ;t timmediate
( STRINGS )
:t type opDUP c@ 31 lit and opBEGIN opDUP opWHILE opSWAP op1+ opDUP c@ opEMIT
opSWAP op1- opREPEAT opDROP opDROP ;t
( Some IO )
talign tLABEL: $banner ts" SUBTLE FORTH - v1.0"
talign tLABEL: $prompt ts" OK> "
:t banner $banner lit op2* type cr ;t
:t prompt $prompt lit op2* type ;t
:t words {last} lit @ opBEGIN opDUP opWHILE
opDUP op1+ op1+ type 32 lit opEMIT @ opREPEAT opDROP ;t
( INPUT bot eot cur c OUTPUT bot eot cur )
:t tap opDUP opEMIT opOVER c! op1+ ;t
:t ktap opDUP opDUP 13 lit <> opTOR
10 lit <> opFROMR and opIF
opDUP 8 lit <> opTOR 127 lit <> opFROMR and opIF
32 lit tap opEXIT
opTHEN opTOR opOVER opR@ op< opDUP opIF
8 lit opDUP opEMIT 32 lit opEMIT opEMIT
opTHEN opFROMR + opEXIT
opTHEN opDROP nip opDUP ;t
( INPUT a line to {tib} )
:t accept {tib} lit @ =buf lit
opOVER + opOVER opBEGIN
2dup <> opWHILE
opKEY opDUP 32 lit op< opIF
ktap opELSE opDUP 127 lit op< opIF
tap opELSE ktap
opTHEN opTHEN 
opREPEAT opDROP
opDUP 0 lit opSWAP c!
opOVER op- nip
0 lit {>in} lit ! ;t
( Parse a single word ending in 32 or 0 )
( Return beg last+1 on stack )
:t word opBEGIN
{tib} lit @ {>in} lit @ op+ c@
op0= opIF {>in} lit @ opDUP opEXIT opTHEN
{tib} lit @ {>in} lit @ op+ c@ 32 lit op-
0<> opIF
    {>in} lit @
    opBEGIN
    {tib} lit @ {>in} lit @ op+ c@
    op0= opIF {>in} lit @ opEXIT opTHEN
    {tib} lit @ {>in} lit @ op+ c@ 32 lit op-
    op0= opIF {>in} lit @ opEXIT opTHEN
    {>in} lit opDUP @ op1+ opSWAP !
    opAGAIN
opTHEN
{>in} lit opDUP @ op1+ opSWAP !
opAGAIN ;t
( Parse a number <last+1> <beg> <straddr> )
( Hence WORD SWAP <test> NUMBER )
:t number opOVER op- op1- opSWAP opOVER op+ 0 lit rot rot opSWAP
opFOR
opDUP {tib} lit @ op+ opR@ op- c@
opEMIT 10 lit opEMIT
opNEXT
;t
( COLD is here )
there post2/ {cold} t!
banner
prompt
accept
word
10 lit opEMIT
number
opBYE
HALT
timage bye
