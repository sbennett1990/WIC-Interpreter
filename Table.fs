(**
 * Table.fs - implements a generic table of (string * int) tuples
 *
 * Used as a symbol table and a jump table in the WICI project.
 *
 * Author: Dr. James Fenwick
 * Revisions: Scott Bennett 
 **)
module Table

    type Table = list<string * int>

    exception SymbolNotFound

    (**
     * Store a tuple in the table.
     *
     * param - symbol to attach to a value
     * param - value assigned to the symbol
     * param - table to modify
     **)
    let rec store symbol value table = 
        match table with
        | [] -> 
            (symbol, value)::[]
        | (sym, v)::tail -> 
            if sym = symbol then
                (symbol, value)::tail
            else
                (sym, v)::(store symbol value tail)

    (**
     * Search the table for symbol, then return
     * the value associated with symbol. If the
     * symbol is not found, then SymbolNotFound
     * exception is raised. 
     *
     * param - symbol associated with a value
     * param - table to search
     * return - value of symbol
     **)
    let rec retrieve symbol table = 
        match table with
        | [] -> raise SymbolNotFound
        | (sym, v)::tail -> 
            if sym = symbol then
                v
            else
                retrieve symbol tail

    (**
     * Print the table to the console, with each tuple on its own line.
     * 
     * param - table to print
     **)
    let rec print table = 
        match table with
        | [] -> printf ""
        | (sym, v)::tail -> 
            printf "%s = %d\n" sym v
            print tail
