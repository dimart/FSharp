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

let mutable LemonTree = Empty
LemonTree <- addElem LemonTree 18
LemonTree <- addElem LemonTree 42
LemonTree <- addElem LemonTree 8
LemonTree <- addElem LemonTree 9
printfn "%A" LemonTree
printfn "%A" <| findElem LemonTree 19
LemonTree <- addElem LemonTree 19
printfn "%A" <| findElem LemonTree 19
LemonTree <- delElem LemonTree 19
printfn "%A" <| findElem LemonTree 19

Console.ReadKey() |> ignore