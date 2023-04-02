from tkinter import *
from tkinter import ttk
import numpy as np

global M, MEMSIZE, PC

def trace_mem():
    global M, MEMSIZE, PC
    print( "MEM:" )
    for i in range( 0, 12, 3 ):
        print( "%2d: %6d %6d %6d" % (i, M[i], M[i+1], M[i+2] ) )

        
def ocisc_gui_step( memmapvar, mem ):
    global M, MEMSIZE, PC
    if M[PC] < 0 :
        pass
    else:
        B = ocisc_vm_step( verbose=False )
        memmapvar.set( ocisc_gui_memmap() )
        mem.selection_clear(0, "end")
        mem.selection_set(PC // 3);
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


def ocisc_vm_step(verbose=True):
    global M, MEMSIZE, PC

    A, B, C = M[ PC ], M[ PC+1 ], M[ PC+2 ]
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
        
def ocisc_initfill ():
    global M, MEMSIZE, PC
    # A first program
    # See also: [[https://esolangs.org/wiki/Subleq]]
    M[0], M[1], M[2]   = 3, 4, 6
    M[3], M[4], M[5]   = 7, 7, 7
    M[6], M[7], M[8]   = 3, 4, 9
    M[9], M[10], M[11] = -1, -1, -1
    # Build GUI
    root = Tk()
    root.title( "OCISC Machine Simulator v.0" )
    # Prepare GUI data
    memmapvar = StringVar( value=ocisc_gui_memmap() )
    
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
    ttk.Button( mainframe,
                text="Step",
                command= lambda: ocisc_gui_step(memmapvar, mem) ).grid(column=1, row=3, sticky=W)
    # Paddings everywhere
    for child in mainframe.winfo_children(): 
        child.grid_configure(padx=5, pady=5)
    mem.selection_clear(0, "end")
    mem.selection_set(0);
    # Show time
    root.mainloop()

def main( verbose=True ):
    global M, MEMSIZE, PC
    ocisc_initfill()
    # OCISC Machine
    # while True :
    #     if M[PC] < 0 :
    #         break
    #     if verbose:
    #         trace_mem()
    #         print( "%3d: A:%6d B:%6d C:%6d" % (PC, M[ PC ], M[ PC+1 ], M[ PC+2 ]) )
    #     A, B, C = M[ PC ], M[ PC+1 ], M[ PC+2 ]
    #     if verbose:
    #         print( "%3d: A:%6d B:%6d C:%6d" % (PC, M[A], M[B], C) )
    #     # (B) = (B) - (A)
    #     M[ B ] = M[ B ] - M[ A ]
    #     # if (B) <= 0 goto (C)
    #     if M[ B ] <= 0 :
    #         PC = C
    #     else:
    #         PC += 3
    # trace_mem()
    return PC
    
    
if __name__ == '__main__':
    PC       = 0
    MEMSIZE  = 300
    M = np.zeros( MEMSIZE, dtype=np.int16 )
    main()
