( -*- mode: forth; -*-  )
( SUBLEQ image constants )
2 CONSTANT =cell
4000 CONSTANT size
100 CONSTANT =buf
100 CONSTANT =stksz
( String delimiter in target image )
-2 CONSTANT $STRING
( Globales controlling SUBLEQ code generation )
VARIABLE VERBOSITY 0 VERBOSITY !
( Image memory allocation )
size CELLS ALLOC CONSTANT tflash
VARIABLE tdp 0 tdp !
( Target building words )
: tcell 2 ;
: there tdp @ ;
: tc! tflash + c! ; : tc@ tflash + c@ ;
: t! tflash + !  ; : t@ tflash + @ ;
: t, there t! tcell tdp +! ; : tc, there tc! 1 tdp +! ;
: talign there 1 AND tdp +! ;
: ts" there BEGIN KEY DUP 34 <> WHILE OVER tc! 1+ REPEAT DROP DUP 1 AND + tdp !  ;
( Tentative SUBLEQ assembler instruction set )
: Z 0 t, ;
: NADDR there 2/ 1+ t, ;
: HALT Z Z -1 t, ;
: NOOP Z Z NADDR ;
: ZERO t, t, NADDR ;
: JMP Z Z t, ;
: ADD t, Z NADDR Z t, NADDR Z Z NADDR ;
: SUB SWAP t, t, NADDR ;
: PUT t, -1 t, NADDR ;
: GET -1 t, t, NADDR ;
: MOV >R R@ DUP t, t, NADDR t, Z NADDR R> Z t, NADDR Z Z NADDR ;
: ILOAD there 2/ 3 4 * 3 + + MOV 0 SWAP MOV ;
: IJMP there 2/ 14 + MOV Z Z NADDR ;
: ISTORE SWAP >R there 2/ 36 + 2DUP MOV 2DUP 1+ MOV 7 + MOV R> 0 MOV ;
( Tentative SUBLEQ assembler macros )
( Alloc in target )
: talloc tdp @ SWAP tdp +! ;
( Variable in target )
: tvar tcell talloc CREATE DOCOL , ' LIT , , ' EXIT , ;
( Label in target. If used in SUBLEQ instr has to be halved )
: tlabel: there 2/ CREATE DOCOL , ' LIT , , ' EXIT , ; 
( Debugging help )
: t. . CR ;
: tdump 0 BEGIN DUP t@ t. 2+ DUP there - 0= UNTIL DROP ;
: tmem 0 BEGIN DUP t@ . 9 EMIT DUP tc@ DUP . SPACE EMIT 9 EMIT 1+ DUP tc@ DUP . SPACE EMIT CR 1+ DUP there - 0= UNTIL DROP ;
: trace IF s" out.slq" FOPEN ELSE FCLOSE THEN ;
: timage 1 trace tdump 0 trace ;
