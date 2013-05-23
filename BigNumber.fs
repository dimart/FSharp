module BigNumber

open System

type BigNumber =
    //sign of BigNumber; if BigNumber = -9999... then sign = true
    val private sign : bool
    //value of BigNumber, looks like [3;2;1] for number 123
    val private value : int list
    //string representation of BigNumber
    val private string : string

    new (s:string) = 
        let sign = if s.StartsWith("-") then true else false
        let stringValue = s.Trim([|'-'|])
        let length = stringValue.Length
        let temp = BigNumber.reverse(stringValue)
        let value = Array.map (fun x -> Int32.Parse (string x)) temp |> Array.toList
        {sign = sign; value = value; string = stringValue}
    
    new (n:int) = BigNumber(n.ToString())
    new () = BigNumber(0)
    private new (n:int list) = {sign = false; value = n; string = BigNumber.listToString(n)}
    private new (n:int list, s:bool) = {sign = s; value = n; string = BigNumber.listToString(n)}
    
    static member private Base = 10

    override this.ToString() = (if this.sign && not (this.string.StartsWith("0")) then "-" else "") + this.string 
    
    static member private reverse(s:string) =
      let array = s.ToCharArray()
      Array.Reverse(array)
      array

    static member private listToString(l:int list) =
      let mutable result = ""
      let rec toString l s =
        let mutable string = s
        match l with 
        | h::t -> string <- String.Concat(sprintf "%A" h, string)
                  toString t string
        | [] -> string
      toString l result
     
    //return true if |a| > |b| && inverse = false
    member private a.isGreaterAbs (b:BigNumber, inverse:bool) : bool =
      let rec isGreaterAbs a b temp =
        match (a, b) with
        | ([], []) -> false
        | ([], _) -> false
        | (_, []) -> true
        | (hA::[], hB::[]) -> if hA < hB then false else if not inverse then hA > hB else temp
        | (hA::tA, hB::tB) -> isGreaterAbs tA tB (hA > hB)
      isGreaterAbs a.value b.value inverse
         
    //return true if a > b else false
    member private a.isGreater (b:BigNumber) =
      if a.sign && not b.sign then false 
      else if not a.sign && b.sign then true 
      else if a.sign && b.sign then not (a.isGreaterAbs (b, false))
      else (a.isGreaterAbs (b, false))
    
    static member private add'(l, transfer) =
      if transfer = 0 then l
      else 
      let rec add l transfer = 
              match l with 
              | [] -> [transfer]
              | h::t -> let temp = h + transfer 
                        (temp % BigNumber.Base)::(add t (temp / BigNumber.Base))
      add l transfer  

    static member private add(a, b) =
      let rec add a b transfer =
        match (a, b) with
        | (a, []) -> BigNumber.add'(a, transfer)
        | ([], b) -> BigNumber.add'(b, transfer)
        | (hA::tA, hB::tB) -> let temp = hA + hB + transfer 
                              (temp % BigNumber.Base)::(add tA tB (temp / BigNumber.Base))
      add a b 0

    static member private sub'(l, transfer) =
      let rec sub a transfer =
        match a with
        | [] -> []
        | h::[] -> if h = transfer then [] else [h - transfer]
        | h::t ->
           let d = h - transfer
           if (d < 0) then (d + BigNumber.Base)::(sub t 1)
           else d::t
      sub l transfer
    
    static member private sub(a, b) =
      let rec sub a b transfer =
        match (a, b) with
        | ([], _) -> []
        | (a, []) -> BigNumber.sub'(a, transfer)
        | (hA::tA, hB::tB) -> let d = hA - hB - transfer
                              let curr = if d < 0 then d + BigNumber.Base else d
                              let next = if d < 0 then sub tA tB 1 else sub tA tB 0
                              curr::(if next = 0::[] then [] else next)
      sub a b 0
    
    static member private mult'(list, n) = 
      let Base = BigNumber.Base
      let rec mult a transfer = 
        match a with
        | [] -> if transfer > 0 then [transfer] else []
        | h::t -> let tr = h * n + transfer
                  (tr % Base)::(mult t (tr / Base))
      mult list 0
    
    static member private mult(a, b) =
      let Base = BigNumber.Base
      let rec mult (b : int list) transfer = 
        match b with
        | [] -> transfer
        | hB::tB  ->  let temp = BigNumber.mult'(a, hB) 
                      match (transfer, temp) with
                      | (hTr::tTr, hT::tlT) -> let temp2 = hT + hTr 
                                               (temp2 % Base)::(mult tB (BigNumber.add(tTr, BigNumber.add'(tlT, temp2 / Base))))
                      | ([], hT::tlT) -> (hT % Base)::(mult tB (BigNumber.add'(tlT, hT / Base)))
                      | (_, _) -> []      
      mult b []

    static member private add(a:BigNumber, b:BigNumber) = 
      match a.sign, b.sign, a.isGreaterAbs(b, false) with 
      | (true, false, true) | (false, true, true) -> new BigNumber(BigNumber.sub(a.value, b.value), a.sign) 
      | (true, false, false) | (false, true, false) -> new BigNumber(BigNumber.sub(b.value, a.value), b.sign)
      | _, _, _ -> new BigNumber(BigNumber.add(a.value, b.value), a.sign) 
     
    static member private sub(a:BigNumber, b:BigNumber) = 
      match a.sign, b.sign, a.isGreaterAbs(b, false) with
      | false, false, true -> new BigNumber(BigNumber.sub(a.value, b.value), a.sign) 
      | false, false, false -> new BigNumber(BigNumber.sub(b.value, a.value), true) 
      | true, true, true -> new BigNumber(BigNumber.sub(b.value, a.value), a.sign) 
      | true, true, false -> new BigNumber(BigNumber.sub(b.value, a.value), false) 
      | false, true, _ | true, false, _-> new BigNumber(BigNumber.add(a.value, b.value), a.sign)
    
    static member private mult(a:BigNumber, b:BigNumber) = 
      match a.sign, b.sign with
      | true, true -> new BigNumber(BigNumber.mult(a.value, b.value), false) 
      | true, false | false, true -> new BigNumber(BigNumber.mult(a.value, b.value), a.sign || b.sign)
      | false, false -> new BigNumber(BigNumber.mult(a.value, b.value), a.sign && b.sign)  
        
    static member (+) (a:BigNumber, b:BigNumber) = BigNumber.add(a, b)
    static member (-) (a:BigNumber, b:BigNumber) = BigNumber.sub(a, b)
    static member (*) (a:BigNumber, b:BigNumber) = BigNumber.mult(a, b)
 
let test = 
  let a = new BigNumber("100000000000000")
  let b = new BigNumber("50000000000000")
  let c = new BigNumber("-50000000000000")
  let d = new BigNumber("-100000000000000")
  let mutable temp = new BigNumber("0")

  printfn "BigNumbers TESTS"
  printfn "[Task 1] Add:"
  printf "A + B: "
  temp <- a + b
  if (temp.ToString() = "150000000000000") then printfn "Passed" else printfn "Fail" 
  printf "A + (-B): "
  temp <- b + c
  if (temp.ToString() = "0") then printfn "Passed" else printfn "Fail"
  printf "A + (-B): "
  temp <- b + d
  if (temp.ToString() = "-50000000000000") then printfn "Passed" else printfn "Fail"
  printf "(-A) + B: "
  temp <- c + b
  if (temp.ToString() = "0") then printfn "Passed" else printfn "Fail"
  printf "(-A) + (-B): "
  temp <- c + d
  if (temp.ToString() = "-150000000000000") then printfn "Passed" else printfn "Fail"
  printfn ""

  
  printfn "[Task 2] Sub:"
  printf "A - B: "
  temp <- a - b
  if (temp.ToString() = "50000000000000") then printfn "Passed" else printfn "Fail" 
  printf "A - B: "
  temp <- b - a
  if (temp.ToString() = "-50000000000000") then printfn "Passed" else printfn "Fail"
  printf "A - (-B): "
  temp <- b - c
  if (temp.ToString() = "100000000000000") then printfn "Passed" else printfn "Fail"
  printf "(-A) - B: "
  temp <- c - b
  if (temp.ToString() = "-100000000000000") then printfn "Passed" else printfn "Fail"
  printf "(-A) - (-B): "
  temp <- c - d
  if (temp.ToString() = "50000000000000") then printfn "Passed" else printfn "Fail"
  printf "(-A) - (-B): "
  temp <- d - c
  if (temp.ToString() = "-50000000000000") then printfn "Passed" else printfn "Fail"
  printfn ""

  printfn "[Task 3] Mult:"
  printf "A * B: "
  temp <- a * b
  if (temp.ToString() = "5000000000000000000000000000") then printfn "Passed" else printfn "Fail" 
  printf "A * (-B): "
  temp <- b * c
  if (temp.ToString() = "-2500000000000000000000000000") then printfn "Passed" else printfn "Fail"
  printf "(-A) * B: "
  temp <- c * b
  if (temp.ToString() = "-2500000000000000000000000000") then printfn "Passed" else printfn "Fail"
  printf "(-A) * (-B): "
  temp <- c * d
  if (temp.ToString() = "5000000000000000000000000000") then printfn "Passed" else printfn "Fail"
  printfn ""

Console.ReadKey() |> ignore