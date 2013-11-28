open System
open System.IO.Ports
open System.Windows.Forms
open System.Drawing

type IObserver = 
    abstract member update : string  -> unit

[<AllowNullLiteralAttribute>]
type ISubject =
    abstract member registerObserver : IObserver -> unit
    abstract member removeObserver   : IObserver -> unit
    abstract member notifyObservers  : unit

type IDisplayElements = 
    abstract member display : unit

type WeatherData() as this =
    class
    let mutable observers  = []
    let mutable luminosity = ""
    let port = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One)
    do
        port.Open()
        port.DataReceived.AddHandler(new SerialDataReceivedEventHandler(this.measurementsChanged))
    interface ISubject with
        member x.registerObserver(o) = observers <- o :: observers
        member x.removeObserver(o)   = observers <- List.filter (fun x -> x <> o) observers 
        member x.notifyObservers     = List.iter (fun (o:IObserver) -> o.update(luminosity)) observers
    member x.measurementsChanged _ _ = 
        luminosity <- port.ReadLine()
        (x :> ISubject).notifyObservers
        ()
    member x.detachMe(o) = (x :> ISubject).removeObserver(o)
    end

type BasicLookDisplay(subject : ISubject) as this =
    let mutable luminosity = ""
    let mutable weatherData = null
    do 
      weatherData <- subject
      weatherData.registerObserver(this)
    interface IObserver with 
        member x.update(data) = 
            luminosity <- data
            (x:>IDisplayElements).display  //There is more elegant ways designing data visualization. See MVC Pattern
    interface IDisplayElements with
        member x.display = printfn "Luminosity = %A" luminosity

type AdvancedLookDisplay(subject : ISubject) as this =
    let form                = new Form(Text = "Display")
    let mutable weatherData = null
    let btnOff              = new Button(Text = "Off", Left = 70)
    let btnOn               = new Button(Text = "On")
    do
        form.Controls.Add(btnOn)
        form.Controls.Add(btnOff)
        form.Show()
        weatherData <- subject
        btnOn.Click.AddHandler (new EventHandler(fun _ _ -> weatherData.registerObserver(this)))
        btnOff.Click.AddHandler(new EventHandler(fun _ _ -> weatherData.removeObserver(this); form.BackColor <- Color.FromArgb(255,255,255)))
    interface IObserver with 
        member x.update(data) = form.BackColor <- Color.FromArgb(0, 0, ((int (data)) % 255))
      

[<EntryPoint>]
let main _ =
    let weatherData    = new WeatherData()
    let textDisplay    = new BasicLookDisplay(weatherData)
    let formDisplay    = new AdvancedLookDisplay(weatherData)
    Application.Run()
    0
