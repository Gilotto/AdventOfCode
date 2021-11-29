namespace AdventOfCode

module Parser =
    open System
    let parseA str =
        if String.IsNullOrEmpty(str) then
            (false, "")
        elif str.[0] = 'A' then
            let remaining = str.[1..]
            (true, remaining)
        else
            (false, str)