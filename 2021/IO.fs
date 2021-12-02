namespace AdventOfCode

module IO =

    open System.IO
    let readInput location =
        File.ReadAllLines location
        |> List.ofArray