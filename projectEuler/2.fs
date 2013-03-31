// Problem 2
// By considering the terms in the Fibonacci sequence whose values do not exceed four million
// find the sum of the even-valued terms

open System

let max = 4000000
let mutable sum = 0
let mutable current = 0
let mutable next = 1
while (next <= max) do
    let temp = next
    next <- current + next
    current <- temp
    if next % 2 = 0 then sum <- sum + next

printfn "%A" sum
