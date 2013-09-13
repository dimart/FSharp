open System
open System.Text.RegularExpressions

let username = "[a-z\d!#$%&'*+/=?^_`{|}~-]+(\.[a-z\d!#$%&'*+/=?^_`{|}~-]+)*"
let hostname = "([a-z\d]([a-z\d-]{0,61}[a-z\d])?\.)*"
let domain  = "(com|edu|info|museum|name|net|org|[a-z][a-z])"

let pattern = "^" + username + "@" + hostname + domain + "$"

let parseEmail str =
    if (Regex.IsMatch(str, pattern)) then printfn "True"
                                     else printfn "False" 

//Tests
printf "Tests\n"
printf "correct emails [SHOULD BE TRUE]:\n"
let correctTests = parseEmail "ab5cd@gmail.com"
                   parseEmail "abc3d.abcd@gmail.com"
                   parseEmail "!abcd2@gmail.com"
                   parseEmail "-abcd4@gmail.ru"
                   parseEmail "abcd.abcd.abcd@gmail.com"
                   parseEmail "abcd@4-hours.com"
                   parseEmail "abcd@g.com"
                   parseEmail "abcd@my.site.the.best.com"

printf "\nincorrect emails [SHOULD BE FALSE]: \n"
let incorrectTests = parseEmail "abcd@-gmail.com"
                     parseEmail "abcd..@gmail.com"
                     parseEmail "abcd@.gmail.n"
                     parseEmail ".abcd@gmail.nettt"
                     parseEmail "abcd@gmail"
                     parseEmail "abcd@gmail."
                     parseEmail "abcdgmail.com"
                     parseEmail "abcd@my.site.the.worse..com"

Console.ReadKey() |> ignore