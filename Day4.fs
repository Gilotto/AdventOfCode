namespace AdventOfCode

module Excercise1 = 
    let text =
        """
--- Day 4: Passport Processing ---

You arrive at the airport only to realize that you grabbed your North Pole Credentials instead of your passport. While these documents are extremely similar, North Pole Credentials aren't issued by a country and therefore aren't actually valid documentation for travel in most of the world.

It seems like you're not the only one having problems, though; a very long line has formed for the automatic passport scanners, and the delay could upset your travel itinerary.

Due to some questionable network security, you realize you might be able to solve both of these problems at the same time.

The automatic passport scanners are slow because they're having trouble detecting which passports have all required fields. The expected fields are as follows:

    byr (Birth Year)
    iyr (Issue Year)
    eyr (Expiration Year)
    hgt (Height)
    hcl (Hair Color)
    ecl (Eye Color)
    pid (Passport ID)
    cid (Country ID)

Passport data is validated in batch files (your puzzle input). Each passport is represented as a sequence of key:value pairs separated by spaces or newlines. Passports are separated by blank lines.

Here is an example batch file containing four passports:

ecl:gry pid:860033327 eyr:2020 hcl:#fffffd
byr:1937 iyr:2017 cid:147 hgt:183cm

iyr:2013 ecl:amb cid:350 eyr:2023 pid:028048884
hcl:#cfa07d byr:1929

hcl:#ae17e1 iyr:2013
eyr:2024
ecl:brn pid:760753108 byr:1931
hgt:179cm

hcl:#cfa07d eyr:2025 pid:166559648
iyr:2011 ecl:brn hgt:59in

The first passport is valid - all eight fields are present. The second passport is invalid - it is missing hgt (the Height field).

The third passport is interesting; the only missing field is cid, so it looks like data from North Pole Credentials, not a passport at all! Surely, nobody would mind if you made the system temporarily ignore missing cid fields. Treat this "passport" as valid.

The fourth passport is missing two fields, cid and byr. Missing cid is fine, but missing any other field is not, so this passport is invalid.

According to the above rules, your improved system would report 2 valid passports.

Count the number of valid passports - those that have all required fields. Treat cid as optional. In your batch file, how many passports are valid?

        """

module Solution1 = 

    let inline tryParseInt a =
        try
            a |> int |> Some
        with 
            | :? System.FormatException -> 
                printfn "format exception %A" a
                Some 1
            | :? System.OverflowException -> 
                printfn "overflow exception %A" a
                Some 1
                
    let inline tryParseDouble a =
        try
            a |> double |> Some
        with 
            | :? System.FormatException -> 
                printfn "format exception %A" a
                Some 1.
            | :? System.OverflowException -> 
                printfn "overflow exception %A" a
                Some 1.

    type ValidatedPassport =
        {
            byr :int
            iyr :int
            eyr :int
            hgt :string
            hcl :string
            ecl :string
            pid :double
            cid :int option
        }  

    type UnvalidatedPassport =
        {
            byr :int option
            iyr :int option
            eyr :int option
            hgt :string option
            hcl :string option
            ecl :string option
            pid :double option
            cid :int option
        }
        static member Create = 
            {
                byr = None
                iyr = None
                eyr = None
                hgt = None
                hcl = None
                ecl = None
                pid = None
                cid = None
            }

    let puzzleInput = 
        System.IO.File.ReadAllLines("./Puzzle4Input.txt")
        |> List.ofArray

    let parseLine sep (line:string)  =
        line.Split (sep)
        |> List.ofArray

    let convertInputStringToPasswordField (inputString:string) (passport:UnvalidatedPassport) =

        let matchIdentifierToPassportField (passportToAddInfo:UnvalidatedPassport) (fieldString:string)  =
            let (identifier,value) = 
                match fieldString with
                |string when string.Contains (':') -> fieldString |> parseLine ([|':'|]) |> fun array -> array.[0], array.[1]
                |_ -> "", ""
            match identifier with 
            |"byr" -> {passportToAddInfo with byr = tryParseInt value} 
            |"iyr" -> {passportToAddInfo with iyr = tryParseInt value} 
            |"eyr" -> {passportToAddInfo with eyr = tryParseInt value} 
            |"hgt" -> {passportToAddInfo with hgt = Some value} 
            |"hcl" -> {passportToAddInfo with hcl = Some value} 
            |"ecl" -> {passportToAddInfo with ecl = Some value} 
            |"pid" -> {passportToAddInfo with pid = tryParseDouble value} 
            |"cid" -> {passportToAddInfo with cid = tryParseInt value} 
            | _ -> passportToAddInfo
        inputString 
        |> parseLine [|' '|]
        |> List.fold matchIdentifierToPassportField passport

    let convertinputToPassword inputStringList =
        let rec parseInput inputList listOfUnvalidatedPassports passportSoFar  =
            match inputList with
            |inputString::tail when inputString = ""    -> parseInput tail (passportSoFar::listOfUnvalidatedPassports) UnvalidatedPassport.Create 
            |inputString::tail                          -> parseInput tail listOfUnvalidatedPassports (convertInputStringToPasswordField inputString passportSoFar)
            |[] -> passportSoFar::listOfUnvalidatedPassports

        parseInput inputStringList List.empty UnvalidatedPassport.Create

    let checkPassport passport :ValidatedPassport option=
        match passport.byr,passport.iyr,passport.eyr,passport.hgt,passport.hcl,passport.ecl,passport.pid,passport.cid with
        |(Some birthYear), 
         (Some issueYear), 
         (Some exYear), 
         (Some height),
         (Some hairColor),
         (Some eyeColor),
         (Some passID),
         (Some countryID) -> 
            {
                ValidatedPassport.byr = birthYear
                ValidatedPassport.iyr = issueYear
                ValidatedPassport.eyr = exYear
                ValidatedPassport.hgt = height
                ValidatedPassport.hcl = hairColor
                ValidatedPassport.ecl = eyeColor
                ValidatedPassport.pid = passID
                ValidatedPassport.cid = countryID |> Some
            } |> Some //Valid Passport
        |(Some birthYear), 
         (Some issueYear), 
         (Some exYear), 
         (Some height),
         (Some hairColor),
         (Some eyeColor),
         (Some passID),
         (None) -> 
            {
                ValidatedPassport.byr = birthYear
                ValidatedPassport.iyr = issueYear
                ValidatedPassport.eyr = exYear
                ValidatedPassport.hgt = height
                ValidatedPassport.hcl = hairColor
                ValidatedPassport.ecl = eyeColor
                ValidatedPassport.pid = passID
                ValidatedPassport.cid = None
            } |> Some //Hacked valid Passport
        | _ , _ , _ , _ , _ , _ , _, _ -> None //Invalid Passport

    let solution1 =  
        puzzleInput
        |> convertinputToPassword
        |> List.map checkPassport
        |> List.countBy (fun passport -> passport.IsSome)