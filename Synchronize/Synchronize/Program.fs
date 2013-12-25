open System

let processes = 5
let rnd       = new Random()
let mutable q = [| for i in 0..processes -> false |]

let asyncProcess(i : int) = 
    let time = rnd.Next(1,100)
    printfn "Process №%A will wait %A" i time
    async {
        do! Async.Sleep(time)
        printfn "Process №%A completed!" i   
    }

let asyncProcesses = [for i in 1 .. processes -> asyncProcess(i)]     

[<EntryPoint>]
let main _ =
    printfn "------------------------"
    asyncProcesses |> Async.Parallel |> Async.RunSynchronously |> ignore
    printfn "------------------------"
    printfn "Done!"
    Console.ReadKey() |> ignore
    0