namespace AdventOfCode

module Day06 =

// --- Day 6: Lanternfish ---

// The sea floor is getting steeper. Maybe the sleigh keys got carried this way?

// A massive school of glowing lanternfish swims past. They must spawn quickly to reach such large numbers - maybe exponentially quickly? You should model their growth rate to be sure.

// Although you know nothing about this specific species of lanternfish, you make some guesses about their attributes. Surely, each lanternfish creates a new lanternfish once every 7 days.

// However, this process isn't necessarily synchronized between every lanternfish - one lanternfish might have 2 days left until it creates another lanternfish, while another might have 4. So, you can model each fish as a single number that represents the number of days until it creates a new lanternfish.

// Furthermore, you reason, a new lanternfish would surely need slightly longer before it's capable of producing more lanternfish: two more days for its first cycle.

// So, suppose you have a lanternfish with an internal timer value of 3:

//     After one day, its internal timer would become 2.
//     After another day, its internal timer would become 1.
//     After another day, its internal timer would become 0.
//     After another day, its internal timer would reset to 6, and it would create a new lanternfish with an internal timer of 8.
//     After another day, the first lanternfish would have an internal timer of 5, and the second lanternfish would have an internal timer of 7.

// A lanternfish that creates a new fish resets its timer to 6, not 7 (because 0 is included as a valid timer value). The new lanternfish starts with an internal timer of 8 and does not start counting down until the next day.

// Realizing what you're trying to do, the submarine automatically produces a list of the ages of several hundred nearby lanternfish (your puzzle input). For example, suppose you were given the following list:

// 3,4,3,1,2

// This list means that the first fish has an internal timer of 3, the second fish has an internal timer of 4, and so on until the fifth fish, which has an internal timer of 2. Simulating these fish over several days would proceed as follows:

// Initial state: 3,4,3,1,2
// After  1 day:  2,3,2,0,1
// After  2 days: 1,2,1,6,0,8
// After  3 days: 0,1,0,5,6,7,8
// After  4 days: 6,0,6,4,5,6,7,8,8
// After  5 days: 5,6,5,3,4,5,6,7,7,8
// After  6 days: 4,5,4,2,3,4,5,6,6,7
// After  7 days: 3,4,3,1,2,3,4,5,5,6
// After  8 days: 2,3,2,0,1,2,3,4,4,5
// After  9 days: 1,2,1,6,0,1,2,3,3,4,8
// After 10 days: 0,1,0,5,6,0,1,2,2,3,7,8
// After 11 days: 6,0,6,4,5,6,0,1,1,2,6,7,8,8,8
// After 12 days: 5,6,5,3,4,5,6,0,0,1,5,6,7,7,7,8,8
// After 13 days: 4,5,4,2,3,4,5,6,6,0,4,5,6,6,6,7,7,8,8
// After 14 days: 3,4,3,1,2,3,4,5,5,6,3,4,5,5,5,6,6,7,7,8
// After 15 days: 2,3,2,0,1,2,3,4,4,5,2,3,4,4,4,5,5,6,6,7
// After 16 days: 1,2,1,6,0,1,2,3,3,4,1,2,3,3,3,4,4,5,5,6,8
// After 17 days: 0,1,0,5,6,0,1,2,2,3,0,1,2,2,2,3,3,4,4,5,7,8
// After 18 days: 6,0,6,4,5,6,0,1,1,2,6,0,1,1,1,2,2,3,3,4,6,7,8,8,8,8

// Each day, a 0 becomes a 6 and adds a new 8 to the end of the list, while each other number decreases by 1 if it was present at the start of the day.

// In this example, after 18 days, there are a total of 26 fish. After 80 days, there would be a total of 5934.

// Find a way to simulate lanternfish. How many lanternfish would there be after 80 days?

// To begin, get your puzzle input.


// 
//  Je hebt daysLeft en timer
//  bij 80 dagen en timer 8 (maximaal) = aantal vissen = 80 - 9 = 71 /7 = 10
// dag 0 - 80,8
// dag 1 - 79,7
// dag 2 - 78,6
// dag 3 - 77,5
// dag 4 - 76,4
// dag 5 - 75,3
// dag 6 - 74,2
// dag 7 - 73,1
// dag 8 - 72,0
// dag 9 - 71,6 + 71,8
// dag10 - 70,5 + 70,7
// dag11 - 69,4 + 69,6
// dag12 - 68,3 + 68,5
// dag13 - 67,2 + 67,4
// dag14 - 66,1 + 66,3
// dag15 - 65,0 + 65,2
// dag16 - 64,6 + 64,1 + 64,8
// dag17 - 63,5 + 63,0 + 63,7
// dag18 - 62,4 + 62,6 + 62,6 + 62,8
// dag19 - 61,3 + 61,5 + 61,5 + 61,7
// dag20 - 60,2 + 60,4 + 60,4 + 60,6
// dag21 - 59,1 + 59,3 + 59,3 + 59,5
// dag22 - 58,0 + 58,2 + 58,2 + 58,4
// dag23 - 57,6 + 57,1 + 57,1 + 57,3 + 57,8
// dag24 - 56,5 + 56,0 + 56,0 + 56,2 + 56,7
// dag25 - 55,4 + 55,6 + 55,6 + 55,1 + 55,6 + 55,8 + 55,8
// dag26 - 54,3 + 54,5 + 54,5 + 54,0 + 54,5 + 54,7 + 54,7
// dag27 - 53,2 + 53,4 + 53,4 + 53,6 + 53,4 + 53,6 + 53,6 + 53.8
// dag28 - 52,1 + 52,3 + 52,3 + 52,5 + 52,3 + 52,5 + 52,5 + 52,7
// dag29 - 51,0 + 51,2 + 51,2 + 51,4 + 51,2 + 51,4 + 51,4 + 51,6
// dag30 - 50,6 + 50,1 + 50,1 + 50,3 + 50,1 + 50,3 + 50,3 + 50,3 

// dus vis1 (80,8) genereert (daysLeft - (initialtimer + 1) ,8) en dan (daysLeft - (6 + 1) ,8)
// 1 + (daysleft-timer,8) + daysleft-timer-8,8 + daysleft-timer-8*2,8 + ...
// 1 + 1 + (daysleft1-timer,8) + daysleft1-timer-8,8 + daysleft1-timer-8*2,8

    let fishGenerator (daysleft, initialtimer) =
        let generator currentFish = 
            match currentFish with
            |(daysleft,timer) when (daysleft - timer) <= 0 ->
                None
            |(daysleft,timer) -> 
                Some ((daysleft-(timer+1),8), (daysleft - (timer+1),6))
        List.unfold generator (daysleft,initialtimer)
        
    let rec countFish countedSoFar poolOfFish =
        match poolOfFish with
        |[] -> countedSoFar
        |head::tail -> 
            let (fish, number) = head
            let fishBreeded = List.init number (fun _ -> fish) |> List.collect fishGenerator 
            let rec addFishToTail (tailSoFar:((int*int)*int) list) (newFish:(int*int) list) =
                match newFish with
                |[] -> tailSoFar
                |head::tail -> 
                    addFishToTail
                        (match tailSoFar |> List.map fst|> List.contains head with 
                        |true -> tailSoFar |> List.map (fun (fish,number) -> if fish = head then fish, number + 1 else fish,number)
                        |false -> (head,1)::tailSoFar)
                        tail
                    
            countFish (countedSoFar + float number) (addFishToTail tail fishBreeded)

    open Parser 
    let parseStetting = pint .>> (opt (pchar ',')) |> many

    let inputlocation = "./input/day06.txt"
    let testInput = ["3,4,3,1,2"]

        //80 - 3 = 73 /7 = 10 fish + rest
    let solutionPart1 =
        let daysLeft = 80
        IO.readInput inputlocation
        // testInput
        |> List.map (run parseStetting)
        |> List.tryHead
        |> Option.bind (function
            | Success parse -> parse |> fst |> List.map (fun fish -> daysLeft,fish )|> Some
            | Failure _ -> None
            )
        |> Option.map (List.countBy id)
        |> Option.map (countFish 0.)
        |> Option.map (sprintf "The answer is %0.0f")
    
// --- Part Two ---

// Suppose the lanternfish live forever and have unlimited food and space. Would they take over the entire ocean?

// After 256 days in the example above, there would be a total of 26984457539 lanternfish!

// How many lanternfish would there be after 256 days?


//(0,0), -> 1
//(0,1), -> 1
//(0,2), -> 1
//(0,3), -> 1
//(0,4), -> 1
//(0,5), -> 1
//(0,6), -> 1
//(0,7), -> 1
//(0,8), -> 1
//(1,0), -> (0,6) + (0,8) = 2
//(1,1), -> (0,0) = 1
//(1,2), -> (0,1) = 1
//(1,3), -> (0,2) = 1
//(1,4), -> (0,3) = 1
//(1,5), -> (0,4) = 1
//(1,6), -> (0,5) = 1
//(1,7), -> (0,6) = 1
//(1,8), -> (0,7) = 1
//(2,0), -> (1,6) + (1,8) -> (0,5) + (0,7) = 2
//(2,1), -> (1,0) -> (0,6) + (0,8) = 2
//(2,2), -> (1,1) -> (0,0) = 1
    let rec updateMapWithList map list =
        match list with
        |[] -> map
        |(key,value)::tail -> updateMapWithList (Map.add key value map) tail
    let rec fishAlgorithm count days (map:Map<(int*int),float>) =
        match days with
        |0 -> match count with
                |[||] -> failwith "Error - empty list at fishalgorithm"
                |[|fish|] -> Map.find (0,fish) map
                |fishes -> fishes |> Array.map (fun item -> map |> Map.find (0,item)) |> Array.reduce (+)
        |_ -> 
            fishAlgorithm (
                count 
                |> Array.collect (fun (timer) -> 
                    if timer - 1 < 0 
                    then [|6; 8|] 
                    else [|timer - 1|])) 
                (days - 1) map

    let fishMap days =
        let days = [1..days]
        let initialMap = 
            [
                (0,8),1.
                (0,7),1.
                (0,6),1.
                (0,5),1.
                (0,4),1.
                (0,3),1.
                (0,2),1.
                (0,1),1.
                (0,0),1.
            ] |> Map.ofList
        let rec fillMap (mapSoFar:Map<(int*int),float>) days =
            match days with
            |[] -> mapSoFar
            |day::otherdays -> 
                let updateMap =
                    [0..8] 
                    |> List.map (fun timer -> (day,timer),fishAlgorithm [|timer|] day mapSoFar)
                    |> updateMapWithList mapSoFar
                fillMap updateMap otherdays
        fillMap initialMap days

    let rec countfishWithMap countedSoFar (map:Map<(int*int),float>) poolOfFish = 
        match poolOfFish with
        |[] -> countedSoFar
        |head::tail -> 
            let fishBreeded = 
                head 
                |> fishGenerator 
                |> List.map (fun fish -> 
                    map.Item fish)
                |> List.reduce (+)
            countfishWithMap (countedSoFar + fishBreeded + 1.) map tail
    
    // let solutionPart2 =
    //     let daysLeft = 256
    //     let fishmap = fishMap daysLeft
    //     // IO.readInput inputlocation
    //     testInput
    //     |> List.map (run parseStetting)
    //     |> List.tryHead
    //     |> Option.bind (function
    //         | Success parse -> parse |> fst |> List.map (fun fish -> daysLeft,fish )|> Some
    //         | Failure _ -> None
    //         )
    //     |> Option.map (countfishWithMap 0. fishmap)
    //     |> Option.map (sprintf "The answer is %0.0f")