(*
 * Project: Binary Tree
 * Author: dimart
 *)

open System

type 'a Tree =
  | Empty
  | Node of 'a Tree * 'a * 'a Tree

let rec addElem tree x =
    match tree with
    | Empty -> Node(Empty, x, Empty)
    | Node(left, value, right) when x < value -> Node(addElem left x, value, right)
    | Node(left, value, right) when x > value -> Node(left, value, addElem right x)
    | _ -> tree

let rec findElem tree x =
  match tree with
    | Empty -> false
    | Node(left, value, right) when x < value -> findElem left x
    | Node(left, value, right) when x > value -> findElem right x
    | Node(left, value, right) when x = value -> true
    | _ -> false

let rec findMin tree = 
    match tree with
    | Node(Empty, value, _) -> value
    | Node(left, value, _) -> findMin left

let rec delElem tree x =
    match tree with
    | Empty -> Empty
    | Node(left, value, right) when x < value -> Node(delElem left x, value, right)
    | Node(left, value, right) when x > value -> Node(left, value, delElem right x)
    | Node(left, value, right) when x = value -> 
      if left = Empty & right = Empty then Empty
      else if left = Empty then right
      else if right = Empty then left
      else let min = findMin right
           Node(left, min, delElem right min)

let printSpaces count = for i = 1 to count do printf " "
let mutable spaces = 0

let rec printSub tree =
  printSpaces spaces
  match tree with
    | Node(Empty, value, Empty) -> printfn "<NULL %A NULL>" value
                                   spaces <- spaces - 1
    | Node(left, value, right) -> printfn "(%A" value
                                  spaces <- spaces + 1
                                  printSub left
                                  spaces <- spaces + 1
                                  printSub right
                                  printSpaces spaces
                                  printfn ")"
                                  spaces <- spaces - 1
    | Empty -> printfn "<NULL>"
               spaces <- spaces - 1 

let print tree = 
    match tree with 
    | Node (left, value, right) -> printfn "ROOT: %A" value
                                   spaces <- spaces + 1
                                   printfn "LEFT:"
                                   printSub left
                                   spaces <- spaces + 1
                                   printfn "RIGHT:"
                                   printSub right
    | Empty -> printfn "This tree is empty."

let mutable LemonTree = Empty
LemonTree <- addElem LemonTree 18
LemonTree <- addElem LemonTree 40
LemonTree <- addElem LemonTree 45
LemonTree <- addElem LemonTree 84
LemonTree <- addElem LemonTree 90
LemonTree <- addElem LemonTree 81
LemonTree <- addElem LemonTree 82
LemonTree <- addElem LemonTree 9
LemonTree <- addElem LemonTree 10
LemonTree <- addElem LemonTree 7

print LemonTree

printfn "%A" <| findElem LemonTree 19
LemonTree <- addElem LemonTree 19
printfn "%A" <| findElem LemonTree 19
LemonTree <- delElem LemonTree 19
printfn "%A" <| findElem LemonTree 19

Console.ReadKey() |> ignore