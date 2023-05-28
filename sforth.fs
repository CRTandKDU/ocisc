( -*- mode: forth; -*-  )
( Load after image.fs howe.fs )
( Target FORTH system baed on Jones' FORTH )
( STACK )
:t drop opDROP ;t
str" swap " tsymbol
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
:t 0= op0= ;t
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
( MEMORY ALLOCATION, HERE, STRINGS and HEADERS )
:t aligned opDUP lsb 0<> 1 lit and + ;t
:t align {here} lit @ aligned {here} lit ! ;t
:t allot {here} lit @ opSWAP {here} lit +! ;t
:t , align {here} lit @ ! 2 lit {here} lit +! ;t
:t here {here} lit @ ;t
( Literal string: online copy )
:t s" talign {tib} lit @ {>in} lit @ op+ opBEGIN
    opDUP c@ op0= opIF
    opDROP here 0 lit 2 lit allot ! opEXIT opTHEN
    opDUP c@ 32 lit op- 0<> opIF
        here 1 lit 1 lit allot c! opSWAP
        opBEGIN
	    opDUP c@ 1 lit allot c!
	    op1+ opDUP c@ opDUP op0= opSWAP
	    34 lit op- op0= or 
        opUNTIL
        opDROP here opOVER op- op1- opTOR opR@ opOVER c!
        opFROMR op1+ op1+ {>in} lit +!
        opEXIT
    opTHEN op1+
opAGAIN ;t
( OUTPUT )
:t cr 10 lit opEMIT ;t
:t space 32 lit opEMIT ;t
( NUMERIC OUTPUT )
:t digit 9 lit opOVER op< opIF 7 lit op+ opTHEN 48 lit op+ ;t
:t [.] abs {base} lit @ opDIVMOD ?dup opIF [.] opTHEN digit opEMIT ;t
:t . opDUP op0< opIF 45 lit opEMIT opTHEN [.] ;t
( STATE WORDS )
:t ] 0 lit {state} lit ! ;t
str" [ " tsymbol
:t [ -1 lit {state} lit ! ;t timmediate
( STRINGS )
:t type opDUP c@ 31 lit and opBEGIN opDUP opWHILE opSWAP op1+ opDUP c@ opEMIT
opSWAP op1- opREPEAT opDROP opDROP ;t
( Some IO )
talign tLABEL: $banner ts" SUBTLE FORTH - v1.0"
talign tLABEL: $prompt ts" OK> "
talign tLABEL: $SNerror ts" ?SN ERROR"
:t .s tDEPTH op1- opFOR {sp} lit @ opR@ op+ [@] . space opNEXT space ;t
:t banner $banner lit op2* type cr ;t
:t prompt {options} lit @ 2 lit and opIF .s opTHEN
$prompt lit op2* type ;t
:t words {last} lit @ opBEGIN opDUP opWHILE
opDUP op1+ op1+ type 32 lit opEMIT @ opREPEAT opDROP ;t
( INPUT bot eot cur c OUTPUT bot eot cur )
:t tap {options} lit @ 1 lit and 0<>
opIF opDUP opEMIT
opTHEN opOVER c! op1+ ;t
:t ktap opDUP opDUP 13 lit <> opTOR
10 lit <> opFROMR and opIF
opDUP 8 lit <> opTOR 127 lit <> opFROMR and opIF
32 lit tap opEXIT
opTHEN opTOR opOVER opR@ op< opDUP opIF
8 lit opDUP opEMIT 32 lit opEMIT opEMIT
opTHEN opFROMR + opEXIT
opTHEN opDROP nip opDUP ;t
( INPUT a line to {tib} )
:t accept {tib} lit @ =buf lit
opOVER + opOVER opBEGIN
2dup <> opWHILE
opKEY opDUP 32 lit op< opIF
ktap opELSE opDUP 127 lit op< opIF
tap opELSE ktap
opTHEN opTHEN 
opREPEAT opDROP
opDUP 0 lit opSWAP c!
opOVER op- nip
0 lit {>in} lit ! ;t
( Parse a single word ending in 32 or 0 )
( Return beg last+1 on stack )
:t word opBEGIN
{tib} lit @ {>in} lit @ op+ c@
op0= opIF {>in} lit @ opDUP opEXIT opTHEN
{tib} lit @ {>in} lit @ op+ c@ 32 lit op-
0<> opIF
    {>in} lit @
    opBEGIN
    {tib} lit @ {>in} lit @ op+ c@
    op0= opIF {>in} lit @ opEXIT opTHEN
    {tib} lit @ {>in} lit @ op+ c@ 32 lit op-
    op0= opIF {>in} lit @ opEXIT opTHEN
    {>in} lit opDUP @ op1+ opSWAP !
    opAGAIN
opTHEN
{>in} lit opDUP @ op1+ opSWAP !
opAGAIN ;t
( Parse a number: beg lst+1 -- sgn num rest )
( where sgn is 0 for positive, -1 for negative )
( num is absolute value )
( rest is the number of unparsed chars )
:t number? opOVER op- op1- opSWAP opOVER op+ 0 lit rot rot opSWAP
opTOR opDUP {tib} lit @ op+ opR@ op- c@ 45 lit =
opIF -1 lit rot rot op1+
opELSE 0 lit rot rot
opTHEN opFROMR
opFOR
    opDUP {tib} lit @ op+ opR@ op- c@
    notfigure? opDUP -1 lit = opIF opDROP opDROP opFROMR op1+ opEXIT
    opTHEN rot {base} lit @ 10 lit = opIF op10x opELSE op16x opTHEN op+
    opSWAP
opNEXT opDROP 0 lit ;t
( Signed parse beg lst+1 -- signednum rest )
:t number number? opTOR opSWAP opDUP op0= opIF
opELSE opSWAP 0 lit opSWAP op- opSWAP opTHEN
opFROMR op+ ;t
( Dictionary lookup: beg lst+1 )
( Compare word to tib: tib+beg len addr -- tib+beg len addr [0|-1] )
:t compare
( 32 lit opEMIT opDUP opDUP [.] 32 lit opEMIT op1+ op1+ type )
opOVER opFOR
    opOVER opR@ op-
    2dup op+ op1+ op1+ op1+ c@ opTOR ( R@ is char in word )
    opTOR rot opDUP opFROMR + c@ ( char from tib )
    opFROMR op- 0<> opIF -rot 0 lit opFROMR opDROP opEXIT
    opTHEN -rot
opNEXT
-1 lit ;t
( Find word in dictionary beg lst+1 -- tib+beg len [addr|0] )
:t find opOVER op- op1- opSWAP {tib} lit @ op+ opSWAP
{last} lit @ opBEGIN opDUP opWHILE
opDUP op1+ op1+ c@ 31 lit and opTOR opOVER op1+ opFROMR = opIF
compare 0<> opIF opEXIT opTHEN
opTHEN @
opREPEAT ;t
( OUTER INTERPRETER )
:t ' word find nip nip ;t
:t cfa op1+ op1+ opDUP c@ 31 lit and +
opDUP 1 lit and 0= opIF op1+ opTHEN op1+ ;t
( Outer interpreter: beg lst+1 --   )
:t interpret 2dup find opDUP 0= opIF
    opDROP opDROP opDROP number 0<> opIF
	$SNerror lit op2* type cr ( Then what? )
    opTHEN ( a number )
	{state} lit @ op0= opIF opEXIT opTHEN
        975 lit , , opEXIT
opTHEN
{state} lit @ op0= opIF
cfa op2/ opTOR 2drop 2drop opEXIT
opTHEN
opDUP op1+ op1+ c@ 64 lit and op0= opIF
cfa op2/ ,
opELSE
cfa op2/ opTOR
opTHEN
2drop 2drop 
;t
( Misc )
:t C, {here} lit @ C! 1 lit {here} lit +! ;t
:t bye opBYE ;t
:t quit opBEGIN
prompt
accept 0<> opIF
opBEGIN word 2dup op- 0<> opWHILE
interpret
opREPEAT 2drop
opTHEN opAGAIN ;t
:t create {here} lit @ {last} lit @ 2 lit allot ! word
opOVER op- op1- opSWAP {tib} lit @ op+ opOVER op+ opSWAP
opDUP op1+ opSWAP opFOR
C, opDUP opR@ op- C@
opNEXT C, opDROP {last} lit ! align ;t
:t : create [ ;t
:t ; ] 1479 lit , ;t timmediate
( COLD is here )
there post2/ {cold} t!
banner
quit
opBYE
there {here} t!
timage
