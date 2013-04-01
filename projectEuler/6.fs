// Problem #6
// Find the difference between the sum of the squares of the first one hundred natural numbers and the square of the sum.

let sumOfSqr = seq { for x in 1..100 -> x*x } |> Seq.sum 
let sqrOfSum = seq {1..100} |> Seq.sum |> fun x -> x*x
let res = sqrOfSum - sumOfSqr
printf "%A" res
