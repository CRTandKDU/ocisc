( -*- mode: forth; -*-  )
( Load after howe.fs )
( Begin target image )
0 t, 0 t,
tLABEL: entry
-1 t,
( Options byte )
tVAR {options} 0 {options} t!
( Separate system from user vocabulary )
tVAR CONST_PRIMITIVE 0 CONST_PRIMITIVE t!
( Some constants )
tVAR CONST_HBIT 32768 CONST_HBIT t!
tVAR CONST_IMAX 32767 CONST_IMAX t!
tVAR CONST_NEG2 -2 CONST_NEG2 t!
tVAR CONST_NEG1 -1 CONST_NEG1 t!
tVAR CONST_ONE 1 CONST_ONE t!
tVAR CONST_TWO 2 CONST_TWO t!
: tINC  CONST_ONE  t, t, NADDR ;
: tDEC  CONST_NEG1 t, t, NADDR ;
tVAR W 0 W t!
tVAR X 0 X t!
tVAR T 0 T t!
tVAR H 0 H t!
( Jump vector )
tVAR {cold} 0 {cold} t!
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
: ++sp {sp} tDEC ;
: --sp {sp} tINC ;
: ++rp {rp} tINC ;
: --rp {rp} tDEC ;
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
HALT
( End of image )


