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
tVAR INVREG 0 INVREG t!
tVAR W 0 W t!
tVAR X 0 X t!
tVAR T 0 T t!
tVAR H 0 H t!
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
: header thead ;
: :a header ;
: ;a vm JMP ;
( Assembly primitive words Root vocabulary )
( bye CFA @414 )
:a bye HALT ;a
:a 1- tos tDEC ;a
:a 1+ tos tINC ;a
:a invert tos tINV ;a
:a [@] tos tos ILOAD ;a
:a [!] W {sp} ILOAD W tos ISTORE --sp tos {sp} ILOAD --sp ;a
( opEMIT CFA @806 )
:a opEMIT tos PUT tos {sp} ILOAD --sp ;a
:a opKEY ++sp tos {sp} ISTORE tos GET ;a
( opPUSH CFA @1004 )
:a opPUSH ++sp tos {sp} ISTORE tos ip ILOAD ip tINC ;a
there 2/ CONST_PRIMITIVE t!
there 2/ {cold} t!
502 t,
72 t,
403 t,
207 t,
HALT
( End of image )
