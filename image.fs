( -*- mode: forth; -*-  )
( Begin target image )
0 t, 0 t,
tLABEL: entry
-1 t,
( Options byte )
tVAR {options} 0 {options} t!
( Separate system from user vocabulary )
tVAR CTE_PRIMITIVE 0 CTE_PRIMITIVE t!
( Some constants )
tVAR CTE_HBIT 32768 CTE_HBIT t!
tVAR CTE_IMAX 32767 CTE_IMAX t!
tVAR CTE_NEG2 -2 CTE_NEG2 t!
tVAR CTE_NEG1 -1 CTE_NEG1 t!
tVAR CTE_ONE 1 CTE_ONE t!
tVAR CTE_TWO 2 CTE_TWO t!
: tINC  CTE_ONE  t, t, NADDR ;
: tDEC  CTE_NEG1 t, t, NADDR ;
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
halt

