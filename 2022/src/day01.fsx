#load "./parser.fsx"

open System.IO
open AdventOfCode.Parser

let day01_part1 = "--- Day 1: Calorie Counting ---

Santa's reindeer typically eat regular reindeer food, but they need a lot of magical energy to deliver presents on Christmas. For that, their favorite snack is a special type of star fruit that only grows deep in the jungle. The Elves have brought you on their annual expedition to the grove where the fruit grows.

To supply enough magical energy, the expedition needs to retrieve a minimum of fifty stars by December 25th. Although the Elves assure you that the grove has plenty of fruit, you decide to grab any fruit you see along the way, just in case.

Collect stars by solving puzzles. Two puzzles will be made available on each day in the Advent calendar; the second puzzle is unlocked when you complete the first. Each puzzle grants one star. Good luck!

The jungle must be too overgrown and difficult to navigate in vehicles or access from the air; the Elves' expedition traditionally goes on foot. As your boats approach land, the Elves begin taking inventory of their supplies. One important consideration is food - in particular, the number of Calories each Elf is carrying (your puzzle input).

The Elves take turns writing down the number of Calories contained by the various meals, snacks, rations, etc. that they've brought with them, one item per line. Each Elf separates their own inventory from the previous Elf's inventory (if any) by a blank line.

For example, suppose the Elves finish writing their items' Calories and end up with the following list:"

let data = File.ReadAllLines "../AdventOfCode/2022/input/day01.txt"

//part 1
data
|> Array.toList
|> List.fold (
        fun state str -> 
        match str with
        |"" -> 0::state //add new elf
        |n -> 
            match state with
            |[] -> int n::state // if first elf, init counting
            |h::t -> h + int n :: t // add number to current elf
        ) []
|> List.max

//part 2
let day01_part2 = "--- Part Two ---

By the time you calculate the answer to the Elves' question, they've already realized that the Elf carrying the most Calories of food might eventually run out of snacks.

To avoid this unacceptable situation, the Elves would instead like to know the total Calories carried by the top three Elves carrying the most Calories. That way, even if one of those Elves runs out of snacks, they still have two backups.

In the example above, the top three Elves are the fourth Elf (with 24000 Calories), then the third Elf (with 11000 Calories), then the fifth Elf (with 10000 Calories). The sum of the Calories carried by these three elves is 45000.

Find the top three Elves carrying the most Calories. How many Calories are those Elves carrying in total?
"
data
|> Array.toList
|> List.fold (
        fun state str -> 
        match str with
        |"" -> 0::state //add new elf
        |n -> 
            match state with
            |[] -> int n::state // if first elf, init counting
            |h::t -> h + int n :: t // add number to current elf
        ) []
|> List.sortDescending
|> List.take 3
|> List.sum