( -*- mode: forth; -*-  )
( Load after howe.fs )
( Begin target image )
0 t, 0 t,
tLABEL: entry
-1 t,
( Options byte @3 )
tVAR {options} 0 {options} t!
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
tVAR H 0 H t!
tVAR bt 0 bt t!
tVAR bl1 0 bl1 t!
tVAR bl2 0 bl2 t!
( Jump vector )
tVAR {cold} 0 {cold} t!
tVAR {last} 0 {last} t!
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
: tDEC  CONST_ONE 2/  t, 2/ t, NADDR ;
: tINC  CONST_NEG1 2/ t, 2/ t, NADDR ;
: tINV  INVREG ZERO DUP INVREG SUB DUP INVREG SWAP MOV tDEC ;
: ++sp {sp} tDEC ;
: --sp {sp} tINC ;
: ++rp {rp} tINC ;
: --rp {rp} tDEC ;
( VM entry point @24 )
tLABEL: start
start entry 2* t!
{sp0} {sp} MOV
{rp0} {rp} MOV
{cold} ip MOV
( VM Forth inner interpreter @60 )
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
: tcompile-only tlast @ tnfa t@ 32 or tlast @ tnfa t! ;
: timmediate tlast  @ tnfa t@ 40 or tlast @ tnfa t! ;
: thead talign there tlast @ t, tlast ! WORD talign tpack talign ;
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
:a opEXIT ip {rp} ILOAD ;a
( BRANCH group )
:a opNEXT W {rp} ILOAD
W tIF W tDEC W {rp} ISTORE T ip ILOAD T ip MOV vm JMP
    tTHEN ip TINC --rp ;a
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
:a opDIVMOD W {sp} ILOAD T ZERO
tBEGIN
CONST_ONE X MOV
W tIF- 0 X MOV tTHEN
X tWHILE T tINC tos W SUB tREPEAT
tos W ADD T tDEC T tos MOV W {sp} ISTORE ;a
( End of primitive ops )
there 2/ CONST_PRIMITIVE t!
( FORTH words in target )
: :t header there 2/ CREATE DOCOL , ' LIT , , ' t, , ' EXIT , ;
: ;t opEXIT ;
( Dictionary )
:t + op+ ;t
there 2/ {cold} t!
opPUSH 2 t,
opPUSH 1000 t,
+
opBYE
HALT
( End of image )
