CC = gcc
CFLAGS =
CPPFLAGS =

LDFLAGS =
LOADLIBES =
LDLIBS = 

subleq: subleq.o sl_monitor.o sl_instrument.o

clean:
	DEL lbforth.o lbforth.exe

