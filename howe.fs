( SUBLEQ image constants )
2 CONSTANT =cell
4000 CONSTANT size
100 CONSTANT =buf
100 CONSTANT =stksz
( Image memory allocation )
size CELLS ALLOC CONSTANT tflash
VARIABLE tdp 0 tdp !
( Target building words )
: tcell 2 ;
: there tdp @ ;
: tc! tflash + c! ; : tc@ tflash + c@ ;
: t! tflash + !  ; : t@ tflash + @ ;
: t, there t! tcell tdp +! ;
( Tentative SUBLEQ assembler )
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
: ISTORE SWAP >R there 2/ 24 + 2DUP MOV 2dup 1+ MOV 7 + MOV R> 0 MOV ;
( Debugging help )
: tdump 0 BEGIN dup t@ . CR 2+ dup there - 0= UNTIL drop ;
: trace IF s" out.slq" FOPEN ELSE FCLOSE THEN ;
: timage 1 trace tdump 0 trace ;
