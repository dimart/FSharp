module WorldInterfaces

type CreatureType =
    | Puppy
    | Kitten
    | Hedgehog
    | Bearcub
    | Piglet
    | Bat
    | Balloon

type DaylightType =
    | Morning
    | Noon
    | Evening
    | Night

type IMagic =
    abstract member CallStork  : CreatureType -> unit
    abstract member CallDaemon : CreatureType -> unit

type IDaylight = 
    abstract member Current : DaylightType

type ILuminary =
    abstract member IsShining : unit -> bool

type IWind = 
    abstract member Speed : int

type IWeatherFactory =
    abstract member CreateWind     : unit -> IWind
    abstract member CreateDaylight : unit -> IDaylight
    abstract member CreateLuminary : unit -> ILuminary

type Creature(ctype : CreatureType) =
    member x.CreatureType = ctype

type ICourier =
    abstract member GiveBaby : Creature -> unit

