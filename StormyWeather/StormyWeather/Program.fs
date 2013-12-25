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

let  updateTimeMS    = 60000.0
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
        member x.TESTGetData() =
            temp
    end

//Test for internet connection
let IsThereInternetConnection() = 
    try
        using (wclient.OpenRead("http://www.google.com")) (fun _ -> true)
    with
        | :? System.Net.WebException as ex -> false

//Check for required materials (images, pics)   
let FilesExist() = Directory.Exists("img") 
                   && Array.fold (fun acc (_, fname) -> acc && IO.File.Exists(fname)) true imgLinks

type WeatherMainForm() as this =
    class
        inherit Form(Text = "Saint-Petersburg Weather", Width = 480, Height = 272) 
        let weather  = new Weather() 
        //TODO add exceptions
        let request  = WebRequest.Create("http://s5.goodfon.ru/crop/576328.jpg?flag=false&w=500&h=283&x=0&y=0&grayscale=&r=0.2441&resolution=480x272");
        let response = request.GetResponse();
        let stream   = response.GetResponseStream();
        //
        let bitmap   = new Bitmap(stream)
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

let downloadImg i = (fun (uri, fname) -> wclient.DownloadFileAsync(new Uri(uri), fname)) imgLinks.[i]
  
type InitForm() as this =
    class
        inherit Form(Text = "Initialization...", Width = 364, Height = 130)
        let pb = new ProgressBar()
        let statusLabel = new Label()
        let mutable i = 0
        do
            wclient.DownloadProgressChanged.AddHandler(fun o e -> pb.Value <- e.ProgressPercentage)
            wclient.DownloadFileCompleted.AddHandler  (fun o e -> pb.Value <- 0; i <- i + 1; 
                                                                  if i <> imgLinks.Length 
                                                                  then downloadImg i;
                                                                       statusLabel.Text <- "Download images: " + (i+1).ToString() + " / " + imgLinks.Length.ToString();
                                                                  else statusLabel.Text <- "Done!"; this.Visible <- false; Application.Run(new WeatherMainForm()))
        //ProgressBar settings
            pb.ForeColor <- Color.LimeGreen
            pb.Location  <- Point(40,40)
            pb.Size      <- Size(255, 25)
            pb.Style     <- ProgressBarStyle.Continuous
            pb.TabIndex  <- 0
       //Status Label settings
            statusLabel.Font <- new Font("Times New Roman", 12.0F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)))
            statusLabel.Location <- new Point(34, 18)
            statusLabel.Name <- "label1";
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
                                  else this.Visible <- false; 
                                       Application.Run(new WeatherMainForm())
                                       this.Close()
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
