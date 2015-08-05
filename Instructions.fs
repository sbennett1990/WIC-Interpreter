(**
 * File: Instructions.fs
 * Author: Scott Bennett
 **)
module Instructions
    open System
    open System.Text.RegularExpressions
    open Stack
    open Table

    type State = {
        stack: Stack;
        symTable: Table;
        jumpTable: Table;
        PC: int
    }

    exception DivideByZero

    (**
     * These four arithmetic operations () pop two values off
     * the top of the stack. First popped is the right hand
     * operand, then the left hand operand. The result of 
     * the operation is then pushed onto the stack.
     *
     * param - state of the current program
     **)

    (* Addition *)
    let add (state : State) = 
        let (op2, stk) = pop state.stack
        let (op1, stk) = pop stk
        let stk = push (op1 + op2) stk
        {state with PC = state.PC + 1; stack = stk}

    
    (* Subtraction *)
    let sub (state : State) =
        let (op2, stk) = pop state.stack
        let (op1, stk) = pop stk
        let stk = push (op1 - op2) stk
        {state with PC = state.PC + 1; stack = stk}

    (* Multiplication *)
    let mul (state : State) =
        let (op2, stk) = pop state.stack
        let (op1, stk) = pop stk
        let stk = push (op1 * op2) stk
        {state with PC = state.PC + 1; stack = stk}

    (* Division *)
    let div (state : State) =
        let (op2, stk) = pop state.stack
        if op2 = 0 then
            raise DivideByZero
        else
            let (op1, stk) = pop stk
            let stk = push (op1 / op2) stk
            {state with PC = state.PC + 1; stack = stk}

    (**
     * Terminate the program. This instruction only has
     * the effect of incrementing the program counter.
     * Termination is handled in WICI.fs.
     *
     * param - state of the current program
     **)
    let halt (state : State) = 
        {state with PC = state.PC + 1}

    (**
     * Used to store labels into the jump table. If a
     * label is already in the jump table, then the 
     * table is unaltered and the only change is to
     * increment the PC.
     *
     * param - name of a label
     * param - state of the current program
     **)
    let nop label (state : State) = 
        try
            let l = retrieve label state.jumpTable
            // label found, only increment PC
            {state with PC = state.PC + 1}
        with
            | SymbolNotFound ->
                // label not found in jump table, so add it
                let jTab = store label state.PC state.jumpTable
                {state with PC = state.PC + 1; jumpTable = jTab} 

    (**
     * Unconditionally jump to the address of label.
     * If the label is not in the jump table, the
     * exception is handled in main and execution halts.
     *
     * param - name of a label, which is the 
     *         destination of the jump
     * param - state of the current program
     **)
    let j label (state : State) =
        let dest = retrieve label state.jumpTable
        {state with PC = dest}

    (**
     * Conditional jump to the address of label.
     * Pop the stack. If the value is 0 then the
     * jump is taken, otherwise the jump is not
     * taken and the PC is incremented by 1.
     *
     * param - name of a label
     * param - state of the current program
     **)
    let jf label (state : State) =
        let (op, stk) = pop state.stack
        if op = 0 then 
            let dest = retrieve label state.jumpTable
            {state with PC = dest; stack = stk}
        else 
            // don't take the jump, fall through to next instruction
            {state with PC = state.PC + 1; stack = stk}

    (**
     * Performs a logical AND on two operands popped
     * off the stack. Renamed to 'wicAnd' because 'and'
     * is an F# keyword and the compiler freaks out.
     *
     * param - state of the current program
     * return - value of op1 && op2
     **)
    let wicAnd (state : State) =
        let (op2, stk) = pop state.stack
        let (op1, stk) = pop stk
        if op1 = 0 then
            let stk = push 0 stk
            {state with PC = state.PC + 1; stack = stk}
        else if op2 = 0 then
            let stk = push 0 stk
            {state with PC = state.PC + 1; stack = stk}
        else // op1 and op2 are not 0
            let stk = push 1 stk
            {state with PC = state.PC + 1; stack = stk}

    (**
     * Performs a logical OR on two operands popped
     * off the stack. Ranamed to 'wicOr' bacause 'or'
     * is an F# keyword and the compiler freaks out.
     *
     * param - state of the current program
     * return - value of op1 || op2
     **)
    let wicOr (state : State) =
        let (op2, stk) = pop state.stack
        let (op1, stk) = pop stk
        if op1 <> 0 then 
            let stk = push 1 stk
            {state with PC = state.PC + 1; stack = stk}
        else if op2 <> 0 then
            let stk = push 1 stk
            {state with PC = state.PC + 1; stack = stk}
        else // op1 and op2 are 0
            let stk = push 0 stk
            {state with PC = state.PC + 1; stack = stk}

    (**
     * Performs a logical NOT on one operand popped
     * off the stack. Renamed to 'wicNot' because 'not'
     * is an F# keyword and the compiler freaks out.
     *
     * param - state of the current program
     * return - value of !op
     **)
    let wicNot (state : State) =
        let (op, stk) = pop state.stack
        if op = 0 then
            let stk = push 1 stk
            {state with PC = state.PC + 1; stack = stk}
        else
            let stk = push 0 stk
            {state with PC = state.PC + 1; stack = stk}

    (**
     * Pop the stack, test if that value is equal to 0.
     * If value = 0, push 1 onto stack, else push 0.
     *
     * param - state of the current program
     **)
    let tsteq (state : State) = 
        let (op, stk) = pop state.stack
        if op = 0 then
            let stk = push 1 stk
            {state with PC = state.PC + 1; stack = stk}
        else
            let stk = push 0 stk
            {state with PC = state.PC + 1; stack = stk}

    (**
     * Pop the stack, test if that value is not equal
     * to 0. If value <> 0, push 1 onto stack, else
     * push 0.
     *
     * param - state of the current program
     **)
    let tstne (state : State) = 
        let (op, stk) = pop state.stack
        if op <> 0 then
            let stk = push 1 stk
            {state with PC = state.PC + 1; stack = stk}
        else
            let stk = push 0 stk
            {state with PC = state.PC + 1; stack = stk}

    (**
     * Pop the stack, test if that value is less than 0.
     * If value < 0, push 1 onto stack, else push 0.
     *
     * param - state of the current program
     **)
    let tstlt (state : State) =
        let (op, stk) = pop state.stack
        if op < 0 then
            let stk = push 1 stk
            {state with PC = state.PC + 1; stack = stk}
        else
            let stk = push 0 stk
            {state with PC = state.PC + 1; stack = stk}

    (**
     * Pop the stack, test if that value is less than
     * or equal to 0. If value <= 0, push 1 onto
     * stack, else push 0.
     *
     * param - state of the current program
     **)
    let tstle (state : State) =
        let (op, stk) = pop state.stack
        if op <= 0 then
            let stk = push 1 stk
            {state with PC = state.PC + 1; stack = stk}
        else
            let stk = push 0 stk
            {state with PC = state.PC + 1; stack = stk}
      
    (**
     * Pop the stack, test if that value is greater
     * than 0. If value > 0, push 1 onto stack,
     * else push 0.
     *
     * param - state of the current program
     **)
    let tstgt (state : State) =
        let (op, stk) = pop state.stack
        if op > 0 then
            let stk = push 1 stk
            {state with PC = state.PC + 1; stack = stk}
        else
            let stk = push 0 stk
            {state with PC = state.PC + 1; stack = stk}

    (**
     * Pop the stack, test if that value is greater
     * than or equal to 0. If value >= 0, push 1
     * onto stack, else push 0.
     *
     * param - state of the current program
     **)
    let tstge (state : State) =
        let (op, stk) = pop state.stack
        if op >= 0 then
            let stk = push 1 stk
            {state with PC = state.PC + 1; stack = stk}
        else
            let stk = push 0 stk
            {state with PC = state.PC + 1; stack = stk}

    (**
     * Get input (an integer) from the user and store that 
     * input in the symbol table. 
     *
     * param - symbol to assign the user input
     * param - state of the current program
     **)
    let get symbol (state : State) =
        printf "enter %s > " symbol
        let v = Console.ReadLine()
        let num = Convert.ToInt32 v
        let newSymT = store symbol num state.symTable
        {state with PC = state.PC + 1; symTable = newSymT}

    (**
     * Fetch the current value of "symbol" from the
     * symbol table and prints to the console.
     *
     * param - symbol in the symbol table
     * param - state of the current program
     **)
    let put symbol (state : State) =
        let v = retrieve symbol state.symTable
        printf "%s = %d\n" symbol v
        {state with PC = state.PC + 1}

    (**
     * Retrieve value from the symbol table and push it
     * onto the stack. If operand is an integer and not
     * a symbol, push the integer onto the stack without
     * accessing the symbol table.
     *
     * param - value to push onto the stack
     * param - state of the current program
     **)
    let push operand (state : State) =
        // Test for symbol
        let matches = Regex.Matches(operand, "^[a-zA-Z].*$")
      
        if (matches.Count = 1) then
            let v = retrieve operand state.symTable
            let stk = Stack.push v state.stack
            {state with PC = state.PC + 1; stack = stk}
        else // Must be a number
            let v = Convert.ToInt32 operand
            let stk = Stack.push v state.stack
            {state with PC = state.PC + 1; stack = stk}
      
    (**
     * Pop a value off the top of the stack and put it
     * into the symbol table.
     *
     * param - symbol to modify in symbol table
     * param - state of the current program
     **)
    let pop symbol (state : State) =
        let (v, stk) = Stack.pop state.stack
        let newSymT = store symbol v state.symTable
        {state with PC = state.PC + 1; stack = stk; symTable = newSymT}
