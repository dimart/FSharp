// Problem 10
// Find the sum of all the primes below two million.

let isPrime n =
    if n <= 1L then false
    else
        let m = int64 (sqrt (float(n)))
        Seq.forall (fun i -> (n % i) <> 0L) {2L..m}
        
let ans = {2L..2000000L} |> Seq.filter isPrime |> Seq.sum
printfn "%A" ans