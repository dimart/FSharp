//F#
open System

let N = 11
let tickets = Array.create N true
let start = DateTime.Now

let getNext () =
  let next() =
    let time = DateTime.Now
    let msec = int (time - start).TotalMilliseconds
    msec % N + 1
  let mutable current = next()
  while not <| tickets.[current] do
    current <- next()
  current

while Array.exists (fun x -> x) tickets do
    printfn "Press a button"
    Console.ReadKey() |> ignore
    let rand = getNext()
    tickets.[rand] <- false
    printfn "%A" <| rand 

printfn "There are no more tickets"
Console.ReadKey() |> ignore
