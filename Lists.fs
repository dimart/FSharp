let rec addToEnd e list =
    match list with
    | head::tail -> head :: addToEnd e tail
    | [] -> [e]

let rec concat list1 list2 =
    match list1 with
    | head::tail -> head::concat tail list2
    | [] -> list2

let rec reverse list =
    match list with
    | head::tail -> (reverse tail) @ [head]
    | [] -> []

let rec map func list =
    match list with
    | head::tail -> (func head) :: (map func tail)
    | [] -> []

let rec find key list=
    match list with
    | [] -> None
    | head::tail -> if key head then Some head 
                                else find key tail

(* TESTS *)
let addToEndTEST =
    (addToEnd [] []) = [[]] &&
    (addToEnd 42 []) = [42] &&
    (addToEnd 23 [1..22]) = [1..23] &&
    (addToEnd "World" ["Hello"]) = ["Hello"; "World"]

let concatTEST =
    (concat [] []) = [] &&
    (concat [1..7] [8]) = [1..8] &&
    (concat [1..23] []) = [1..23] &&
    (concat ["Hello"] ["World"]) <> ["Goodbye World!"]

let reverseTEST =
    (reverse []) = [] &&
    (reverse [8]) = [8] &&
    (reverse [1..23]) = [23..-1..1] &&
    (reverse ["Hello"; "World"]) = ["World"; "Hello"]

let mapTEST =
    (map ((+) 1) []) = [] &&
    (map ((+) 1) [1..10]) = [2..11] &&
    (map ((*) 10) [1..23]) = [10..10..230] &&
    (map ( (+) "!") ["Hello"; "World"]) = ["!Hello"; "!World"]

let findTEST =
    (find ((>) 0) []) = None &&
    (find ((=) 8) [8]) = Some 8 &&
    (find ((=) 1000) [1..23]) = None &&
    (find ((>) (0, 0)) [(1, 1); (5, 5)]) <> Some (1, 1)

    
printfn "Result of addToEndTEST = %A" addToEndTEST
printfn "Result of concatTEST = %A" concatTEST
printfn "Result of reverseTEST = %A" reverseTEST
printfn "Result of mapTEST = %A" mapTEST
printfn "Result of findTEST = %A" findTEST
