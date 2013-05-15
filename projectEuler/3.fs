// Problem 3
// What is the largest prime factor (LPF) of the number 600851475143 ?

let x = 600851475143L
let mutable LPF = 2L
let mutable div = x

while (div <> 1L) do 
    if (div % LPF = 0L) then div <- div / LPF
                        else LPF <- LPF + 1L
                        
printf "%A" LPF