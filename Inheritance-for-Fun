module Main
open System

type Meal (s:string, c:int) =
    let name = s
    let coast = c

    member x.getName = name
    member x.getCoast = coast

type Food (s:string, c:int, w:int) = 
    inherit Meal (s, c)
    let weight = w
  
    member x.getWeight = weight 

type Drink (s:string, c:int, alc:Boolean, vol:int) =
    inherit Meal (s,c)
    let alco = alc
    let volume = int

    member x.isAlco = alco

type Sushi (s:string, c:int, w:int, sp:String) =
    inherit Food (s,c,w)
    let species = sp

    member x.getSpecies = sp 

type Soup (s:string, c:int, w:int, sp:String) =
    inherit Food (s,c,w)
    let species = sp

    member x.getSpecies = sp 

type Juice (s:string, c:int, vol:int, t:String) =
    inherit Drink (s,c,false,vol)
    let volume = int
    let taste = t

let tonus = new Juice("Tonus", 82, 1000, "Orange")
printf "Juice with name = %A" <| tonus.getName 
printf " coasts %A." <| tonus.getCoast 
printf " Is it alco? %A" <| tonus.isAlco

Console.ReadKey() |> ignore
