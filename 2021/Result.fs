namespace AdventOfCode

module Result =
   /// Apply a Result<fn> to a Result<x> monadically
    let apply fR xR = 
        match fR, xR with
        | Ok f, Ok x -> Ok (f x)
        | Error err1, Ok _ -> Error err1
        | Ok _, Error err2 -> Error err2
        | Error err1, Error _ -> Error err1 

    let sequence (aListOfValidations:Result<_,_> list) = 
        let (<*>) = apply
        let (<!>) = Result.map
        let cons head tail = head::tail
        let consR headR tailR = cons <!> headR <*> tailR
        let initialValue = Ok [] // empty list inside Result
  
        // loop through the list, prepending each element
        // to the initial value
        List.foldBack consR aListOfValidations initialValue