( -*- mode: forth; -*-  )
( Load after howe.fs )
( Begin target image )
0 t, 0 t,
tLABEL: entry
-1 t,
( Options byte @3 )
tVAR {options} 2 {options} t!
( Separate system from user vocabulary @4 )
tVAR CONST_PRIMITIVE 0 CONST_PRIMITIVE t!
( Some constants @5 )
tVAR CONST_HBIT 32768 CONST_HBIT t!
tVAR CONST_IMAX 32767 CONST_IMAX t!
tVAR CONST_NEG2 -2 CONST_NEG2 t!
tVAR CONST_NEG1 -1 CONST_NEG1 t!
tVAR CONST_ONE 1 CONST_ONE t!
tVAR CONST_TWO 2 CONST_TWO t!
tVAR CONST_16 16 CONST_16 t!
tVAR INVREG 0 INVREG t!
tVAR W 0 W t!
tVAR X 0 X t!
tVAR T 0 T t!
tVAR R0 0 R0 t!
tVAR R1 0 R1 t!
tVAR R2 0 R2 t!
tVAR R3 0 R3 t!
tVAR R4 0 R4 t!
tVAR R5 0 R5 t!
tVAR R6 0 R6 t!
tVAR R7 0 R7 t!
tVAR H 0 H t!
tVAR bt 0 bt t!
tVAR bl1 0 bl1 t!
tVAR bl2 0 bl2 t!
( Jump vector )
tVAR {cold} 0 {cold} t!
tVAR {last} 0 {last} t!
str" {here} " tsymbol
tVAR {here} 0 {here} t!
( Instruction pointer )
tVAR ip 0 ip t!
( Top of stack )
tVAR tos 0 tos t!
( Return stack pointer )
tVAR {rp0}
tVAR {rp}
=stack-start =stksz + DUP {rp0} t! {rp} t!
( Stack pointer )
tVAR {sp0}
tVAR {sp}
=stack-start =stksz 2* + DUP {sp0} t! {sp} t!
( FORTH system variables )
tVAR {base} 10 {base} t!
tVAR {state} 0 {state} t!
str" >in " tsymbol
tVAR {>in} 0 {>in} t!
str" tib " tsymbol
tVAR {tib} =buf tALLOC {tib} t!
: tDEC  CONST_ONE 2/  t, 2/ t, NADDR ;
: tINC  CONST_NEG1 2/ t, 2/ t, NADDR ;
: tINV  INVREG ZERO DUP INVREG SUB DUP INVREG SWAP MOV tDEC ;
: ++sp {sp} tDEC ;
: --sp {sp} tINC ;
: ++rp {rp} tINC ;
: --rp {rp} tDEC ;
: NG1! DUP ZERO tDEC ;
: ONE! DUP ZERO tINC ;
tVAR CONST_32 32 CONST_32 t!
tVAR CONST_10 10 CONST_10 t!
( VM entry point )
tLABEL: start
start entry 2* t!
{sp0} {sp} MOV
{rp0} {rp} MOV
{cold} ip MOV
( VM Forth inner interpreter )
tLABEL: vm
ip W MOV
ip tINC
T W ILOAD
T W MOV
CONST_PRIMITIVE T SUB
T tIF- W IJMP tTHEN ++rp
ip {rp} ISTORE
W ip MOV
vm JMP
( Additional vocabulary for target FORTH )
( Header for words Name Field Address )
: tNFA tcell + ;
( Header for words Code Field Address )
: tCFA tNFA DUP tc@ 31 and + tcell + tdown ;
( Header for words Building )
: count DUP 1+ SWAP C@ ;
: tpack talign DUP tc, 0 DO count tc, LOOP DROP ;
: tcompile-only {last} t@ tnfa t@ 32 or {last} t@ tnfa t! ;
: timmediate {last}  t@ tnfa t@ 64 or {last} t@ tnfa t! ;
: thead talign there {last} t@ t, {last} t! WORD talign tpack talign ;
: header >in thead in> ;
: :a there CREATE DOCOL , ' LIT , , ' 2/ , ' t, , ' EXIT , ;
: ;a vm JMP ;
( Assembly ops are SUBLEQ code fragments to which )
( image FORTH words are compiled. They are VM low )
( level instructions )
:a opBYE HALT ;a
:a op1- tos tDEC ;a
:a op1+ tos tINC ;a
:a opINVERT tos tINV ;a
:a [@] tos tos ILOAD ;a
:a [!] W {sp} ILOAD W tos ISTORE --sp tos {sp} ILOAD --sp ;a
:a op- W {sp} ILOAD tos W SUB W tos MOV --sp ;a
:a op+ W {sp} ILOAD tos W ADD --sp ;a
:a opR@ ++sp tos {sp} ISTORE tos {rp} ILOAD ;a
:a opRDROP --rp ;a
:a opRP@ ++sp tos {sp} ISTORE {rp} tos MOV ;a
:a oRP! tos {rp} MOV tos {sp} ILOAD --sp ;a
:a opSP@ ++sp tos {sp} ISTORE {sp} tos MOV tos tINC ;a
:a opSP! tos {sp} MOV ;a
:a opEMIT tos PUT tos {sp} ILOAD --sp ;a
:a opKEY ++sp tos {sp} ISTORE tos GET ;a
:a opPUSH ++sp tos {sp} ISTORE tos ip ILOAD ip tINC ;a
:a opSWAP tos W MOV tos {sp} ILOAD w {sp} ISTORE ;a
:a opDUP ++sp tos {sp} ISTORE ;a
:a opOVER W {sp} ILOAD ++sp tos {sp} ISTORE W tos MOV ;a
:a opDROP tos {sp} ILOAD --sp ;a
:a opTOR ++rp tos {rp} ISTORE tos {sp} ILOAD --sp ;a
:a opFROMR ++sp tos {sp} ISTORE tos {rp} ILOAD --rp ;a
str" opEXIT " tsymbol
:a opEXIT ip {rp} ILOAD --rp ;a
( BRANCH group )
:a tNEXT W {rp} ILOAD
W tIF W tDEC W {rp} ISTORE T ip ILOAD T ip MOV ;a
tTHEN ip tINC --rp ;a
:a opJUMP ip ip ILOAD ;a
:a opJUMPZ tos W MOV 0 T MOV
W tIF CONST_NEG1 T MOV tTHEN tos {sp} ILOAD --sp
T tIF ip tINC vm JMP tTHEN W ip ILOAD w ip MOV ;a
( COMPARISON group )
:a op0> tos W MOV 0 tos MOV W tIF+ CONST_NEG1 tos MOV tTHEN ;a
:a op0= tos W MOV CONST_NEG1 tos MOV
W tIF 0 tos MOV tTHEN W tDEC W tIF+ 0 tos MOV tTHEN ;a
:a op0< tos W MOV 0 tos MOV
W tIF- CONST_NEG1 tos MOV tTHEN W tINC W tIF- CONST_NEG1 tos MOV tTHEN ;a
:a op< W {sp} ILOAD --sp tos W SUB 0 tos MOV
W tIF- CONST_NEG1 tos MOV tTHEN ;a
:a op> W {sp} ILOAD --sp tos W SUB 0 tos MOV
W tIF+ CONST_NEG1 tos MOV tTHEN ;a
( Arithmetic )
:a lsb
tos tos ADD tos tos ADD tos tos ADD
tos tos ADD tos tos ADD tos tos ADD
tos tos ADD tos tos ADD tos tos ADD
tos tos ADD tos tos ADD tos tos ADD
tos tos ADD tos tos ADD
tos W MOV 0 tos MOV W tIF CONST_NEG1 tos MOV tTHEN ;a
:a rshift
CONST_16 W MOV
tos W SUB
tos {sp} ILOAD --sp
X ZERO
tBEGIN W tWHILE
X X ADD
tos bt MOV 0 bl1 MOV
bt tIF- CONST_NEG1 bl1 MOV tTHEN bt tINC bt tIF- CONST_NEG1 bl1 MOV tTHEN
bl1 tIF X tINC tTHEN
tos tos ADD
W tDEC
tREPEAT
X tos MOV ;a
:a op2* tos tos ADD ;a
( See also div2.fs )
:a op2/ CONST_16 W MOV X ZERO
tBEGIN W tDEC W tWHILE
    X X ADD
    tos bt MOV 0 bl1 MOV
    bt tIF- CONST_NEG1 bl1 MOV tTHEN bt tINC bt tIF- CONST_NEG1 bl1 MOV tTHEN
    bl1 tIF X tINC tTHEN
    tos tos ADD
tREPEAT
X tos MOV ;a
( OPDIVMOD )
:a opDIVMOD
tos R0 MOV
W {sp} ILOAD
T ZERO
tBEGIN
CONST_ONE X MOV
W tIF- 0 X MOV tTHEN
X tWHILE T tINC tos W SUB tREPEAT
tos W ADD
T tDEC
T tos MOV
W R1 MOV W tIF- R1 R0 ADD tTHEN
R1 {sp} ISTORE
R0 ZERO R1 ZERO ;a
( OPMUX )
str" opMUX " tsymbol
:a opMUX
CONST_16 r0 MOV
r1 ZERO
r3 {sp} iLOAD --sp
r4 {sp} iLOAD --sp
tBEGIN r0 tWHILE
r1 r1 ADD
tos r5 MOV r6 ZERO
r5 tIF- CONST_NEG1 r6 MOV tTHEN
r5 tINC r5 tIF- CONST_NEG1 r6 MOV tTHEN
r6 tIF- 
r4 r7 MOV r5 ZERO
r7 tIF- CONST_ONE r5 MOV tTHEN
r7 tINC r7 tIF- CONST_ONE r5 MOV tTHEN
r1 r5 ADD
tTHEN
r6 tINC
r6 tIF  
r3 r7 MOV r5 ZERO
r7 tIF- CONST_ONE r5 MOV tTHEN
r7 tINC r7 tIF- CONST_ONE r5 MOV tTHEN
r1 r5 ADD
tTHEN
tos tos ADD
r3 r3 ADD
r4 r4 ADD
r0 tDEC
tREPEAT
R1 tos MOV ;a
( Mult special cases x10, x16 )
:a op16x
tos r1 MOV r1 r1 ADD r1 r1 ADD r1 r1 ADD r1 r1 ADD
r1 tos MOV ;a
:a op10x
tos r1 MOV r1 r1 ADD r1 r2 MOV r1 r1 ADD r1 r1 ADD
r1 r2 ADD r1 tos MOV ;a
( Terminal buffer handling and chars )
:a notfigure?
CONST_32 r0 MOV r0 CONST_16 ADD tos r1 MOV
r0 r1 SUB r1 tIF- CONST_NEG1 tos MOV ;a
tTHEN CONST_10 r0 MOV
r0 r1 SUB r1 tIF- r1 r0 ADD r1 TOS MOV ;a
tTHEN CONST_TWO r0 MOV r0 CONST_TWO ADD r0 CONST_TWO ADD
r0 r2 MOV r0 CONST_ONE ADD
r0 r1 SUB r1 tIF- CONST_NEG1 tos MOV ;a
tTHEN r2 r1 SUB r1 tIF- r1 r2 ADD r1 CONST_10 ADD r1 tos MOV ;a
tTHEN CONST_NEG1 tos MOV ;a
( Stack internals )
:a tDEPTH {sp0} r0 MOV {sp} r1 MOV r1 r0 SUB CONST_NEG1 r0 SUB
++sp tos {sp} ISTORE r0 tos MOV ;a
( Barrier )
str" const_primitive" tsymbol
there post2/ CONST_PRIMITIVE t!
( End of primitive ops )
: :t header there 2/ CREATE DOCOL , ' LIT , , ' t, , ' EXIT , ;
: ;t opEXIT there {here} t! ;
: lit opPUSH t, ;
: opMARK opJUMP there 0 t, ;
: opIF opJUMPZ there 0 t, ;
: opTHEN there 2/ SWAP t! ;
: opELSE opMARK SWAP opTHEN ;
: opBEGIN talign there ;
: opUNTIL talign opJUMPZ 2/ t, ;
: opAGAIN talign opJUMP 2/ t, ;
: opWHILE opIF ;
: opREPEAT SWAP opAGAIN opTHEN ;
: opFOR opTOR opBEGIN ;
: opNEXT talign tNEXT 2/ t, ;
( End of image.fs )
