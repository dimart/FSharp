(*
    Based on short movie from Pixar "Partly Cloudy"
    2013
*)
module World

open WorldInterfaces
open System

type Stork() =
  interface ICourier with
    member x.GiveBaby(c : Creature) = ()
 
type Daemon() = 
   interface ICourier with 
    member x.GiveBaby(c : Creature) = ()
    
type Wind() =
 interface IWind with
    member x.Speed = (System.Random()).Next(0,10)

type Magic() =
    member x.CallCourier(ctype : CreatureType) = 
      match ctype with 
      | Puppy -> new Stork()  :> ICourier
      | _     -> new Daemon() :> ICourier
//    interface IMagic with
//      member x.CallStork = 

type Luminary() =
    let shine = (System.Random()).Next(0,1)
    interface ILuminary with 
      member x.IsShining() =  
       match shine with
       | 0 -> false
       | 1 -> true
       | _ -> false

type Daylight() =
   let daytime = (System.Random()).Next(0,4)
   interface IDaylight with
     member x.Current = 
      match daytime with
      | 0 -> Morning
      | 1 -> Noon
      | 2 -> Evening
      | _ -> Night

type RandomWeatherFactory() =
    interface IWeatherFactory with 
     member x.CreateWind()     = new Wind()     :> IWind
     member x.CreateDaylight() = new Daylight() :> IDaylight
     member x.CreateLuminary() = new Luminary() :> ILuminary

type Cloud(factory : IWeatherFactory) =
    let daylight = factory.CreateDaylight() // new Daylight()
    let luminary = factory.CreateLuminary() // new Luminary()
    let wind     = factory.CreateWind()     // new Wind()
 
    member x.InternalCreate() =
      if daylight.Current = DaylightType.Morning then
        if luminary.IsShining() then
          new Creature(CreatureType.Kitten)
      // TODO – implement all other creatures
        else
          raise <| new System.NotImplementedException()
      else
        raise <| new System.NotImplementedException()
 
    member x.Create() =
      let creature = x.InternalCreate()
      let magic = new Magic()     
      magic.CallCourier(creature.CreatureType).GiveBaby(creature)
      creature

