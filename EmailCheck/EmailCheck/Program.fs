﻿open System
open System.Text.RegularExpressions
open FsUnit
open NUnit.Framework

type EmailCheck() =
    class
        let username = "[a-z\d!#$%&'*+/=?^_`{|}~-]+(\.[a-z\d!#$%&'*+/=?^_`{|}~-]+)*"
        let hostname = "([a-z\d]([a-z\d-]{0,61}[a-z\d])?\.)*"
        let domain  = "(com|edu|info|museum|name|net|org|[a-z][a-z])"
        let pattern = "^" + username + "@" + hostname + domain + "$"

        member x.parseEmail (email : string) = Regex.IsMatch(email.ToLower(), pattern)
    end
                         
[<TestFixture>]
type EmailTests() =
    let emailCheck = new EmailCheck()

    let corrects   = [ "Ab5cd@gmail.com";
                       "Abc3d.abcd@gmail.com";
                       "!abcd2@gmail.com";
                       "-abcd4@gmail.ru";
                       "abcd.abcd.abcd@gmail.com";
                       "abcd@4-hours.com";
                       "abcd@g.com";
                       "abcd@my.site.the.best.com" ]

    let incorrects = [ "Abcd@-gmail.com";
                       "abcd..@gmail.com";
                       "abcd@.gmail.n";
                       ".abcd@gmail.nettt";
                       "abcd@gmail";
                       "abcd@gmail.";
                       "abcdgmail.com";
                       "abcd@my.site.the.worse..com" ]
    [<Test>]
    member x.Corrects()   = corrects |> List.iter (fun e -> let t = emailCheck.parseEmail(e) in 
                                                            Assert.AreEqual(true, t, "Error in email:" + e)
                                                            )

    [<Test>]
    member x.Incorrects() = incorrects |> List.iter (fun e -> let t = emailCheck.parseEmail(e) in 
                                                              Assert.AreEqual(false, t, "Error in email:" + e)
                                                              )
    