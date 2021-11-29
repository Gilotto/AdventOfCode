namespace AdventOfCode
// --- Day 11: Seating System ---

// Your plane lands with plenty of time to spare. The final leg of your journey is a ferry that goes directly to the tropical island where you can finally start your vacation. As you reach the waiting area to board the ferry, you realize you're so early, nobody else has even arrived yet!

// By modeling the process people use to choose (or abandon) their seat in the waiting area, you're pretty sure you can predict the best place to sit. You make a quick map of the seat layout (your puzzle input).

// The seat layout fits neatly on a grid. Each position is either floor (.), an empty seat (L), or an occupied seat (#). For example, the initial seat layout might look like this:

// L.LL.LL.LL
// LLLLLLL.LL
// L.L.L..L..
// LLLL.LL.LL
// L.LL.LL.LL
// L.LLLLL.LL
// ..L.L.....
// LLLLLLLLLL
// L.LLLLLL.L
// L.LLLLL.LL

// Now, you just need to model the people who will be arriving shortly. Fortunately, people are entirely predictable and always follow a simple set of rules. All decisions are based on the number of occupied seats adjacent to a given seat (one of the eight positions immediately up, down, left, right, or diagonal from the seat). The following rules are applied to every seat simultaneously:

//     If a seat is empty (L) and there are no occupied seats adjacent to it, the seat becomes occupied.
//     If a seat is occupied (#) and four or more seats adjacent to it are also occupied, the seat becomes empty.
//     Otherwise, the seat's state does not change.

// Floor (.) never changes; seats don't move, and nobody sits on the floor.

// After one round of these rules, every seat in the example layout becomes occupied:

// #.##.##.##
// #######.##
// #.#.#..#..
// ####.##.##
// #.##.##.##
// #.#####.##
// ..#.#.....
// ##########
// #.######.#
// #.#####.##

// After a second round, the seats with four or more occupied adjacent seats become empty again:

// #.LL.L#.##
// #LLLLLL.L#
// L.L.L..L..
// #LLL.LL.L#
// #.LL.LL.LL
// #.LLLL#.##
// ..L.L.....
// #LLLLLLLL#
// #.LLLLLL.L
// #.#LLLL.##

// This process continues for three more rounds:

// #.##.L#.##
// #L###LL.L#
// L.#.#..#..
// #L##.##.L#
// #.##.LL.LL
// #.###L#.##
// ..#.#.....
// #L######L#
// #.LL###L.L
// #.#L###.##

// #.#L.L#.##
// #LLL#LL.L#
// L.L.L..#..
// #LLL.##.L#
// #.LL.LL.LL
// #.LL#L#.##
// ..L.L.....
// #L#LLLL#L#
// #.LLLLLL.L
// #.#L#L#.##

// #.#L.L#.##
// #LLL#LL.L#
// L.#.L..#..
// #L##.##.L#
// #.#L.LL.LL
// #.#L#L#.##
// ..L.L.....
// #L#L##L#L#
// #.LLLLLL.L
// #.#L#L#.##

// At this point, something interesting happens: the chaos stabilizes and further applications of these rules cause no seats to change state! Once people stop moving around, you count 37 occupied seats.

// Simulate your seating area by applying the seating rules repeatedly until no seats change state. How many seats end up occupied?

module Solution =
    let puzzleInput = 
        System.IO.File.ReadAllLines("./Puzzle11Input.txt")
        |> Array.toList
        |> List.map (fun str -> 
            str.ToCharArray()
            |> Array.toList) 

    type State = 
        |Nothing
        |EmptySeat
        |OccupiedSeat

    type Coordinate =
        {X: int
         Y: int}

    let rec mapLineToCoordinateAndSeat (index,row) seatsSoFar input =
        match input with
        |[] -> seatsSoFar
        |head::tail -> 
            (
                {X = index; Y = row},
                match head with
                |'.' -> Nothing
                |'L' -> EmptySeat
                |'#' -> OccupiedSeat
                |_ -> failwithf "Illegal state found!")
                |> fun seat -> mapLineToCoordinateAndSeat (index + 1, row) (seat::seatsSoFar) tail

    let mapLineListToCoordinateAndSeatList input =
        input
        |> List.mapi (fun row line -> mapLineToCoordinateAndSeat (0,row) [] line)
        |> List.concat
    
    let inline mapListToCoordinateAndSeatMap list =
        list
        |> Map.ofList

    let findAdjacentOccupiedSeats (map:Map<Coordinate,State>) (coordinate:Coordinate) =
        let left = {coordinate with X = coordinate.X - 1}
        let right = {coordinate with X = coordinate.X + 1}
        let top = {coordinate with Y = coordinate.Y - 1}
        let bottom = {coordinate with Y = coordinate.Y + 1}
        let topLeft = {coordinate with X = coordinate.X - 1; Y = coordinate.Y - 1}
        let topRight = {coordinate with X = coordinate.X + 1; Y = coordinate.Y - 1}
        let bottomLeft = {coordinate with X = coordinate.X - 1; Y = coordinate.Y + 1}
        let bottomRight = {coordinate with X = coordinate.X + 1; Y = coordinate.Y + 1}
        [topLeft;top;topRight;right;bottomRight;bottom;bottomLeft;left]
        |> List.map (fun coor -> map.TryGetValue coor)
        |> List.map (fun (exists,state) -> if exists then Some state else None)
        |> List.choose id
        |> List.countBy id
        |> List.tryFind (fun (state,_) -> state = OccupiedSeat)
        |> function 
            |Some (_,num) -> 
                // printfn "number found: %i for coordinate %i,%i" num coordinate.X coordinate.Y
                coordinate, num
            |None -> coordinate, 0
    
    let updateSeatMapWithList (seatMap:Map<Coordinate,State>) coordinateAndNumberOfSeatsList =
        let rec updateSeatMap (referenceSeatMap:Map<Coordinate,State>) updatedSeatList coordinateAndNumberOfSeatsList =
            match coordinateAndNumberOfSeatsList with
            |head::tail -> 
                let ((coordinate:Coordinate),numberOfSeats) = head
                match referenceSeatMap.Item coordinate, numberOfSeats with 
                |Nothing, _ -> updateSeatMap referenceSeatMap ((coordinate,Nothing)::updatedSeatList) tail
                |EmptySeat, 0 -> updateSeatMap referenceSeatMap ((coordinate,OccupiedSeat)::updatedSeatList) tail
                |EmptySeat, _ -> updateSeatMap referenceSeatMap ((coordinate,EmptySeat)::updatedSeatList) tail
                |OccupiedSeat, num when num >= 4 -> updateSeatMap referenceSeatMap ((coordinate, EmptySeat)::updatedSeatList) tail
                |OccupiedSeat, num when num < 4 -> updateSeatMap referenceSeatMap ((coordinate, OccupiedSeat)::updatedSeatList) tail
                |e -> failwithf "unexpected %s" (e.ToString())
            |[] -> updatedSeatList, mapListToCoordinateAndSeatMap updatedSeatList
        updateSeatMap seatMap List.empty coordinateAndNumberOfSeatsList

    let changeSeats (seatList:(Coordinate * State) list) seatMap =
        seatList
        |> List.map fst
        |> List.map (findAdjacentOccupiedSeats seatMap)
        |> updateSeatMapWithList seatMap

    let rec checkLists num (referenceList:(Coordinate * State) list) (newlist,newSeatmap) =
        if (newlist |> List.sort) = (referenceList |> List.sort) || num > 9000
        then 
            printfn "number of rounds: %i" num
            newlist
        else checkLists (num + 1) newlist (changeSeats newlist newSeatmap)

    let test = 
        
            ["L.LL.LL.LL";
            "LLLLLLL.LL";
            "L.L.L..L..";
            "LLLL.LL.LL";
            "L.LL.LL.LL";
            "L.LLLLL.LL";
            "..L.L.....";
            "LLLLLLLLLL";
            "L.LLLLLL.L";
            "L.LLLLL.LL"]
            |> List.map (fun str -> 
            str.ToCharArray()
            |> Array.toList) 
        
    let solution = 
        checkLists 0 List.empty (mapLineListToCoordinateAndSeatList puzzleInput,mapListToCoordinateAndSeatMap (mapLineListToCoordinateAndSeatList puzzleInput))
        |> List.map snd
        |> List.countBy id
