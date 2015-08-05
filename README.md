# WIC-Interpreter

"WIC" stands for Wren Intermediate Code. Wren is a small imperative language mostly used for 
teaching. It "compiles" to WIC. Wren is taken from the book _Syntax and Semantics of Programming Languages_, 
by Ken Slonneger and Barry Kurtz. You can read the book in its entirety at 
http://homepage.cs.uiowa.edu/~slonnegr/plf/Book/

Example Wren program: http://www.cs.arizona.edu/~collberg/Teaching/520/2005/Html/Html-50/#SECTION01050000000000000000

This program takes a WIC program file (`<some-file>.wic`) and interprets it, displaying the results.

## Build

#### Unix

`fsharpc Stack.fs Table.fs Instructions.fs WICI.fs`

#### Windows
Assuming Fsc.exe (the F# compiler included with the .NET Framework) is part of 
your %PATH%:

`Fsc.exe Stack.fs Table.fs Instructions.fs WICI.fs --platform:anycpu --warn:4 -o:WICI.exe`

## Use

#### Unix
`mono WICI.exe <code-file>.wic`

#### Windows
`.\WICI.exe <code-file>.wic`

## About WIC
WIC is a very simple, stack-based intermediate language. The only
supported types are signed integers and Boolean values (1 = true, 0 = false). 

### Flow Control
The `halt` opcode stops the machine.

Labels are defined in the form `L1 label` (Where 1 can be any positive 
number). 

Jump statements are in the form `j L1`, where L1 can be any label. The
two forms of the jump instruction are:

- `j` - Jump unconditionally
- `jf` - Jump if value on the top of the stack is false (0). This pops
  that value off the top of the stack. 
  
The `nop` instruction does nothing. 

### Test statements
Test statements pop a value off the top of the stack and compare
it with zero, pushing either 1 (true) or 0 (false) back on to the
stack. The six test instructions are:

- `tsteq` - equals
- `tstne` - not equals
- `tstlt` - less than
- `tstle` - less than or equal
- `tstgt` - greater than
- `tstge` - greater than or equal

### Math and Logic 
The  four arithmetic operations are `add`, `sub`, `mul`, and `div`. They 
first pop the right operand off the stack, then pop the left operand, 
perform the operation, and finally push the result back on the stack. 

The logical `and` and `or` instructions work the same way. 

The logical `not` instruction is similar, except it only pops a single
value off the stack. 

### Stack instructions
The push instruction is of the form `push A` where A is a variable
name. It pushes the value of the variable A onto the stack. 

The pop instruction is similar -- `pop A` -- except that it pops a
value off of the stack and stores it into variable A. 

### I/O
The `get A` instruction prompts the user `enter A > ` and stores the
input in the variable A.

The `put A` instruction prints `A = <value>` where <value> is the value 
stored in A. 
