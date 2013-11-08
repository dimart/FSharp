module WorldTest

open World
open WorldInterfaces
open NUnit.Framework
open FsUnit

type StaticWeatherFactory() =
    interface IWeatherFactory with 
     member x.CreateWind()     = { new IWind     with member x.Speed = 2          }
     member x.CreateDaylight() = { new IDaylight with member x.Current = Morning  }
     member x.CreateLuminary() = { new ILuminary with member x.IsShining() = true }

[<TestFixture>] 
type ``Weather: WSpeed = 2; Morning; IsShining = true`` ()=
    let testCloud = new Cloud(new StaticWeatherFactory())

    [<Test>] member x.
     `` Kitten should be returned`` ()=
            testCloud.Create().CreatureType |> should equal Kitten