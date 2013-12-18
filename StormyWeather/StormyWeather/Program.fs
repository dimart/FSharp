open System
open System.Windows.Forms
open System.Drawing
open System.IO
open System.Net
open System.Xml.Linq
open System.Timers
open FSharp.Data

let  updateTimeMS    = 5000.0
type weatherXml      = XmlProvider<"http://export.yandex.ru/weather-ng/forecasts/26063.xml">
type SetIntCallback  = delegate of int -> unit
type SetTextCallback = delegate of string -> unit

type IWeatherObserver = 
    abstract member updateTemp       : int    -> unit
    abstract member updateWindSpeed  : int    -> unit
    abstract member updateWCondition : string -> unit
    abstract member updateWindDirect : string -> unit 

type IWeather =
    abstract member registerObserver : IWeatherObserver -> unit
    abstract member removeObserver   : IWeatherObserver -> unit
    abstract member notifyObservers  : unit

type Weather() as this =
    class
        let mutable observers = []
        let timer = new Timer(updateTimeMS)
        let mutable temp       = 0
        let mutable windSpeed  = 0
        let mutable wCondition = ""
        let mutable windDirect = ""
        do
            timer.Elapsed |> Event.add this.Update
            timer.Start()
        interface IWeather with
            member x.registerObserver(o) = observers <- o :: observers
            member x.removeObserver(o)   = observers <- List.filter (fun x -> x <> o)  observers 
            member x.notifyObservers     = List.iter (fun (o:IWeatherObserver) -> o.updateTemp(temp);
                                                                                  o.updateWindSpeed(windSpeed);
                                                                                  o.updateWindDirect(windDirect);
                                                                                  o.updateWCondition(wCondition);
                                                                                  ) observers
        member private x.Update _ = 
            temp        <- weatherXml.GetSample().Fact.Temperature.Value
            windSpeed   <- Convert.ToInt32(weatherXml.GetSample().Fact.WindSpeed)
            wCondition  <- weatherXml.GetSample().Fact.WeatherCondition.Code
            windDirect  <- weatherXml.GetSample().Fact.WindDirection
            (x :> IWeather).notifyObservers
            ()
    end

type WeatherMainForm(w:int, h:int) as this =
    class
        inherit Form(Text = "Saint-Petersburg Weather", Width = w, Height = h) 
        let weather  = new Weather() 
        let bitmap   = new Bitmap(Directory.GetCurrentDirectory() + "\\back01.jpg")
        let cityName = new Label(
                                  Text = "Saint-Petersburg", 
                                  Font = new Font("Monotype Corsiva", 27.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204))), 
                                  Location = new Point(13, 5),
                                  Size = new System.Drawing.Size(241, 45),
                                  BackColor = System.Drawing.Color.Transparent
                                )
        let temp     = new Label(
                                  Text = "0 C", 
                                  Font = new Font("Times New Roman", 48.0F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))), 
                                  Location = new Point(8, 45),
                                  Size = new System.Drawing.Size(123, 73),
                                  BackColor = System.Drawing.Color.Transparent
                                )
                                  
        let windLabel = new Label(
                                  Text = "Wind:", 
                                  Location = new Point(17,115), 
                                  Font = new Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                                  Size = new System.Drawing.Size(62, 23),
                                  BackColor = System.Drawing.Color.Transparent
                                  )
        let windSpeed = new Label(
                                  Text = "0 m/s", 
                                  Location = new Point(80, 115), 
                                  Font = new Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                                  Size = new System.Drawing.Size(57, 23),
                                  BackColor = System.Drawing.Color.Transparent
                                  )
        let windDirect = new Label(
                                  Text = "", 
                                  Location = new Point(137, 115), 
                                  Font = new Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                                  Size = new System.Drawing.Size(48, 23),
                                  BackColor = System.Drawing.Color.Transparent
                                  )
        let wCondition = new Label(
                                  Text = "", 
                                  Location = new Point(17, 137), 
                                  Font = new Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204))),
                                  Size = new System.Drawing.Size(120, 23),
                                  BackColor = System.Drawing.Color.Transparent
                                  )
        do
              (weather :> IWeather).registerObserver(this)
              this.FormBorderStyle <- FormBorderStyle.None
              this.GotFocus.AddHandler(new EventHandler(fun _ _ -> (this.FormBorderStyle <- FormBorderStyle.FixedSingle) |> ignore))
              this.LostFocus.AddHandler(new EventHandler(fun _ _ -> (this.FormBorderStyle <- FormBorderStyle.None) |> ignore))
              this.MaximizeBox <- false
              this.Controls.Add(temp) 
              this.Controls.Add(cityName)
              this.Controls.Add(windLabel)
              this.Controls.Add(windSpeed)
              this.Controls.Add(windDirect)
              this.Controls.Add(wCondition)
              this.BackgroundImage <- bitmap
        interface IWeatherObserver with 
            member x.updateTemp(data)       = if (temp.InvokeRequired) 
                                                then temp.Invoke(new SetIntCallback((x:>IWeatherObserver).updateTemp), data) |> ignore 
                                                else temp.Text <- (data.ToString() + " C")
            member x.updateWindSpeed(data)  = if (windSpeed.InvokeRequired) 
                                              then windSpeed.Invoke(new SetIntCallback((x:>IWeatherObserver).updateWindSpeed), data) |> ignore 
                                              else windSpeed.Text <- (data.ToString() + " m/s")
            member x.updateWindDirect(data) = if (windDirect.InvokeRequired) 
                                              then windDirect.Invoke(new SetTextCallback((x:>IWeatherObserver).updateWindDirect), data) |> ignore 
                                              else windDirect.Text <- data
            member x.updateWCondition(data) = if (windDirect.InvokeRequired) 
                                              then wCondition.Invoke(new SetTextCallback((x:>IWeatherObserver).updateWCondition), data) |> ignore 
                                              else wCondition.Text <- data
    end

#if COMPILED
[<STAThread()>]
Application.Run(new WeatherMainForm(480,272))
#endif
