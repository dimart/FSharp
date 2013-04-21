// Problem 4
// Find the largest palindrome made from the product of two 3-digit numbers.

let isPalindrome (x : int) = 
   let mutable ret = true
   let s = System.Convert.ToString x 
   for i = 0  to s.Length/2 do
     if s.[i] <> s.[s.Length-i-1] then 
       ret <- false
   ret

let mutable max = 0   
    
for i = 1 to 999 do
  for j = 1 to 999 do
  let p = i*j
  if isPalindrome p then
    if (max < p) then max <- p
	
printf "%A" max