namespace AdventOfCode

module Day04 =

// --- Day 4: Giant Squid ---

// You're already almost 1.5km (almost a mile) below the surface of the ocean, already so deep that you can't see any sunlight. What you can see, however, is a giant squid that has attached itself to the outside of your submarine.

// Maybe it wants to play bingo?

// Bingo is played on a set of boards each consisting of a 5x5 grid of numbers. Numbers are chosen at random, and the chosen number is marked on all boards on which it appears. (Numbers may not appear on all boards.) If all numbers in any row or any column of a board are marked, that board wins. (Diagonals don't count.)

// The submarine has a bingo subsystem to help passengers (currently, you and the giant squid) pass the time. It automatically generates a random order in which to draw numbers and a random set of boards (your puzzle input). For example:

// 7,4,9,5,11,17,23,2,0,14,21,24,10,16,13,6,15,25,12,22,18,20,8,19,3,26,1

// 22 13 17 11  0
//  8  2 23  4 24
// 21  9 14 16  7
//  6 10  3 18  5
//  1 12 20 15 19

//  3 15  0  2 22
//  9 18 13 17  5
// 19  8  7 25 23
// 20 11 10 24  4
// 14 21 16 12  6

// 14 21 17 24  4
// 10 16 15  9 19
// 18  8 23 26 20
// 22 11 13  6  5
//  2  0 12  3  7

// After the first five numbers are drawn (7, 4, 9, 5, and 11), there are no winners, but the boards are marked as follows (shown here adjacent to each other to save space):

// 22 13 17 11  0         3 15  0  2 22        14 21 17 24  4
//  8  2 23  4 24         9 18 13 17  5        10 16 15  9 19
// 21  9 14 16  7        19  8  7 25 23        18  8 23 26 20
//  6 10  3 18  5        20 11 10 24  4        22 11 13  6  5
//  1 12 20 15 19        14 21 16 12  6         2  0 12  3  7

// After the next six numbers are drawn (17, 23, 2, 0, 14, and 21), there are still no winners:

// 22 13 17 11  0         3 15  0  2 22        14 21 17 24  4
//  8  2 23  4 24         9 18 13 17  5        10 16 15  9 19
// 21  9 14 16  7        19  8  7 25 23        18  8 23 26 20
//  6 10  3 18  5        20 11 10 24  4        22 11 13  6  5
//  1 12 20 15 19        14 21 16 12  6         2  0 12  3  7

// Finally, 24 is drawn:

// 22 13 17 11  0         3 15  0  2 22        14 21 17 24  4
//  8  2 23  4 24         9 18 13 17  5        10 16 15  9 19
// 21  9 14 16  7        19  8  7 25 23        18  8 23 26 20
//  6 10  3 18  5        20 11 10 24  4        22 11 13  6  5
//  1 12 20 15 19        14 21 16 12  6         2  0 12  3  7

// At this point, the third board wins because it has at least one complete row or column of marked numbers (in this case, the entire top row is marked: 14 21 17 24 4).

// The score of the winning board can now be calculated. Start by finding the sum of all unmarked numbers on that board; in this case, the sum is 188. Then, multiply that sum by the number that was just called when the board won, 24, to get the final score, 188 * 24 = 4512.

// To guarantee victory against the giant squid, figure out which board will win first. What will your final score be if you choose that board?
    
    open Parser

    type ColumnNum = int
    type RowNum = int
    type Location = ColumnNum * RowNum
    type BingoNumber = int
    type BingoCard =
        {
            fields : (Location * BingoNumber option) list
        }
        member this.removeNumber numberToMatch =
            let newFields = 
                this.fields
                |> List.map (fun (location,bingoNumber) -> 
                    match bingoNumber with
                    |Some number when number = numberToMatch -> location,None
                    |_ -> location,bingoNumber)
            {this with fields = newFields}

        member this.column number = 
            this.fields
            |> List.filter (fun ((_,colNum),_) -> colNum = number)
            |> List.choose snd
        
        member this.row number =
            this.fields
            |> List.filter (fun ((rowNum,_),_) -> rowNum = number)
            |> List.choose snd
        
        member this.sumOfCard = 
            this.fields 
            |> List.sumBy (
                fun (_,bingoNumberOpt) -> 
                    match bingoNumberOpt with
                    |Some number -> number
                    |None -> 0)

        static member init (listOfrows:BingoNumber list list) =
            {
                fields = 
                    listOfrows
                    |> List.mapi ( fun rowNum listofNumbers -> 
                        listofNumbers 
                        |> List.mapi ( fun colNum bingonumber -> 
                            let location = (rowNum + 1, colNum + 1)
                            location, Some bingonumber ))
                    |> List.concat
            }
    type BingoGame =
        {
            bingoNumbersToDraw : BingoNumber list
            cards : BingoCard list
        }

    let checkIfWinner bingoCards =
        let rowsAndColumns (card:BingoCard) = 
            ([1..5] |> List.map card.row) @ ([1..5] |> List.map card.column)
            
        bingoCards
        |> List.map rowsAndColumns
        |> List.map (
            fun card -> 
                card 
                |> List.map (fun rowOrColumn -> List.isEmpty rowOrColumn)
                |> List.contains true
        )
        |> List.zip bingoCards

    let stopOnFirstBingo (checkedCards:(BingoCard*bool)list) =
        checkedCards
        |> List.choose (fun (card,isBingo) -> if isBingo then card.sumOfCard |> Some else None)
        |> List.tryHead
        |> fun opt -> opt, checkedCards |> List.map fst

    let lastCardWithBingo (checkedCards:(BingoCard*bool)list) =
        match checkedCards with
        |[] -> None, []
        |[lastCard,isBingo] -> 
            if isBingo
            then lastCard.sumOfCard |> Some, [lastCard]
            else None, [lastCard]
        |cards -> None, cards |> List.filter (fun (_,isbingo) -> not isbingo) |> List.map fst
    
    let rec playBingo winningCondition bingoGame =
        match bingoGame.bingoNumbersToDraw with
        |[] -> failwith "game ended without winner"
        |numberDrawn::restOfNumbers ->
            let updateCards =
                bingoGame.cards
                |> List.map (fun card -> card.removeNumber numberDrawn)

            checkIfWinner updateCards
            |> winningCondition
            |> function
                |Some value, _ -> value * numberDrawn
                |None, cards -> playBingo winningCondition {bingoNumbersToDraw = restOfNumbers; cards = cards}


    let parserSettings =

        let space = pchar ' '
    
        let bingoRowParser = 
            opt space >>. pint .>> many1 space .>>. pint .>> many1 space .>>. pint .>> many1 space .>>. pint .>> many1 space .>>. pint
            |>> fun ((((i1,i2),i3),i4),i5) -> [i1;i2;i3;i4;i5]
        
        let bingoNumbersParser =
            pint .>> (opt (pchar ',')) |> many1
        
        [bingoRowParser;bingoNumbersParser]
        |> choice

    let inputLocation = "./input/day04.txt"
    let testInput =
        [
            "7,4,9,5,11,17,23,2,0,14,21,24,10,16,13,6,15,25,12,22,18,20,8,19,3,26,1"
            ""
            "22 13 17 11  0"
            " 8  2 23  4 24"
            "21  9 14 16  7"
            " 6 10  3 18  5"
            " 1 12 20 15 19"
            ""
            " 3 15  0  2 22"
            " 9 18 13 17  5"
            "19  8  7 25 23"
            "20 11 10 24  4"
            "14 21 16 12  6"
            ""
            "14 21 17 24  4"
            "10 16 15  9 19"
            "18  8 23 26 20"
            "22 11 13  6  5"
            " 2  0 12  3  7"
        ]

    let testsolution =
        let giveParseResult = function
            |Success (numbers,_) -> Some numbers
            |Failure err -> None

        testInput
        |> List.map (run parserSettings)
        |> function
            | [] -> failwith "No input parsed"
            | (parsedNumbers::parsedCardRows) -> 
                {
                    bingoNumbersToDraw = 
                        match giveParseResult parsedNumbers with
                        |Some numbers -> numbers
                        |None -> failwith "No inputnumbers parsed"

                    cards = 
                        parsedCardRows
                        |> List.choose giveParseResult
                        |> List.chunkBySize 5
                        |> List.map (fun cardcolumns -> 
                            BingoCard.init cardcolumns)
                }
        |> playBingo (stopOnFirstBingo )
        |> sprintf "The answer is: %i"

    let solutionPart1 =
        let giveParseResult = function
            |Success (numbers,_) -> Some numbers
            |Failure err -> None

        IO.readInput inputLocation
        |> List.map (run parserSettings)
        |> function
            | [] -> failwith "No input parsed"
            | (parsedNumbers::parsedCardRows) -> 
                {
                    bingoNumbersToDraw = 
                        match giveParseResult parsedNumbers with
                        |Some numbers -> numbers
                        |None -> failwith "No inputnumbers parsed"

                    cards = 
                        parsedCardRows
                        |> List.choose giveParseResult
                        |> List.chunkBySize 5
                        |> List.map (fun cardcolumns -> 
                            BingoCard.init cardcolumns)
                }
        |> playBingo stopOnFirstBingo
        |> sprintf "The answer is: %i"

// --- Part Two ---

// On the other hand, it might be wise to try a different strategy: let the giant squid win.

// You aren't sure how many bingo boards a giant squid could play at once, so rather than waste time counting its arms, the safe thing to do is to figure out which board will win last and choose that one. That way, no matter which boards it picks, it will win for sure.

// In the above example, the second board is the last to win, which happens after 13 is eventually called and its middle column is completely marked. If you were to keep playing until this point, the second board would have a sum of unmarked numbers equal to 148 for a final score of 148 * 13 = 1924.

// Figure out which board will win last. Once it wins, what would its final score be?

    let solutionPart2 =
        let giveParseResult = function
            |Success (numbers,_) -> Some numbers
            |Failure err -> None

        IO.readInput inputLocation
        |> List.map (run parserSettings)
        |> function
            | [] -> failwith "No input parsed"
            | (parsedNumbers::parsedCardRows) -> 
                {
                    bingoNumbersToDraw = 
                        match giveParseResult parsedNumbers with
                        |Some numbers -> numbers
                        |None -> failwith "No inputnumbers parsed"

                    cards = 
                        parsedCardRows
                        |> List.choose giveParseResult
                        |> List.chunkBySize 5
                        |> List.map (fun cardcolumns -> 
                            BingoCard.init cardcolumns)
                }
        |> playBingo lastCardWithBingo
        |> sprintf "The answer is: %i"