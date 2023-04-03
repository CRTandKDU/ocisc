import argparse
from tkinter import *
from tkinter import ttk
import numpy as np
import csv

global M, MEMSIZE, PC

# By convention we use negative addresses as pseudo-instructions
HALT = -1
OUT  = -2
IN   = -3

def trace_mem():
    global M, MEMSIZE, PC
    print( "MEM:" )
    for i in range( 0, 12, 3 ):
        print( "%2d: %6d %6d %6d" % (i, M[i], M[i+1], M[i+2] ) )

        
def ocisc_io_input( addr, inputvar ):
    global M, MEMSIZE, PC
    s = inputvar.get()
    if 0 < len(s):
        M[addr] = ord(s[0])
        inputvar.set( s[1:] )



def ocisc_io_output( c, outputvar ):
    outputvar.set( outputvar.get() + chr(c) )


def ocisc_gui_continue( memmapvar, mem, inputvar, outputvar  ):
    global M, MEMSIZE, PC
    while HALT != M[PC]:
        ocisc_gui_step( memmapvar, mem, inputvar, outputvar )

        
def ocisc_gui_step( memmapvar, mem, inputvar, outputvar ):
    global M, MEMSIZE, PC
    if HALT == M[PC] :
        pass
    else:
        B = ocisc_vm_step( verbose=False,
                           inputvar=inputvar, outputvar=outputvar )
        memmapvar.set( ocisc_gui_memmap() )
        mem.selection_clear(0, "end")
        mem.selection_set(PC // 3);
        # When returning from a SUBLEQ highlight the modified cell line
        if B > 0:
            mem.itemconfig( B // 3, { 'bg': '#fa9d66' } )

        
def ocisc_gui_memmap():
    global M, MEMSIZE, PC
    memmap = []
    for i in range( 0, MEMSIZE, 3 ):
        memmap += [ "--> %03d %5d %5d %5d" % (i, M[i], M[i+1], M[i+2])
                    if PC == i
                    else "    %03d %5d %5d %5d" % (i, M[i], M[i+1], M[i+2])
                   ]
    return memmap


def ocisc_vm_step( inputvar, outputvar, verbose=True ):
    global M, MEMSIZE, PC

    A, B, C = M[ PC ], M[ PC+1 ], M[ PC+2 ]
    # Pseudo-instructions
    if HALT == A:
        # Don't change PC
        return A
    if OUT == A:
        ocisc_io_output( B, outputvar )
        PC += 3
        return A
    if IN == A:
        ocisc_io_input( B, inputvar )
        PC += 3
        return A
    # SUBLEQ instruction
    if verbose:
        print( "%3d: A:%6d B:%6d C:%6d" % (PC, M[A], M[B], C) )
    # (B) = (B) - (A)
    M[ B ] = M[ B ] - M[ A ]
    # if (B) <= 0 goto (C)
    if M[ B ] <= 0 :
        PC = C
    else:
        PC += 3
    return B


def ocisc_load( fn ):
    global M, MEMSIZE, PC
    p = 0
    with open( fn, newline='' ) as csvfile:
        reader = csv.reader( csvfile, delimiter=' ' )
        for row in reader:
            if ';' != row[0]:
                M[p] = int( row[0] )
                p += 1
                M[p] = int( row[1] )
                p += 1
                M[p] = int( row[2] )
                p += 1
                             
def ocisc_initfill ( fn ):
    global M, MEMSIZE, PC
    ocisc_load( fn )
    # Build GUI
    root = Tk()
    root.title( "OCISC Machine Simulator v.0" )
    # Prepare GUI data
    memmapvar = StringVar( value=ocisc_gui_memmap() )
    inputvar  = StringVar( value="" )
    outputvar = StringVar( value="" )
    
    mainframe = ttk.Frame( root, padding="3 3 12 12" )
    mainframe.grid( column=0, row=0, sticky=(N, W, E, S) )
    root.columnconfigure( 0, weight=1 )
    root.rowconfigure( 0, weight=1 )
    ttk.Label( mainframe, text="OCISC Machine (SUBLEQ)" ).grid(column=1, row=1, sticky=(W, E))
    mem = Listbox( mainframe,
                   listvariable=memmapvar,
                   font=('Courier', '10'),
                   selectmode=SINGLE,
                   width=30,
                   height=20 )
    mem.grid(column=1, row=2, sticky=(W, E))
    ioframe = ttk.Frame( mainframe )
    ioframe.grid( column=3, row=2, sticky=(N, W, E, S) )
    ttk.Label( ioframe, text="Input: " ).grid(column=1, row=1, sticky=(W, E))
    inentry = ttk.Entry( ioframe, textvariable=inputvar )
    inentry.grid( column=1, row=2, sticky=(W,E) )
    ttk.Label( ioframe, text="Output: " ).grid(column=1, row=3, sticky=(W, E))
    outentry = ttk.Entry( ioframe, textvariable = outputvar )
    outentry.grid( column=1, row=4, sticky=(W,E) )
    ttk.Button( mainframe,
                text="Step",
                command= lambda: ocisc_gui_step(memmapvar, mem, inputvar, outputvar) ).grid(column=1, row=3, sticky=W)
    ttk.Button( mainframe,
                text="Continue",
                command= lambda: ocisc_gui_continue(memmapvar, mem, inputvar, outputvar) ).grid(column=3, row=3, sticky=E)
    # Paddings everywhere
    for child in mainframe.winfo_children(): 
        child.grid_configure(padx=5, pady=5)
    mem.selection_clear(0, "end")
    mem.selection_set(0);
    # Show time
    root.mainloop()

def main( fn ):
    global M, MEMSIZE, PC
    ocisc_initfill( fn )
    return PC
    
    
if __name__ == '__main__':
    parser = argparse.ArgumentParser(
                    prog='sleq',
                    description='OCISC SUBLEQ Basic Simulator',
                    epilog='')
    parser.add_argument('filename',
                        help="SUBLEQ source file")
    parser.add_argument( '-M', '--MEMSIZE',
                         default=300,
                         required=False,
                         help="Bulk memory size (in cells)" )
    args = parser.parse_args()
    PC       = 0
    MEMSIZE  = args.MEMSIZE
    M = np.zeros( MEMSIZE, dtype=np.int16 )
    main( args.filename )
