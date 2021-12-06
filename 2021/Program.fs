namespace AdventOfCode

open System
open Parser
module main =

    // module TestValues =
    //     let testValid = "ABC"
    //     let testError = "BCD"
    //     let testEmpty = ""
    //     let testLower = "abc"
    //     let testNumbers = "123"

    //     let tests = [testValid;testError;testEmpty;testLower;testNumbers]

    // module ParseSettings =
    //     let parseA = pchar 'A'
    //     let parseB = pchar 'B'
    //     let parseAthenB = parseA .>>. parseB
    //     let parseAorB = parseA <|> parseB
        
    //     let parseLowercase =
    //         anyOf ['a'..'z']
        
    //     let parseDigit =
    //         anyOf ['0'..'9']

    //     let parseThreeDigits =
    //         parseDigit .>>. parseDigit .>>. parseDigit

    //     let parseThreeDigitsAsStr =
    //         parseThreeDigits |>> fun ((c1, c2), c3) -> String [| c1; c2; c3|]

    //     let parseThreeDigitsAsInt =
    //         mapP int parseThreeDigitsAsStr

    //     let parsers = [ 'A'; 'B'; 'C'] |> List.map pchar

    //     let combined = sequence parsers

    //     let parseABC = pstring "ABC"
    //     let parserSettings = parseABC


    [<EntryPoint>]
    let main argv =
        printfn "Day1 Part 1: %s\n" Day01.solutionPart1
        printfn "Day1 Part 2: %s\n" Day01.solutionPart2
        printfn "Day2 Part 1: %A\n" Day02.solutionPart1
        printfn "Day2 Part 2: %A\n" Day02.solutionPart2
        printfn "Day3 Part 1: %A\n" Day03.solutionPart1
        printfn "Day3 Part 2: %A\n" Day03.solutionPart2
        printfn "Day4 Part 1: %s\n" Day04.solutionPart1
        printfn "Day4 Part 2: %s\n" Day04.solutionPart2
        0 // return an integer exit code