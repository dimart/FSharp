// Problem #5
// What is the smallest positive number that is evenly divisible by all of the numbers from 1 to 20?

let rec GCD a b = 
    if a = 0 then b 
    else GCD (b % a) a

let mutable res = 1
for i = 2 to 20 do res <- (i / GCD i res) * res

printfn "%A" res 
