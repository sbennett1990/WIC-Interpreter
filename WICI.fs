(**
 * File: WICI.fs
 * Author: Scott Bennett
 **)
module WICI
    open System
    open System.IO
    open Stack
    open Table
    open Instructions

    exception NoCmdArg
    exception FileNotFound of string
    exception InstructionNotFound
    exception ProgramEmpty

    (**
     * Takes as input a list of tuples where each tuple is a wic instruction
     * in the form (opcode, operand).  Interprets each instruction until halt
     * is executed.
     * @params - instructions - list of tuples
     *         - state - state of the machine represented by a Stack, 
     *                   SymbolTable, JumpTable and PC
     * @returns - string "Program halted"
     **)
    let rec interpret instructions state = 
        if instructions = [] then
            raise ProgramEmpty

        let instr = List.nth instructions state.PC
        match instr with
        | ("halt", operand) -> 
            printfn "\nSymbol Table"
            printfn "------------"
            print state.symTable
            "\nProgram halted"
        | ("nop", operand) -> let newState = nop operand state
                              interpret instructions newState
        | ("add", operand) -> let newState = add state
                              interpret instructions newState
        | ("sub", operand) -> let newState = sub state
                              interpret instructions newState
        | ("mul", operand) -> let newState = mul state
                              interpret instructions newState
        | ("div", operand) -> let newState = div state
                              interpret instructions newState
        | ("push", operand) -> let newState = push operand state
                               interpret instructions newState
        | ("pop", operand) -> let newState = pop operand state
                              interpret instructions newState
        | ("get", operand) -> let newState = get operand state
                              interpret instructions newState
        | ("put", operand) -> let newState = put operand state
                              interpret instructions newState
        | ("and", operand) -> let newState = wicAnd state
                              interpret instructions newState
        | ("or", operand) -> let newState = wicOr state
                             interpret instructions newState
        | ("not", operand) -> let newState = wicNot state
                              interpret instructions newState
        | ("tsteq", operand) -> let newState = tsteq state
                                interpret instructions newState
        | ("tstne", operand) -> let newState = tstne state
                                interpret instructions newState
        | ("tstlt", operand) -> let newState = tstlt state
                                interpret instructions newState
        | ("tstle", operand) -> let newState = tstle state
                                interpret instructions newState
        | ("tstgt", operand) -> let newState = tstgt state
                                interpret instructions newState
        | ("tstge", operand) -> let newState = tstge state
                                interpret instructions newState
        | ("j", operand) -> let newState = j operand state
                            interpret instructions newState
        | ("jf", operand) -> let newState = jf operand state
                             interpret instructions newState
        | unknownInstr -> 
            raise InstructionNotFound

    (**
     * Takes as input the name of a wic file. It opens the file and
     * reads the instructions into an array where each element of the
     * array is an instruction.
     * 
     * @param - string that is the file name
     * @returns - array of lines in the file
     **)
    let readFile file = 
        if not(File.Exists(file)) then
            raise(FileNotFound(file))
        
        File.ReadAllLines(file)

    (**
     * Takes as input the a list of instructions.  Parses the instructions
     * and returns a list of tuples in the form: (opcode, operand)
     * @param - list of instructions where each element is in the form:
     *          "   opcode operand  "
     * @returns - list of tuples in the form: (opcode, operand)
     *            label instruction is changed to a nop
     **)
    let rec buildInstructions (input : List<string>) = 
        match input with
        | [] -> []
        | head::tail ->
            //split on whitespace and get rid of the "" elements
            let pieces = Array.filter (fun ele -> not(ele = "")) 
                                      (head.Split [|' '|]) 
            if (Array.length pieces = 0) || (pieces.[0] = "#") then
                buildInstructions tail
            else
                let fst = pieces.[0]
                let snd = 
                    if (Array.length pieces = 1) then 
                        "" 
                    else 
                        pieces.[1]
                if snd = "label" then 
                    ("nop", fst)::buildInstructions tail
                else 
                    (fst, snd)::buildInstructions tail

    (**
     * builds the jump table for the wic instructions.  Since label instructions
     * were change to nop instructions, the labels are the operand of the nop
     * instructions.  Each label and address is added to the jump table.
     * @params - instr - list of tuples in the form (opcode, operand)
     *           addr - starting value of PC (0 when first called)
     * @returns - jump table
     **)
    let rec buildJumpTable instr addr = 
        match instr with 
        | [] -> []
        | (fst, snd)::tail -> 
            if fst = "nop" then 
                (snd, addr)::buildJumpTable tail (addr + 1)
            else
                buildJumpTable tail (addr + 1)

    (**
     * Execution starts here
     * param - command line argument that the name of the file containing
     *         wic instructions
     **)
    [<EntryPoint>]
    let main args = 
        try
            if (Array.length args) = 0 then 
                raise NoCmdArg
            
            let file = args.[0]
            let input = readFile file
            let instr = buildInstructions (List.ofArray input)
            let jmpTab = buildJumpTable instr 0;
            printfn "%s" (interpret instr {symTable = []; jumpTable = jmpTab; stack = []; PC = 0})
        with
            | NoCmdArg -> 
                printfn "Usage: mono wi.exe <filename>"
            | FileNotFound(file) -> 
                printfn "%A does not exist\nUsage: mono wi.exe <filename>" file 
            | StackEmpty ->
                printfn "Stack underflow error. Execution halted."
             | SymbolNotFound ->
                printfn "Table retrieve failed. Symbol not found. Execution halted."
            | InstructionNotFound -> 
                printfn "instruction not found. Execution halted." 
             | ProgramEmpty -> 
                printfn "Program file empty. Execution halted." 
            | DivideByZero -> 
                printfn "Divide by zero. Execution halted." 
        
        0
