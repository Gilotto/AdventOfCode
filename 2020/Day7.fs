namespace AdventOfCode

module excercise1 =
    let text=""
    //You land at the regional airport in time for your next flight. In fact, it looks like you'll even have time to grab some food: all flights are currently delayed due to issues in luggage processing.

    // Due to recent aviation regulations, many rules (your puzzle input) are being enforced about bags and their contents; bags must be color-coded and must contain specific quantities of other color-coded bags. Apparently, nobody responsible for these regulations considered how long they would take to enforce!

    // For example, consider the following rules:

    // light red bags contain 1 bright white bag, 2 muted yellow bags.
    // dark orange bags contain 3 bright white bags, 4 muted yellow bags.
    // bright white bags contain 1 shiny gold bag.
    // muted yellow bags contain 2 shiny gold bags, 9 faded blue bags.
    // shiny gold bags contain 1 dark olive bag, 2 vibrant plum bags.
    // dark olive bags contain 3 faded blue bags, 4 dotted black bags.
    // vibrant plum bags contain 5 faded blue bags, 6 dotted black bags.
    // faded blue bags contain no other bags.
    // dotted black bags contain no other bags.

    // These rules specify the required contents for 9 bag types. In this example, every faded blue bag is empty, every vibrant plum bag contains 11 bags (5 faded blue and 6 dotted black), and so on.

    // You have a shiny gold bag. If you wanted to carry it in at least one other bag, how many different bag colors would be valid for the outermost bag? (In other words: how many colors can, eventually, contain at least one shiny gold bag?)

    // In the above rules, the following options would be available to you:

    //     A bright white bag, which can hold your shiny gold bag directly.
    //     A muted yellow bag, which can hold your shiny gold bag directly, plus some other bags.
    //     A dark orange bag, which can hold bright white and muted yellow bags, either of which could then hold your shiny gold bag.
    //     A light red bag, which can hold bright white and muted yellow bags, either of which could then hold your shiny gold bag.

    // So, in this example, the number of bag colors that can eventually contain at least one shiny gold bag is 4.

    // How many bag colors can eventually contain at least one shiny gold bag? (The list of rules is quite long; make sure you get all of it.)
    
    // --- Part Two ---
    // It's getting pretty expensive to fly these days - not because of ticket prices, but because of the ridiculous number of bags you need to buy!

    // Consider again your shiny gold bag and the rules from the above example:

    //     faded blue bags contain 0 other bags.
    //     dotted black bags contain 0 other bags.
    //     vibrant plum bags contain 11 other bags: 5 faded blue bags and 6 dotted black bags.
    //     dark olive bags contain 7 other bags: 3 faded blue bags and 4 dotted black bags.

    // So, a single shiny gold bag must contain 1 dark olive bag (and the 7 bags within it) plus 2 vibrant plum bags (and the 11 bags within each of those): 1 + 1*7 + 2 + 2*11 = 32 bags!

    // Of course, the actual rules have a small chance of going several levels deeper than this example; be sure to count all of the bags, even if the nesting becomes topologically impractical!

    // Here's another example:

    // shiny gold bags contain 2 dark red bags.
    // dark red bags contain 2 dark orange bags.
    // dark orange bags contain 2 dark yellow bags.
    // dark yellow bags contain 2 dark green bags.
    // dark green bags contain 2 dark blue bags.
    // dark blue bags contain 2 dark violet bags.
    // dark violet bags contain no other bags.

    // In this example, a single shiny gold bag must contain 126 other bags.

    // How many individual bags are required inside your single shiny gold bag?

module Solution1 =
    let puzzleInput () = 
        System.IO.File.ReadAllLines("./Puzzle7Input.txt")
        |> List.ofArray
    
    type Bag = 
        {
            name :string
            contents :(int * Bag) list
        }

    let splitLine (sep:string) (line:string)  =
        line.Split (sep)
        |> List.ofArray
    
    let mapContentLineToBag (content:string) =
        if content.Contains "." then content.Replace(".","") else content
        |> splitLine " "
        |> fun numberAndName -> (int numberAndName.Head, {name = numberAndName.[1] + " " + numberAndName.[2]; contents = []} )
        
    let mapInputLineToBag (inputLine:string) =
        let (name,contents) = 
            inputLine 
            |> splitLine " contain " //split into bag Name - bag contents
            |> fun lineList -> lineList.Head, lineList.Item 1
        let bagName = name.Replace(" bags","")
        let bagContents (str:string) = 
            match str with
            |"" 
            |"no other bags." -> []
            |str when str.Contains(",") -> 
                str
                |> splitLine ", " 
                |> List.map (fun bagLine -> mapContentLineToBag bagLine)
            |str -> mapContentLineToBag str |> List.singleton
                
        {name = bagName; contents = bagContents contents}

    let mapStringListToBagList stringList =
        let rec mapBags (listOfBags:Bag list) (input:string list) =
            match input with
            |str::tail -> mapBags ((str |> mapInputLineToBag)::listOfBags) tail
            |[] -> listOfBags
        mapBags [] stringList

    let fillBagsWithBags bagList =
        let replaceBagInListWithBag  (bag:Bag) (bagInList:Bag) =
            bagInList.contents
            |> List.map (
                fun (number,bagsInBag) -> 
                    match bagsInBag.name with
                    |name when name = bag.name -> number, bag
                    |_ -> number, bagsInBag)
            |> fun newContents -> {bagInList with contents = newContents}
        let rec findBagContents (bagList:Bag list) (reference:Bag list) =
            match reference with
            |head::tail -> findBagContents (bagList |> List.map (replaceBagInListWithBag head)) tail 
            |[] -> bagList
        
        let rec checkListIsSame (list1:Bag list) list2 =
            match list1 with
            |list1 when list1 = list2 -> list1
            |_ -> checkListIsSame list2 (findBagContents list2 list2)
        
        checkListIsSame bagList (findBagContents bagList bagList)

    let getName bagName =
        bagName.contents
        |> List.map (fun (_,name) -> name)

    let getAllBagNamesInBag (bag:Bag) = 
        let rec findBagInBag (bagsSoFar:string list) (bagList:Bag list) =
            match bagList with
            |[] -> bagsSoFar |> List.rev
            |head::tail -> findBagInBag (head.name::bagsSoFar) ((getName head)@tail)
        findBagInBag [] [bag]
    
    let getBagNumbers bag =
        let rec calculateBagContents (countedSoFar:int) (numBag:(int*Bag)) = 
            let (numberOfBags,bag) = numBag
            match bag.contents with
            |[] -> numberOfBags + countedSoFar
            |stuffInside -> 
                stuffInside 
                |> List.map (fun (num,bags) -> calculateBagContents countedSoFar (numberOfBags * num, bags))
                |> List.reduce (+)
                |> fun bagsInbag -> numberOfBags + bagsInbag
        calculateBagContents 0 (1,bag) - 1
        
    let solution1 = 
        puzzleInput ()
        |> mapStringListToBagList
        |> fillBagsWithBags
        |> List.map getAllBagNamesInBag
        |> List.map (fun list -> list.Tail |> List.tryFind (fun item -> item = "shiny gold"))
        |> List.countBy id

    let solution2 =
        puzzleInput ()
        |> mapStringListToBagList
        |> fillBagsWithBags
        |> List.find (fun bagName -> bagName.name = "shiny gold")
        |> getBagNumbers