open System
open System.IO
open System.Net
open System.Windows.Forms
open System.Collections.Concurrent
open System.Text.RegularExpressions

type WebCrawler() as this =
    inherit Form(Text = "Web Crawler", Width = 430, Height = 80)
    
    let attendedLinks    = new ConcurrentDictionary<string, unit>()
    let downloadedImages = new ConcurrentDictionary<string, unit>()
    
    let imgPat  = "http://[a-z-A-Z0-9./_]*\.(:?ico|bmp|jpeg|jpg|png|gif)"
    let hrefPat = "href=\"/[a-z-A-Z0-9./_]*\""

    let infoInput = new TextBox(Left = 50, Top = 5, Width = 200, Text  = "http://habrahabr.ru/")
    let startBtn  = new Button(Left = 270, Top = 4, Width = 100, Text = "Crawle!")
    let timer     = new Timer()
    do 
        if not (Directory.Exists("imgs")) then Directory.CreateDirectory("imgs") |> ignore
        startBtn.Click.AddHandler(fun _ _ -> this.Crawle(infoInput.Text) 
                                             startBtn.Enabled  <- false 
                                             infoInput.Enabled <- false
                                             startBtn.Text <- "crawling..."
                                             timer.Interval <- 5000
                                             timer.Tick 
                                             |> Event.add 
                                                (fun _ -> timer.Stop() 
                                                          MessageBox.Show("Maybe it's time to check your new imgs folder...") |> ignore 
                                                )
                                             timer.Start()
                                )                              
        this.Controls.Add(infoInput)
        this.Controls.Add(startBtn)
        this.StartPosition   <- FormStartPosition.CenterScreen
        this.FormBorderStyle <- FormBorderStyle.FixedSingle
    
    member private x.GetHtml(link : string) =
        async {
             try       let! html = new Uri(link) |> (new WebClient()).AsyncDownloadString
                       return html
             with e -> return String.Empty
        } 
    
    member private x.GetLinks(html : string) =
        async {
            try       let hrefs = [ for x in Regex.Matches(html,hrefPat) -> x.Value ]
                      return [ for h in hrefs -> h.[6 .. (h.Length - 2)] ] 
            with e -> printfn "%A" e.Message
                      return List.Empty
        }

    member private x.GetImages(html : string) =
        async {
            try       return [ for x in Regex.Matches(html, imgPat) -> x.Value ] 
            with e -> printfn "%A" e.Message
                      return List.Empty
        } 

    member private x.DownloadImg(link : string) =
        async {
            try      if not (downloadedImages.ContainsKey(link)) 
                     then downloadedImages.GetOrAdd(link, ()) |> ignore
                     let fileName = "./imgs/" + link.GetHashCode().ToString() + ".jpg"
                     (new WebClient()).DownloadFileAsync(Uri(link), fileName)
            with e -> printf "%A" e.Message
        }

    member private x.StartCrawling(link : string) = 
        async {
            attendedLinks.GetOrAdd(link, ()) |> ignore
            let! html   = x.GetHtml   link
            let! images = x.GetImages html
            let! links  = x.GetLinks  html
            let  notAttendedLink url = not (attendedLinks.ContainsKey(link))
            
            images
                |> Seq.map x.DownloadImg
                |> Async.Parallel
                |> Async.RunSynchronously
                |> ignore

            links 
                |> List.map (fun x -> link + x)
                |> List.filter notAttendedLink
                |> Seq.map x.StartCrawling
                |> Async.Parallel
                |> Async.RunSynchronously
                |> ignore
        }

    member x.Crawle(link : string) =
        try 
             x.StartCrawling link
                |> Async.RunSynchronously
        with _ -> ()

Application.Run(new WebCrawler())

