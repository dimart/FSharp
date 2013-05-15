// Problem #7
// What is the 10 001st prime number?

let isPrime x = 
    let mutable i = 2
    while x % i <> 0 do
        i <- i + 1
        if (i > int (sqrt (float x))) then i <- x
    i = x

let mutable ans = 1
let mutable i = 2    

while i <= 10001 do
    ans <- ans + 2
    if isPrime ans then i <- i + 1

printfn "%A" <| ans 