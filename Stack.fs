(**
 * Stack.fs - implements a stack<int> abstract data type
 * Author: Scott Bennett
 **)
module Stack

    type Stack = list<int>

    exception StackEmpty

    (**
     * Push a value onto the top of the stack.
     * 
     * param - value to push
     * param - stack to push it onto
     * return - new stack
     **)
    let push v (stk : Stack) = 
        v::stk

    (**
     * Peek at the top of the stack without modifying it.
     * 
     * param - stack to peek at
     * return - value at the top
     **)
    let top (stk : Stack) = 
        match stk with
        | [] -> raise StackEmpty
        | head::tail -> head

    (**
     * Pop a value off the top of the stack.
     *
     * param - stack to pop
     * return - tuple of (value * new stack)
     **)
    let pop (stk : Stack) = 
        match stk with
        | [] -> raise StackEmpty
        | head::tail -> (head, tail)
