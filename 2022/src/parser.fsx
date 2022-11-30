namespace AdventOfCode

module Parser =
    //https://fsharpforfunandprofit.com/posts/understanding-parser-combinators/
    open System

    type ParseResult<'a> =
        | Success of 'a
        | Failure of string
        
    type Parser<'T> = Parser of (string -> ParseResult<'T * string>)

    let run (Parser parser) input =
        parser input

    let mapToResult = function
        |Success (value,_) -> Ok value
        |Failure err -> Error err

    let bindP f (p:Parser<'a>) :Parser<'b>=
        let innerFn input = 
            let result1 = run p input
            match result1 with
            | Failure err ->
                Failure err
            | Success (value1,remainingInput) ->
                let p2 = f value1
                run p2 remainingInput
        Parser innerFn

    /// bindP
    let ( >>= ) (p:Parser<'a>) f :Parser<'b>= bindP f p

    let returnP (x:'a) :Parser<'a>=
        let innerFn input =
            Success (x, input)
        Parser innerFn

    let pchar charToMatch =
        let innerFn str =
            if String.IsNullOrEmpty(str) then
                Failure "No more input"
                
            else
                let first = str.[0]
                if first = charToMatch then
                    let remaining = str.[1..]
                    Success (charToMatch,remaining)
                else
                    let msg = sprintf "Expecting '%c'. Got '%c'" charToMatch first
                    Failure msg
        Parser innerFn

    let applyP fP xP =
        fP >>= (fun f -> 
        xP >>= (fun x ->
            returnP (f x))) 
    
    /// applyP
    let ( <*> ) = applyP

    let andThen p1 p2 =
        p1 >>= (fun p1Result ->
        p2 >>= (fun p2Result ->
            returnP (p1Result,p2Result) ))
    
    /// andThen
    let ( .>>. ) = andThen

    let mapP f =
        bindP (f >> returnP)
    
    /// mapP
    let ( <!> ) = mapP

    /// mapP
    let ( |>> ) x f = mapP f x
    
    /// andThenKeepFirst
    let ( .>> ) p1 p2 =
        p1 .>>. p2
        |> mapP fst
    /// andThenKeepSecond
    let ( >>. ) p1 p2 =
        p1 .>>. p2
        |> mapP snd

    let orElse parser1 parser2 =
        let innerFn input =
            let result1 = run parser1 input
            match result1 with
            | Failure err -> 
                let result2 = run parser2 input
                result2
            | Success result ->
                result1
        Parser innerFn
    
    /// orElse
    let ( <|> ) = orElse

    let opt (p:Parser<'a>) :Parser<'a option>=
        let some = p |>> Some
        let none = returnP None
        some <|> none  

    let choice listOfParsers =
        List.reduce ( <|> ) listOfParsers 
    
    let anyOf listOfChars =
        listOfChars
        |> List.map pchar
        |> choice

    let lift2 f xP yP =
        returnP f <*> xP <*> yP
    
    let addP =
        lift2 (+)
    
    let startsWith (str:string) (prefix:string) = 
        str.StartsWith(prefix)
    
    let startsWithP =
        lift2 startsWith

    let rec sequence parserList =
        let cons head tail = head::tail

        let consP = lift2 cons

        match parserList with
        | [] -> 
            returnP []
        | head :: tail -> 
            consP head (sequence tail)
    
    let charListToStr charList =
        charList |> List.toArray |> String
    
    let pstring str =
        str
        |> List.ofSeq
        |> List.map pchar
        |> sequence
        |> mapP charListToStr
    
    let rec parseZeroOrMore parser input =
        let firstResult = run parser input
        match firstResult with
        | Failure err ->
            ([], input)
        | Success (firstValue, inputAfterFirstParse) ->
            let (subsequentValues,remainingInput) =
                parseZeroOrMore parser inputAfterFirstParse
            let values = firstValue::subsequentValues
            (values,remainingInput)
    
    let many parser =
        let innerFn input =
            Success (parseZeroOrMore parser input)
        Parser innerFn

    let many1 p =
        p      >>= (fun head ->
        many p >>= (fun tail ->
            returnP (head::tail) ))

    let pint =
        let resultToInt (sign,charList) = 
            let i = charList |> List.toArray |> System.String |> int //can overflow!
            match sign with
            | Some _ -> -i
            | None -> i
        
        let digit = anyOf ['0'..'9']
        let digits = many1 digit
        
        opt (pchar '-') .>>. digits
        |>> resultToInt
    
    let between p1 p2 p3 =
        p1 >>. p2 .>> p3

    let sepBy1 p sep =
        let sepThenP = sep >>. p
        p .>>. many sepThenP
        |>> fun (p, pList) -> p::pList
    
    let sepBy p sep =
        sepBy1 p sep <|> returnP []

    let pfloat =
        let resultToFloat ((sign,digits1),digits2) =
            let fl = sprintf "%s.%s" (digits1|> charListToStr) (digits2|> charListToStr) |> float
            match sign with
            |Some _ -> -fl
            |None -> fl
        
        let digit = anyOf ['0'..'9']
        let digits = many1 digit

        opt (pchar '-') .>>. digits .>> pchar '.' .>>. digits
        |> mapP resultToFloat