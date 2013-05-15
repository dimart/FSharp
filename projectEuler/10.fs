// Problem 10
// Find the sum of all the primes below two million.

let isPrime x = 
    let mutable i = 2
    while x % i <> 0 do
        i <- i + 1
        if (i > int (sqrt (float x))) then i <- x
    i = x

let ans = {1..2000000} |> Seq.filter isPrime |> Seq.map int64 |> Seq.sum
printfn "%A" ans