open System
open System.IO
open FSharp.Core
open System.Net
open System.Runtime.InteropServices
open HtmlAgilityPack

type ImageURL = ImageURL of string
type Filename = Filename of string

module Handler =
    module private Imported =
        [<DllImport( "user32.dll", CharSet = CharSet.Auto, SetLastError = true)>]
        extern void SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

    let private BaseURL = "https://www.bing.com"

    let getImageURL =
        let req = HttpWebRequest.Create(BaseURL)
        let resp = req.GetResponse()
        let body = resp.GetResponseStream()
        let html = new HtmlDocument()
        use reader = new StreamReader(body)
        do html.LoadHtml(reader.ReadToEnd())
        let doc = html.DocumentNode;
        let link = doc.SelectSingleNode("//link[@id='bgLink']")
        if link = null then
            None
        else
            let url = link.Attributes.["href"].Value
            Some (ImageURL (BaseURL + url))

    let saveImage (ImageURL url) =
        let now = DateTime.Now;
        let date = sprintf "%d-%02d-%02d" now.Year now.Month now.Day
        let homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
        let bingFolder = sprintf @"%s\Pictures\bing\" homeDir

        if not(Directory.Exists(bingFolder)) then
            do Directory.CreateDirectory(bingFolder) |> ignore

        let localFilename = sprintf "%s/%s.jpg" bingFolder date

        if File.Exists(localFilename) then
            Some (Filename localFilename)
        else
            use client = new WebClient()
            do client.DownloadFile(url, localFilename);
            Some (Filename localFilename)

    let private SPI_SETDESKWALLPAPER = 20;
    let private SPIF_UPDATEINIFILE = 1;
    let private SPIF_SENDCHANGE = 2;

    let setWallPaper (Filename img) =
        Imported.SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, img, (SPIF_UPDATEINIFILE ||| SPIF_SENDCHANGE))

module Program =
    [<EntryPoint>]
    let main argv =
        Handler.getImageURL
        |> Option.bind Handler.saveImage
        |> Option.iter Handler.setWallPaper

        0
