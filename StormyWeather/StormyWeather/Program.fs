// Когда я начинал это писать, только Бог и я понимали, что я делаю
// Сейчас остался только Бог (с)

open System
open System.Windows.Forms
open System.Text.RegularExpressions
open System.Drawing
open System.IO
open System.Net
open System.Xml.Linq
open System.Timers
open FSharp.Data
open System.ComponentModel
open System.Diagnostics

let  updateTimeMS    = 1000.0
let  wclient         = new WebClient()
let  imgLinks        = [| ("https://www.dropbox.com/s/ivt1ej3673ql7fa/weather-clear.jpg?dl=1",            "./img/weather-clear.jpg");
                          ("https://www.dropbox.com/s/7j92yxq69xqtewy/weather-clear-night.jpg?dl=1",      "./img/weather-clear-night.jpg");
                          ("https://www.dropbox.com/s/njtmgehmc2gro2x/weather-few-clouds.jpg?dl=1",       "./img/weather-few-clouds.jpg");
                          ("https://www.dropbox.com/s/ghbhh1anon0gwld/weather-few-clouds-night.jpg?dl=1", "./img/weather-few-clouds-night.jpg");
                          ("https://www.dropbox.com/s/5dnhu02r0gzgn6s/weather-fog.jpg?dl=1",              "./img/weather-fog.jpg");
                          ("https://www.dropbox.com/s/naddjzowxbsf2b0/weather-overcast.jpg?dl=1",         "./img/weather-overcast.jpg");
                          ("https://www.dropbox.com/s/7eaf9rzwt93bl8r/weather-showers.jpg?dl=1",          "./img/weather-showers.jpg");
                          ("https://www.dropbox.com/s/xs2zlncbobdd4gb/weather-snow.jpg?dl=1",             "./img/weather-snow.jpg");
                          ("https://www.dropbox.com/s/qixtjpm0mk3zavg/weather-storm.jpg?dl=1",            "./img/weather-storm.jpg")
                       |]

type weatherXml      = XmlProvider<"http://export.yandex.ru/weather-ng/forecasts/26063.xml">
type SetIntCallback  = delegate of int -> unit
type SetTextCallback = delegate of string -> unit

type IWeatherObserver = 
    abstract member updateTemp       : int    -> unit
    abstract member updateWCondition : string -> unit

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
                                                                                  o.updateWCondition(wCondition);
                                                                                  ) observers
        member private x.Update _ = 
            temp        <- weatherXml.GetSample().Fact.Temperature.Value
            wCondition  <- weatherXml.GetSample().Fact.WeatherCondition.Code
            (x :> IWeather).notifyObservers
            ()
        member x.TESTGetData() =
            temp
    end

let labelFactory( text     : string, fontName  : string,
                  fontSize : Single, fontStyle : FontStyle,
                  xL        : int   , yL         : int,
                  xS        : int   , yS         : int     ) = new Label(Text    = text, ForeColor = Color.White, 
                                                                         Font      = new Font(fontName, fontSize, fontStyle, GraphicsUnit.Point, ((byte)(204))),
                                                                         Location  = new Point(xL,yL),
                                                                         Size      = new Size(xS,yS),
                                                                         BackColor = Color.Transparent)

type WeatherMainForm() as this =
    class
        inherit Form(Text = "Saint-Petersburg Weather", Width = 480, Height = 300) 
        let weather    = new Weather() 
        let cityName   = labelFactory("Saint-Petersburg", "Monotype Corsiva", 27.75F, FontStyle.Italic, 13, 5, 241, 45)
        let temp       = labelFactory("0 C", "Times New Roman", 48.0F, FontStyle.Regular, 8, 45, 123, 73)
        let wCondition = labelFactory("", "Times New Roman", 15.75F, FontStyle.Regular, 17, 137, 120, 23)
        do
              (weather :> IWeather).registerObserver(this)
              this.FormBorderStyle <- FormBorderStyle.None
              this.GotFocus.AddHandler(new EventHandler(fun _ _ -> (this.FormBorderStyle <- FormBorderStyle.FixedSingle) |> ignore))
              this.LostFocus.AddHandler(new EventHandler(fun _ _ -> (this.FormBorderStyle <- FormBorderStyle.None) |> ignore))
              this.MaximizeBox <- false
              this.Controls.Add(temp) 
              this.Controls.Add(cityName)
              this.Controls.Add(wCondition)
              this.BackgroundImage <- new Bitmap("./img/weather-showers.jpg")
        interface IWeatherObserver with 
            member x.updateTemp(data)       = if (temp.InvokeRequired) 
                                                then temp.Invoke(new SetIntCallback((x:>IWeatherObserver).updateTemp), data) |> ignore 
                                                else temp.Text <- (data.ToString() + " C")
            member x.updateWCondition(data) = if (wCondition.InvokeRequired) 
                                              then wCondition.Invoke(new SetTextCallback((x:>IWeatherObserver).updateWCondition), data) |> ignore 
                                              else wCondition.Text <- data
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
        let wf          = new WeatherMainForm()
        let mutable i = 0
        do
            wclient.DownloadProgressChanged.AddHandler(fun o e -> pb.Value <- e.ProgressPercentage)
            wclient.DownloadFileCompleted.AddHandler  (fun o e -> pb.Value <- 0; i <- i + 1; 
                                                                  if i <> imgLinks.Length 
                                                                  then downloadImg i;
                                                                       statusLabel.Text <- "Download images: " + (i+1).ToString() + " / " + imgLinks.Length.ToString();
                                                                  else statusLabel.Text <- "Done!"; this.Visible <- false; wf.Show())
            //ProgressBar settings
            pb.ForeColor <- Color.LimeGreen
            pb.Location  <- Point(40,40)
            pb.Size      <- Size(255, 25)
            pb.Style     <- ProgressBarStyle.Continuous
            pb.TabIndex  <- 0
            //Status Label settings
            statusLabel.Font <- new Font("Times New Roman", 12.0F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)))
            statusLabel.Location <- new Point(34, 18)
            statusLabel.Size <- new System.Drawing.Size(250, 21);
            statusLabel.TabIndex <- 1;
            statusLabel.Text <- "";
            //Add controls to the form
            this.Controls.Add(pb)
            this.Controls.Add(statusLabel)
            this.ControlBox      <- false;
            this.FormBorderStyle <- FormBorderStyle.FixedSingle;
            this.StartPosition   <- FormStartPosition.CenterScreen;
            if not (FilesExist()) then Directory.CreateDirectory("img") |> ignore
                                       downloadImg 0; statusLabel.Text <- "Download images: " + (i+1).ToString() + " / " + imgLinks.Length.ToString();
                                  else this.VisibleChanged.AddHandler(fun _ _ -> this.Visible <- false); wf.Show()
    end

[<EntryPoint>]
[<STAThread>]
match IsThereInternetConnection() with
| true  -> try Application.Run(new InitForm()) with :? ObjectDisposedException as ex -> ()
| false -> ((MessageBox.Show("Check your Internet connetion.", "Internet connection needed", 
                              MessageBoxButtons.OK, MessageBoxIcon.Error))) |> ignore; 

//let b  = new BackgroundWorker()
//
//type exec = delegate of unit -> int
//
//let a = new exec(fun _ ->
//                   let mutable  t = ws.TESTGetData();
//                   while t = 0 do
//                    t <- ws.TESTGetData();
//                   t
//                  )
//b.DoWork.Add(fun _ -> a.Invoke() |> ignore)
//b.RunWorkerCompleted.Add(fun _ -> p.Close())
