(*
    OOWorld of Elementary Particles with own Coliders
    Particle--|
              |--Fermion--|
              |           |--Quark--|
              |           |         |--uQuark
              |           |         |--dQuark
              |           |         |--cQuark
              |           |         |--sQuark
              |           |         |--tQuark
              |           |         |--bQuark
              |           |--Lepton-|
              |           |         |--Electron
              |           |         |--Electron neutrino
              |           |         |--Muon 
              |           |         |--Muon neutrino
              |           |         |--Tau
              |           |         |--Tau neutrino
              |
              |--Boson----|
              |           |--GBoson-|
              |           |         |--Photon
              |           |         |--Gluon
              |           |         |--W-Boson---|
              |           |         |            |--W-BosonPositive
              |           |         |            |--W-BosonNegative
              |           |         |--Z-Boson
              |           |--SBoson-|
              |           |         |--Higgs
              |
              |--Hypothet-|
              |           |--Graviton
*)

open System

[<Measure>] type eV 
[<Measure>] type c 
[<Measure>] type MeV = eV^6
[<Measure>] type e

type Particle(name : string, mass : float<MeV/c^2>, charge : float<e>, spin : float, stat : string) = 
    class
       let name         = name
       let mass         = mass
       let charge       = charge
       let spin         = spin
       let statistics   = stat
       
       member this.Name        = name
       member this.Mass        = mass
       member this.Charge      = charge
       member this.Spin        = spin
       member this.Statistic   = statistics
       member this.ShowInfo    = printfn "[INFO]\n\
                                          Name:   %A\n\
                                          Mass:   %A MeV/c^2\n\
                                          Charge: %A e\n\
                                          Spin:   %A\n\
                                          Stat:   %A"
                                          name mass charge spin stat
    end

type Fermion(name : string, mass : float<MeV/c^2>, charge : float<e>, generation : int, discovered : int) =  
    class 
       inherit Particle(name, mass, charge, 0.5, "Fermi–Dirac statistics") 
       let generation = generation
       let discovered = discovered

       member this.Generation = generation      
       member this.Discovered = discovered                     
    end

type Boson(name : string, mass : float<MeV/c^2>, charge : float<e>, spin : float, discovered : int) = 
    class  
       inherit Particle(name, mass, charge, spin, "Bose–Einstein statistics") 
       let discovered = discovered

       member this.Discovered = discovered 
    end

[<AbstractClass>]
type Hypothetical(name : string, mass : float<MeV/c^2>, charge : float<e>, spin : float, stat : string, theorized : int) =
    class
       inherit Particle(name, mass, charge, spin, stat)
       let theorized = theorized

       member this.Theorized = theorized
       abstract member Description : unit -> string
    end
    
[<AbstractClass>]
type Quark(name : string, mass : float<MeV/c^2>, charge : float<e>, generation : int, discovered : int) =
    class
       inherit Fermion(name, mass, charge, generation, discovered)
       member this.ColorCharge = true
       abstract member Description : unit -> string
    end

[<AbstractClass>]
type Lepton(name : string, mass : float<MeV/c^2>, charge : float<e>, generation : int, discovered : int) = 
    class
       inherit Fermion(name, mass, charge, generation, discovered)
       member this.ColorCharge = false
       abstract member Description : unit -> string
    end

[<AbstractClass>]
type GaugeBoson(name : string, mass : float<MeV/c^2>, charge : float<e>, discovered : int) = 
    class
       inherit Boson(name, mass, charge, 1.0, discovered)
       abstract member Description : unit -> string
    end

[<AbstractClass>]
type ScalarBoson(name : string, mass : float<MeV/c^2>, charge : float<e>, discovered : int) = 
    class
        inherit Boson(name, mass, charge, 0.0, discovered)
        abstract member Description : unit -> string
    end
//--------------------------------Quarks[BEGIN]----------------------------------------------------
type uQuark() = 
    inherit Quark("Up quark", 2.3<MeV/c^2>, 0.66<e>, 1, 1968)
    override this.Description() = "The Up quark is a teeny little point inside the proton and neutron, \
                                   it is friends forever with the Down quark."

type dQuark() = 
    inherit Quark("Down quark", 4.8<MeV/c^2>, -0.33<e>, 1, 1968)
    override this.Description() = "The Down quark along with the Up quark, make up protons and neutrons.\
                                   Everyday physical matter contains of Up-quarks and Down qouarks."

type cQuark() = 
    inherit Quark("Charm quark", 1290.0<MeV/c^2>, 0.66<e>, 2, 1974)
    override this.Description() = "Heavier than a Strange quark, but not so heavy as a Bottom quark, the \
                                   Charm-quark was discovered in 1974. Particles that contain Charm and \
                                   Anticharm quarks are called \"charmed matter\"."

type sQuark() = 
    inherit Quark("Strange quark", 95.0<MeV/c^2>, -0.33<e>, 2, 1968)
    override this.Description() = "The 2nd generation of Down quark, Strange quake weighs about the same \
                                   as a Muon and was discovered in 1968."

type tQuark() = 
    inherit Quark("Top quark", 173070.0<MeV/c^2>, 0.66<e>, 3, 1995)
    override this.Description() = "Discovered at Fermilab in 1995, the Top quark is as short-lived as is \
                                   massive. Weighing in at a hefty 175 GeV, its lifetime, a mere 10^-24 \
                                   second, is the briefest of the six quarks."

type bQuark() = 
    inherit Quark("Buttom quark", 4180.0<MeV/c^2>, -0.33<e>, 3, 1977)
    override this.Description() = "Nine times heavier than a proton, the short-lived Bottom quark is the \
                                   3rd generation of the Down and Charm quarks, all sharing a -1/3 charge \
                                   It was discovered at Fermilab in 1977."
//--------------------------------Quarks[END]-------------------------------------------------------

//--------------------------------Leptons[BEGIN]----------------------------------------------------
type Electron() = 
    inherit Lepton("Electron", 0.510998928<MeV/c^2>, -1.0<e>, 1, 1897)
    override this.Description() = "The electron is a fundamental subatomic partile carrying a negative charge. \
                                   Its mass is 1/1000 of the smallest atom. It participates in electromagnetic \
                                   interactions, and is typically found orbiting the nucleus of an atom."

type ElectronNeutrino() = 
    inherit Lepton("Electron neutrino", 0.0000000001<MeV/c^2>, 0.0<e>, 1, 1956)
    override this.Description() = "He like to steal the away energy and is notoriously difficult to detect. \
                                   Travelling close to the speed of light, he is the most pervasive form of \
                                   matter in the universe. Trillions of neutrinos are passing through everything \
                                   around us, including us, at every moment. The result of radioactive neutron \
                                   decay, most neutrinos originate from the sun. Their mass is next to nothing."

type Muon() = 
    inherit Lepton("Muon", 105.6583715<MeV/c^2>, -1.0<e>, 2, 1936)
    override this.Description() = "The Muon is a short-lived, heavier version of the electron. It has the same negative \
                                   charge, but is 200 times more massive than the electron."

type MuonNeutrino() = 
    inherit Lepton("Muon neutrino", 0.0000000001<MeV/c^2>, 0.0<e>, 2, 1962)
    override this.Description() = "Like its first-generation sibling lepton the electron-neutrino, the Muon neutrino \
                                   is extrimely difficult to detect. Discovered in 1962, it is emitted in the decay of a muon. \
                                   Its mass is about one-third of an electron."

type Tau() = 
    inherit Lepton("Tau", 1776.82<MeV/c^2>, -1.0<e>, 3, 1975)
    override this.Description() = "The Tau is a short-lived (3x10^-13 second), heavier version of the muon and electron. \
                                   It has the same negative charge, but is 3,478 times more massive that the ellectron."

type TauNeutrino() = 
    inherit Lepton("Tau Neutrino", 0.0000000001<MeV/c^2>, 0.0<e>, 3, 2000)
    override this.Description() = "Like its sibling leptons the electron-neutrino and the muon-neutrino, this cheeky little devil, \
                                   the Tau-neutrino, is extremely difficult to detect. Discovered in 2000, it is about 100 times \
                                   heavier than a muon-neutrino."
//--------------------------------Leptons[END]-----------------------------------------------------

//--------------------------------Guage Boson[BEGIN]-----------------------------------------------
type Photon() =
    inherit GaugeBoson("Photon", 0.0<MeV/c^2>, 0.0<e>, 1923)
    override this.Description() = "The Photon is quanta of visible light, a wave/particle that communicates the electromagnetic \
                                   force, traveling at the speed of light. With a mass and electric charge of zero, it also \
                                   carries microwaves, radio waves and x-rays."

type Gluon() =
    inherit GaugeBoson("Gluon", 0.000000002<MeV/c^2>, 0.0<e>, 1979)
    override this.Description() = "The Gluon is the force-carring particle of the strong nuclear force, which holds quarks together \
                                   and binds the nucleus of atom. Discovered in 1979, it is stable and massles. \
                                   At extremely high temperatures, quarks and gluons fluidly mix into a quark-gluon plasma. \
                                   It is theorized that gluons can interact with each other and from Glueballs."
type WBoson(charge : float<e>) =
    inherit GaugeBoson("W-Boson", 80398.0<MeV/c^2>, charge, 1983)
    override this.Description() = "He may be a weak force boson, but he's got formidable heft! Doscovered on 1983 at CERN, \
                                   the W-Boson is best known for nuclear decay. Very massive and extremely short-lived \
                                   (10^-25 second), a W-boson is heavier than an atom of iron. Unlike other bosons \
                                   it has either positive or negative charge."

type WBosonPos() =
    inherit WBoson(1.0<e>)

type WBosonNeg() = 
    inherit WBoson(-1.0<e>)

type ZBoson() =
    inherit GaugeBoson("Z-Boson", 91187.6<MeV/c^2>, 0.0<e>, 1983)
    override this.Description() = "The Z-Boson is a very massive carrier particle for weak force. Unlike its siblings the\
                                   W-/W+ particles, the Z is neutrally charged. Living only 10^-25 second, the Z quickly \
                                   decays into other particles. Discovered in 1983, the Z has allowed physicists to further \
                                   study electroweak theory."         
//-------------------------------Guage Boson[END]--------------------------------------------------

//-------------------------------Scalar Boson[BEGIN]-----------------------------------------------
type Higgs() =
    inherit ScalarBoson("Higgs Boson", 125300.0<MeV/c^2>, 0.0<e>, 2013)
    override this.Description() = "The Higgs Boson is particle of the Higgs mechanism, which physicists believe will reveal \
                                   how all matter in the universe gets its mass."  
//-------------------------------Scalar Boson[END]-------------------------------------------------

//-------------------------------Hypothetical[BEGIN]-----------------------------------------------
type Graviton() = 
    inherit Hypothetical("Graviton", 0.0<MeV/c^2>, 0.0<e>, 2.0, "Bose–Einstein statistics", 1930)
    override this.Description() = "The Graviton is a particle not yet observed. It communicates the force of gravity and \
                                   is the smallest bundle of the gravitational force field."  
//-------------------------------Hypothetical[END]-----------------------------------------------

let dq x = (x :> Quark)

type switchstate = On | Off

type Collider(name : string) =
    class
    let name          = name
    let mutable state = Off
    let quarks        = [| 
                          dq(new uQuark()); dq(new dQuark()); 
                          dq(new sQuark()); dq(new cQuark());
                          dq(new tQuark()); dq(new bQuark()) 
                        |]

    member this.Name = name
    member this.SwitchOn() = 
        printfn "Collider %A starts working..." name 
        printfn "Initialization of all systems... Done\n"
        state <- match state with On -> Off | Off -> On 

    member this.CatchQ() = 
        let rnd = System.Random()
        quarks.[rnd.Next(0,quarks.Length)] 

    member this.GetLucky() = 
        match state with
        | On ->  printfn "Let's find some new particles!"
        | Off -> printfn "You'll be more lucky if you turn on the collider first!\n\
                          But of course we can do it for you... lazy little scientist." 
                 state = On |> ignore
        let rnd = System.Random()
        match rnd.Next(0,1000000) with
        | 1       -> printfn "Yeah, we found something interesting! Nobel Prize waiting for you!"
                     new Graviton() :> Particle
        | _       -> printfn "It's the Electron...again" 
                     new Electron() :> Particle
    end
//-----------------

let myHomeCollider = new Collider("Home Collider")
myHomeCollider.SwitchOn()

let quark = myHomeCollider.CatchQ()
printfn "We caught: %A" quark.Name
quark.ShowInfo
printfn "About:  %A\n" <| quark.Description()

let particle = myHomeCollider.GetLucky()
particle.Name |> printfn "We caught: %A"

Console.ReadKey() |> ignore
