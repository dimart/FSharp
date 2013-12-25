open System

let processes = 5

let asyncProcess(i : int) = 
    async {
        do! Async.Sleep((new Random()).Next(1,1000))
        printfn "Process waited %A ms" (i+1)
    }

let asyncProcesses = [for i in 1 .. processes -> asyncProcess(i)]

[<EntryPoint>]
let main _ =
    asyncProcesses |> Async.Parallel |> Async.RunSynchronously |> ignore
    Console.ReadKey() |> ignore
    0