module SingletonExample

open System
open Microsoft.FSharp.Collections

//////////////////////////
//Basic Chocolate Boiler//
//////////////////////////
(*
type ChocolateBoiler () =
    class
    let mutable empty  = true
    let mutable boiled = false
 
    member x.fill() =
        if x.isEmpty
        then empty  <- false
             boiled <- false
             printfn "Заполнение нагревателя молочно-шоколадной смесью...\nГотово."

    member x.drain() =
        if (not x.isEmpty &&  x.isBoiled)
        then
            printfn "Слитие нагретого молока и шоколада...\nГотово."
            empty <- true 
    
    member x.boil() =
        if (not x.isEmpty && not x.isBoiled)
        then 
            printfn "Доводим содержимое до кипения...\nГотово."
            boiled <- true

    member x.isEmpty  = empty
    member x.isBoiled = boiled
    end

[<EntryPoint>]
let main _ =
    let myCB  = new ChocolateBoiler()
    let myCB' = new ChocolateBoiler()
    myCB.fill()
    myCB'.fill()
    System.Console.ReadKey() |> ignore
    0
*)
///////////////////////////////////////
//Classic Singleton Chockolate Boiler//
///////////////////////////////////////
(*
[<AllowNullLiteralAttribute>]
type ChocolateBoiler private () =
    class
    let mutable empty  = true
    let mutable boiled = false
    static let mutable uniqueInstance = null
    
    static member Instance = 
        if uniqueInstance = null
        then uniqueInstance <- new ChocolateBoiler()
        uniqueInstance

    member x.fill() =
        if x.isEmpty
        then empty  <- false
             boiled <- false
             printfn "Заполнение нагревателя молочно-шоколадной смесью...\nГотово."

    member x.drain() =
        if (not x.isEmpty &&  x.isBoiled)
        then
            printfn "Слитие нагретого молока и шоколада...\nГотово."
            empty <- true 
    
    member x.boil() =
        if (not x.isEmpty && not x.isBoiled)
        then 
            printfn "Доводим содержимое до кипения...\nГотово."
            boiled <- true

    member x.isEmpty  = empty
    member x.isBoiled = boiled
    end

[<EntryPoint>]
let main _ =
//    let myCB  = ChocolateBoiler.Instance
//    let myCB' = ChocolateBoiler.Instance
//    myCB.fill()
//    myCB'.fill()
    let CB = PSeq.map (fun _ -> ChocolateBoiler.Instance) [1..10000]
    PSeq.iter (fun (x : ChocolateBoiler) -> x.fill()) CB 
    System.Console.ReadKey() |> ignore
    0
*)
////////////////////////////////////
//Lock Singleton Chockolate Boiler//
////////////////////////////////////
(*
[<AllowNullLiteralAttribute>]
type ChocolateBoiler private () =
    class
    let mutable empty  = true
    let mutable boiled = false
    static let mutable uniqueInstance = null
    static let lockobj = new Object()
    
    static member Instance = 
//        lock lockobj (fun () ->
//        if uniqueInstance = null
//        then uniqueInstance <- new ChocolateBoiler()
//        uniqueInstance)
        if uniqueInstance = null
        then lock lockobj (fun () -> if uniqueInstance = null then uniqueInstance <- new ChocolateBoiler())
        uniqueInstance

    member x.fill() =
        if x.isEmpty
        then empty  <- false
             boiled <- false
             printfn "Заполнение нагревателя молочно-шоколадной смесью...\nГотово."

    member x.drain() =
        if (not x.isEmpty &&  x.isBoiled)
        then
            printfn "Слитие нагретого молока и шоколада...\nГотово."
            empty <- true 
    
    member x.boil() =
        if (not x.isEmpty && not x.isBoiled)
        then 
            printfn "Доводим содержимое до кипения...\nГотово."
            boiled <- true

    member x.isEmpty  = empty
    member x.isBoiled = boiled
    end

[<EntryPoint>]
let main _ =
    let CB = PSeq.map (fun _ -> ChocolateBoiler.Instance) [1..100000]
    PSeq.iter (fun (x : ChocolateBoiler) -> x.fill()) CB 
    System.Console.ReadKey() |> ignore
    0
*)
////////////////////////////////////////////////////
//Static uniqueInstance Singleton Chocolate Boiler//
////////////////////////////////////////////////////
(*
type ChocolateBoiler private () =
    static let uniqueInstance = ChocolateBoiler()
    let mutable empty  = true
    let mutable boiled = false
    
    static member Instance = uniqueInstance

    member x.fill() =
        if x.isEmpty
        then empty  <- false
             boiled <- false
             printfn "Заполнение нагревателя молочно-шоколадной смесью...\nГотово."

    member x.drain() =
        if (not x.isEmpty &&  x.isBoiled)
        then
            printfn "Слитие нагретого молока и шоколада...\nГотово."
            empty <- true 
    
    member x.boil() =
        if (not x.isEmpty && not x.isBoiled)
        then 
            printfn "Доводим содержимое до кипения...\nГотово."
            boiled <- true

    member x.isEmpty  = empty
    member x.isBoiled = boiled

[<EntryPoint>]
let main _ =
    let chocolateBoilers = PSeq.map (fun _ -> ChocolateBoiler.Instance) [1..1000000]
    PSeq.iter (fun (x : ChocolateBoiler) -> x.fill()) chocolateBoilers
    System.Console.ReadKey() |> ignore
    0
*)