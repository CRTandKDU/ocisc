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
:t banner $banner lit op2* type 10 lit opEMIT ;t
:t words {last} lit @ opBEGIN opDUP opWHILE
opDUP op1+ op1+ type 32 lit opEMIT @ opREPEAT opDROP ;t
( COLD is here )
there post2/ {cold} t!
banner
words
opBYE
HALT
timage
