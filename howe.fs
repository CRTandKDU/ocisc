( -*- mode: forth; -*-  )
( SUBLEQ image constants )
VARIABLE voc.root
2	CONSTANT =cell
16384	CONSTANT size
256	CONSTANT =buf
256	CONSTANT =stksz
62514	CONSTANT =stack-start
( String delimiter in target image )
-2 CONSTANT $STRING
( Globales controlling SUBLEQ code generation )
VARIABLE VERBOSITY 0 VERBOSITY !
( Image memory allocation )
size CELLS ALLOC CONSTANT tflash
VARIABLE tdp 0 tdp !
VARIABLE tlast 0 tlast !
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
: ZERO DUP 2/ t, 2/ t, NADDR ;
: JMP Z Z t, ;
: ADD 2/ t, Z NADDR Z 2/ t, NADDR Z Z NADDR ;
: SUB SWAP 2/ t, 2/ t, NADDR ;
: PUT 2/ t, -1 t, NADDR ;
: GET 2/ -1 t, t, NADDR ;
: MOV 2/ >R R@ DUP t, t, NADDR 2/ t, Z NADDR R> Z t, NADDR Z Z NADDR ;
: ILOAD there 2/ 3 4 * 3 + + 2* MOV 0 SWAP MOV ;
: IJMP there 2/ 14 + 2* MOV Z Z NADDR ;
: ISTORE SWAP >R there 2/ 36 + 2DUP 2* MOV 2DUP 1+ 2* MOV 7 + 2* MOV R> 0 MOV ;
( Tentative SUBLEQ assembler macros )
( Alloc in target )
: tALLOC tdp @ SWAP tdp +! ;
( Variable in target )
: tVAR tcell tALLOC CREATE DOCOL , ' LIT , , ' EXIT , ;
( Label in target. If used in SUBLEQ instr has to be halved )
: tLABEL: there 2/ CREATE DOCOL , ' LIT , , ' EXIT , ;
( Creates an empty location in image )
: tmark there 0 t, ;
( Align cell downwards )
: tdown tcell negate and ;
( Conditional and loop macros )
: tBEGIN talign there ;
: tAGAIN JMP ;
: tIF 2/ DUP t, Z there 2/ 4 + DUP t, Z Z 6 + t, Z Z NADDR Z t, tmark ;
: tIF- 2/ t, Z there 2/ 4 + t, Z Z there 2/ 4 + t, Z Z tmark ;
: tIF+ Z 2/ t, tmark ;
: tTHEN tBEGIN 2/ SWAP t! ;
: tWHILE tIF SWAP ;
: tREPEAT 2/ JMP tTHEN ;
: tUNTIL 2/ DUP t, Z there 2/ 4 + DUP t, Z Z 6 + t, Z Z NADDR Z t, 2/ t, ;
( Debugging help )
: tdump 0 BEGIN DUP t@ t. 2+ DUP there - 0>= UNTIL DROP ;
: tmem. DUP . 9 EMIT ;
: tmem@. DUP t@ . 9 EMIT ;
: tmemC@. DUP tc@ DUP 31 SWAP > IF DUP . THEN SPACE EMIT 9 EMIT ;
: tmem 0 BEGIN tmem. tmem@. tmemC@. 1+ tmemC@. CR 1+ DUP there - 0>= UNTIL DROP ;
: trace IF s" out.slq" FOPEN ELSE FCLOSE THEN ;
: timage 1 trace STR" v2.0 raw" type CR tdump 0 trace ;
: tsymbol VERBOSITY @ IF there 2/ . TYPE CR ELSE DROP DROP THEN ;
1 VERBOSITY !
