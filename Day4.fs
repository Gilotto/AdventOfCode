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

    let puzzleInput = 
        System.IO.File.ReadAllLines("./Puzzle4Input.txt")

    let findCharinString (index, mapString:string) =
        mapString.[(index - 1) % mapString.Length]
    
    let filterVertical slopeVertical input=
        input
        |> Array.indexed
        |> Array.filter (fun (index, mapstring) -> (index % slopeVertical) = 0)
        |> Array.map (fun (_,relevantMapstring) -> relevantMapstring)

    let solution1 input slopeHorizontal slopeVertical = 
        input
        |> filterVertical slopeVertical
        |> Array.indexed
        |> Array.map (fun (index,mapString) -> 1 + (index * slopeHorizontal), mapString) //index 0 corresponds to char 1, index 1 corresponds to char 1 + 1*3 =4 etc.
        |> Array.map findCharinString
        |> Array.map (fun char -> if char = '#' then true else false)
        |> Array.countBy id
        |> Array.filter (fun (yesOrNo , _) -> yesOrNo)
        |> fun array -> array.[0] |> fun ( _ , number) -> number
        |> fun answer -> 
            printfn "Answer = %i; for slope Hor: %i; Ver: %i " answer slopeHorizontal slopeVertical
            answer

    solution1 puzzleInput 3 1 |> ignore
