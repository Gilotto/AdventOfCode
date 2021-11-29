namespace AdventOfCode

open System
open Parser
module main =

    [<EntryPoint>]
    let main argv =
        argv
        |> Array.map Parser.parseA 
        |> printfn "%A"
        0 // return an integer exit code