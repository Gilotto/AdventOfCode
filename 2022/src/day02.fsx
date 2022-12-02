let day02_part1 = """--- Day 2: Rock Paper Scissors ---

The Elves begin to set up camp on the beach. To decide whose tent gets to be closest to the snack storage, a giant Rock Paper Scissors tournament is already in progress.

Rock Paper Scissors is a game between two players. Each game contains many rounds; in each round, the players each simultaneously choose one of Rock, Paper, or Scissors using a hand shape. Then, a winner for that round is selected: Rock defeats Scissors, Scissors defeats Paper, and Paper defeats Rock. If both players choose the same shape, the round instead ends in a draw.

Appreciative of your help yesterday, one Elf gives you an encrypted strategy guide (your puzzle input) that they say will be sure to help you win. "The first column is what your opponent is going to play: A for Rock, B for Paper, and C for Scissors. The second column--" Suddenly, the Elf is called away to help with someone's tent.

The second column, you reason, must be what you should play in response: X for Rock, Y for Paper, and Z for Scissors. Winning every time would be suspicious, so the responses must have been carefully chosen.

The winner of the whole tournament is the player with the highest score. Your total score is the sum of your scores for each round. The score for a single round is the score for the shape you selected (1 for Rock, 2 for Paper, and 3 for Scissors) plus the score for the outcome of the round (0 if you lost, 3 if the round was a draw, and 6 if you won).

Since you can't be sure if the Elf is trying to help you or trick you, you should calculate the score you would get if you were to follow the strategy guide.

For example, suppose you were given the following strategy guide:

A Y
B X
C Z

This strategy guide predicts and recommends the following:

    In the first round, your opponent will choose Rock (A), and you should choose Paper (Y). This ends in a win for you with a score of 8 (2 because you chose Paper + 6 because you won).
    In the second round, your opponent will choose Paper (B), and you should choose Rock (X). This ends in a loss for you with a score of 1 (1 + 0).
    The third round is a draw with both players choosing Scissors, giving you a score of 3 + 3 = 6.

In this example, if you were to follow the strategy guide, you would get a total score of 15 (8 + 1 + 6).

What would your total score be if everything goes exactly according to your strategy guide?
"""

let data = System.IO.File.ReadAllLines "../AdventOfCode/2022/input/day02.txt"

data
|> Array.toList
|> List.fold (
    fun countSoFar s ->
        match s.[0],s.[2] with
        |'A','X' -> countSoFar + 4//Rock , Rock - draw (1 + 3) 
        |'A','Y' -> countSoFar + 8//Rock , Paper - win (2 + 6)
        |'A','Z' -> countSoFar + 3//Rock , Scissors - lose (3 + 0)
        |'B','X' -> countSoFar + 1//Paper , Rock - lose (1 + 0)
        |'B','Y' -> countSoFar + 5//Paper , Paper - draw (2 + 3)
        |'B','Z' -> countSoFar + 9//Paper , Scissors - win (3 + 6)
        |'C','X' -> countSoFar + 7//Scissors , Rock - win (1 + 6)
        |'C','Y' -> countSoFar + 2//Scissors , Paper - lose (2 + 0)
        |'C','Z' -> countSoFar + 6//Scissors , Scissors - draw (3 + 3)
        |c1,c2 -> failwithf "combination not found: %c %c" c1 c2
) 0

let day02_part2 = """--- Part Two ---

The Elf finishes helping with the tent and sneaks back over to you. "Anyway, the second column says how the round needs to end: X means you need to lose, Y means you need to end the round in a draw, and Z means you need to win. Good luck!"

The total score is still calculated in the same way, but now you need to figure out what shape to choose so the round ends as indicated. The example above now goes like this:

    In the first round, your opponent will choose Rock (A), and you need the round to end in a draw (Y), so you also choose Rock. This gives you a score of 1 + 3 = 4.
    In the second round, your opponent will choose Paper (B), and you choose Rock so you lose (X) with a score of 1 + 0 = 1.
    In the third round, you will defeat your opponent's Scissors with Rock for a score of 1 + 6 = 7.

Now that you're correctly decrypting the ultra top secret strategy guide, you would get a total score of 12."""

data
|> Array.toList
|> List.fold (
    fun countSoFar s ->
        match s.[0],s.[2] with
        |'A','X' -> countSoFar + 3//Rock , lose - Scissors (0 + 3) 
        |'A','Y' -> countSoFar + 4//Rock , draw - Rock (3 + 1)
        |'A','Z' -> countSoFar + 8//Rock , win - Paper (6 + 2)
        |'B','X' -> countSoFar + 1//Paper , lose - Rock (0 + 1)
        |'B','Y' -> countSoFar + 5//Paper , draw - Paper (3 + 2)
        |'B','Z' -> countSoFar + 9//Paper , win - Scissors (6 + 3)
        |'C','X' -> countSoFar + 2//Scissors , lose - Paper (0 + 2)
        |'C','Y' -> countSoFar + 6//Scissors , draw - Scissors (3 + 3)
        |'C','Z' -> countSoFar + 7//Scissors , win - Rock (6 + 1)
        |c1,c2 -> failwithf "combination not found: %c %c" c1 c2
) 0