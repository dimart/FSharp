/* Problem #1
 * Find the sum of all the multiples of 3 or 5 below 1000 
 * http://projecteuler.net/problem=1
 */
 
Seq.sum <| seq { for x in 0..999 -> if (x % 3 = 0) || (x % 5 = 0) then x else 0 } 