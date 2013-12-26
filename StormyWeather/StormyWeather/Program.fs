open System
open System.Windows.Forms
open System.Drawing
open System.IO
open System.Net
open System.Xml.Linq
open System.Timers
open FSharp.Data
open System.ComponentModel

let  updateTimeMS    = 1000.0
let  wclient         = new WebClient()
let  imgLinks        = [| ("https://www.dropbox.com/s/ivt1ej3673ql7fa/weather-clear.jpg?dl=1",            "./img/weather-clear.jpg");
                          ("https://www.dropbox.com/s/njtmgehmc2gro2x/weather-few-clouds.jpg?dl=1",       "./img/weather-few-clouds.jpg");
                          ("https://www.dropbox.com/s/5dnhu02r0gzgn6s/weather-fog.jpg?dl=1",              "./img/weather-fog.jpg");
                          ("https://www.dropbox.com/s/naddjzowxbsf2b0/weather-overcast.jpg?dl=1",         "./img/weather-overcast.jpg");
                          ("https://www.dropbox.com/s/7eaf9rzwt93bl8r/weather-showers.jpg?dl=1",          "./img/weather-showers.jpg");
                          ("https://www.dropbox.com/s/xs2zlncbobdd4gb/weather-snow.jpg?dl=1",             "./img/weather-snow.jpg");
                          ("https://www.dropbox.com/s/qixtjpm0mk3zavg/weather-storm.jpg?dl=1",            "./img/weather-storm.jpg")
                       |]

type SaintPbXml     = XmlProvider<"http://export.yandex.ru/weather-ng/forecasts/26063.xml">
type MoscowXml      = XmlProvider<"http://export.yandex.ru/weather-ng/forecasts/27612.xml">

type SetIntCallback  = delegate of int -> unit
type SetTextCallback = delegate of string -> unit

type IWeatherObserver = 
    abstract member updateTemp       : int    -> unit
    abstract member updateWCondition : string -> unit

type IWeather =
    abstract member registerObserver : IWeatherObserver -> unit
    abstract member removeObserver   : IWeatherObserver -> unit
    abstract member notifyObservers  : unit

type Weather(cityName : string) as this =
    class
        let mutable observers = []
        let mutable temp       = 0
        let mutable wCondition = ""
        let cityName = cityName
        let timer = new Timer(updateTimeMS)
        do
            timer.Elapsed |> Event.add this.Update
            timer.Start()
        interface IWeather with
            member x.registerObserver(o) = observers <- o :: observers
            member x.removeObserver(o)   = observers <- List.filter (fun x -> x <> o)  observers 
            member x.notifyObservers     = List.iter (fun (o:IWeatherObserver) -> o.updateTemp(temp);
                                                                                  o.updateWCondition(wCondition);
                                                                                  ) observers
        member private x.Update _ = 
            match cityName with
            | "Saint-Petersburg" -> x.UpdateSPB
            | "Moscow" -> x.UpdateMSC
            | _        -> failwith "I don't know this city."
            (x :> IWeather).notifyObservers
            ()

        member private x.UpdateSPB = 
            temp        <- SaintPbXml.GetSample().Fact.Temperature.Value
            wCondition  <- SaintPbXml.GetSample().Fact.WeatherCondition.Code

        member private x.UpdateMSC = 
            temp        <- MoscowXml.GetSample().Fact.Temperature.Value
            wCondition  <- MoscowXml.GetSample().Fact.WeatherCondition.Code
        member x.WCondition = wCondition
        member x.Temp       = temp
    end

let labelFactory( text     : string, fontName  : string,
                  fontSize : Single, fontStyle : FontStyle,
                  xL       : int   , yL        : int,
                  xS       : int   , yS        : int     
                  ) 
                  = new Label(Text      = text, 
                              ForeColor = Color.White, 
                              Font      = new Font(fontName, fontSize, fontStyle, GraphicsUnit.Point, ((byte)(204))),
                              Location  = new Point(xL,yL),
                              Size      = new Size(xS,yS),
                              BackColor = Color.Transparent)

type WeatherMainForm(cityName : string) as this =
    class
        inherit Form(Text = cityName, Width = 480, Height = 300)  
        let cityName   = labelFactory(cityName, "Monotype Corsiva", 27.75F, FontStyle.Italic, 13, 5, 241, 45)
        let temp       = labelFactory("0 C", "Times New Roman", 48.0F, FontStyle.Regular, 8, 45, 123, 73)
        let wCondition = labelFactory("", "Times New Roman", 15.75F, FontStyle.Regular, 17, 137, 120, 23)
        do
              this.FormBorderStyle <- FormBorderStyle.None
              this.GotFocus.AddHandler(new EventHandler(fun _ _ -> (this.FormBorderStyle <- FormBorderStyle.FixedSingle) |> ignore))
              this.LostFocus.AddHandler(new EventHandler(fun _ _ -> (this.FormBorderStyle <- FormBorderStyle.None) |> ignore))
              this.MaximizeBox <- false
              this.Controls.Add(temp) 
              this.Controls.Add(cityName)
              this.Controls.Add(wCondition)
              this.StartPosition <- FormStartPosition.CenterScreen
        interface IWeatherObserver with 
            member x.updateTemp(data)       = if (temp.InvokeRequired) 
                                                then temp.Invoke(new SetIntCallback((x:>IWeatherObserver).updateTemp), data) |> ignore 
                                                else temp.Text <- (data.ToString() + " C")                                           
            member x.updateWCondition(data) = if (wCondition.InvokeRequired) 
                                              then wCondition.Invoke(new SetTextCallback((x:>IWeatherObserver).updateWCondition), data) |> ignore 
                                              else wCondition.Text <- data
                                              match data with
                                                | "overcast" -> this.BackgroundImage <- new Bitmap("./img/weather-overcast.jpg")
                                                | "clear"    -> this.BackgroundImage <- new Bitmap("./img/weather-clear.jpg")
                                                | "snow"     -> this.BackgroundImage <- new Bitmap("./img/weather-snow.jpg")
                                                | "storm"    -> this.BackgroundImage <- new Bitmap("./img/weather-storm.jpg")
                                                | "showers"  -> this.BackgroundImage <- new Bitmap("./img/showers.jpg")
                                                | "fog"      -> this.BackgroundImage <- new Bitmap("./img/weather-fog.jpg")
                                                | _          -> this.BackgroundImage <- new Bitmap("./img/weather-few-clouds.jpg")
        member x.RegisterWeatherStation(weather) = 
               (weather :> IWeather).registerObserver(this)
    end

//Test for the Internet connection
let IsThereInternetConnection() = 
    try
        using (wclient.OpenRead("http://www.google.com")) (fun _ -> true)
    with
        | :? System.Net.WebException as ex -> false

//Check for required materials (images, pics)   
let FilesExist() = Directory.Exists("img") 
                   && Array.fold (fun acc (_, fname) -> acc && IO.File.Exists(fname)) true imgLinks

let downloadImg i = (fun (uri, fname) -> wclient.DownloadFileAsync(new Uri(uri), fname)) imgLinks.[i]
  
type InitForm() as this =
    class
        inherit Form(Text = "Initialization...", Width = 364, Height = 130)
        let pb = new ProgressBar()
        let statusLabel = new Label()
        let cityBox     = new ComboBox()
        let showWeather = new Button()
        let mutable i = 0
        do
            //EventHandlers
            wclient.DownloadProgressChanged.AddHandler(fun o e -> pb.Value <- e.ProgressPercentage)
            wclient.DownloadFileCompleted.AddHandler(fun o e -> pb.Value <- 0; i <- i + 1; 
                                                                  if i <> imgLinks.Length 
                                                                  then downloadImg i;
                                                                       statusLabel.Text <- "Download images: " + (i+1).ToString() + " / " + imgLinks.Length.ToString();
                                                                  else statusLabel.Text <- "Done!"
                                                                       this.StartWheatherMainForm())
            showWeather.Click.AddHandler(fun o e -> let wf = new WeatherMainForm(cityBox.SelectedItem.ToString())
                                                    let ws = new Weather(cityBox.SelectedItem.ToString()) 
                                                    while ws.WCondition = "" do 
                                                        this.Text <- "Loading weather..."    
                                                    this.Visible <- false;
                                                    wf.RegisterWeatherStation(ws)
                                                    wf.ShowDialog() |> ignore)
            //ProgressBar settings
            pb.ForeColor <- Color.LimeGreen
            pb.Location  <- Point(40,40)
            pb.Size      <- Size(255, 25)
            pb.Style     <- ProgressBarStyle.Continuous
            pb.TabIndex  <- 0
            //StatusLabel settings
            statusLabel.Font <- new Font("Times New Roman", 12.0F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)))
            statusLabel.Location <- new Point(34, 18)
            statusLabel.Size <- new Size(250, 21)
            statusLabel.Text <- ""
            //CityBox settings
            cityBox.Location   <- Point(40,40)
            cityBox.DataSource <- [|"Saint-Petersburg"; "Moscow"|]
            cityBox.Visible    <- false
            cityBox.DropDownStyle <- ComboBoxStyle.DropDownList
            //ShowWeather button settings
            showWeather.Location <- new Point(180,40)
            showWeather.Text     <- "Show Weather"
            showWeather.Size     <- new Size(100,20)
            showWeather.Visible  <- false
            //Add controls to the form
            this.Controls.Add(pb)
            this.Controls.Add(statusLabel)
            this.Controls.Add(cityBox)
            this.Controls.Add(showWeather)
            this.ControlBox <- false
            this.FormBorderStyle <- FormBorderStyle.FixedSingle
            this.StartPosition   <- FormStartPosition.CenterScreen
            if not (FilesExist()) then Directory.CreateDirectory("img") |> ignore
                                       downloadImg 0; statusLabel.Text <- "Download images: " + (i+1).ToString() + " / " + imgLinks.Length.ToString();
                                  else this.StartWheatherMainForm()
        member x.StartWheatherMainForm() = 
            pb.Visible <- false
            statusLabel.Text <- "Choose the city"
            cityBox.Visible  <- true
            showWeather.Visible <- true
            ()
    end

[<EntryPoint>]
[<STAThread>]
match IsThereInternetConnection() with
| true  -> Application.Run(new InitForm()) 
| false -> ((MessageBox.Show("Check your Internet connetion.", "Internet connection needed", 
                              MessageBoxButtons.OK, MessageBoxIcon.Error))) |> ignore; 