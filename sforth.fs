( -*- mode: forth; -*-  )
( Load after image.fs howe.fs )
( Target FORTH system baed on Jones' FORTH )
( STACK )
:t drop opDROP ;t
:t swap opSWAP ;t
:t dup opDUP ;t
:t over opOVER ;t
:t rot opTOR opSWAP opFROMR opSWAP ;t
:t -rot rot rot ;t
:t 2drop opDROP opDROP ;t
:t 2dup opOVER opOVER ;t
( ARITHMETIC )
:t 1+ op1+ ;t
:t 1- op1- ;t
:t + op+ ;t
:t - op- ;t
( Missing multiplication here )
:t /mod opDIVMOD ;t
:t 2/ op2/ ;t
:t 2* op2* ;t
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
( BIT MANIP )
:t invert opINVERT ;t
:t or opOVER opMUX ;t
:t and 0 lit opSWAP opMUX ;t
( FORTH CODE WORDS )
:t ! op2/ [!] ;t
:t @ op2/ [@] ;t
( COLD is here )
there post2/ {cold} t!
2 lit 2 lit +
opBYE
HALT

