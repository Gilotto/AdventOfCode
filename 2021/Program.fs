namespace AdventOfCode

open System
open Parser
module main =

    module TestValues =
        let testValid = "ABC"
        let testError = "BCD"
        let testEmpty = ""
        let testLower = "abc"
        let testNumbers = "123"

        let tests = [testValid;testError;testEmpty;testLower;testNumbers]

    module ParseSettings =
        let parseA = pchar 'A'
        let parseB = pchar 'B'
        let parseAthenB = parseA .>>. parseB
        let parseAorB = parseA <|> parseB
        
        let parseLowercase =
            anyOf ['a'..'z']
        
        let parseDigit =
            anyOf ['0'..'9']

        let parseThreeDigits =
            parseDigit .>>. parseDigit .>>. parseDigit

        let parseThreeDigitsAsStr =
            parseThreeDigits |>> fun ((c1, c2), c3) -> String [| c1; c2; c3|]

        let parseThreeDigitsAsInt =
            mapP int parseThreeDigitsAsStr

        let parsers = [ 'A'; 'B'; 'C'] |> List.map pchar

        let combined = sequence parsers

        let parseABC = pstring "ABC"
        let parserSettings = parseABC

    [<EntryPoint>]
    let main argv =
        TestValues.tests
        |> List.map (ParseSettings.parserSettings |> run)
        |> List.iter (printfn "%A")
        0 // return an integer exit code