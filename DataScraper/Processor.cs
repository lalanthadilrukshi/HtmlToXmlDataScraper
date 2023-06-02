
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;


using HtmlAgilityPack;

namespace DataScraper
{
    public class Processor
    {




        private ISettings _Settings;

        public Processor(ISettings Settings)
        {

            _Settings = Settings;

        }
        private static string ConvertFileUrlToFileName(string FileUrl)
        {
            string fileUrl = FileUrl;
            fileUrl = fileUrl.Replace("//", @"_");
            fileUrl = fileUrl.Replace("/", @"_");
            fileUrl = fileUrl.Replace(".", @"_");
            fileUrl = fileUrl.Replace("=", @"_");
            fileUrl = fileUrl.Replace("?", @"_");
            fileUrl = fileUrl.Replace(":", @"");
            fileUrl = fileUrl.Replace(" ", @"");
            fileUrl = fileUrl.Replace("-", @"");
            return fileUrl;
        }








        public void ExecuteToDownLoadAndSaveFiles()

        {
            string downloadUrl = "https://www.cbssports.com/nfl/teams/BUF/buffalo-bills/roster/";
            string dataDirectoryPath = _Settings.DownLoadedFilesSavePath;
            SaveHtmlDoc(dataDirectoryPath, downloadUrl);
            List<string> PlayerHrefList = getPlayer_hrefs(downloadUrl);

            string playersUrlPath = "https://www.cbssports.com";
            foreach (string href in PlayerHrefList)
            {
                SaveHtmlDoc(dataDirectoryPath + "/players", playersUrlPath + href);
            }
        }
        //public void SaveUrlDoc(string dataDirectoryPath,string downloadUrl)

        //{

        //    HtmlWeb web = new HtmlWeb();
        //    HtmlDocument document = web.Load(downloadUrl);

        //    dataDirectoryPath = dataDirectoryPath.Replace("/", @"\");//## following also ok  // dataDirectoryPath = dataDirectoryPath.Replace("/", "\\");

        //    string fileDirectoryPath = $"{dataDirectoryPath}";

        //    if (!Directory.Exists(fileDirectoryPath))
        //        Directory.CreateDirectory(fileDirectoryPath);


        //    string filename = ConvertFileUrlToFileName(downloadUrl);
        //    document.Save(($"{dataDirectoryPath}\\{filename}.html"));


        //}

        public void SaveHtmlDoc(string dataDirectoryPath, string downloadUrl)

        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(downloadUrl);
            string fileSavePath = GetFileSavePathAndCreateDirectoryIfNotExist(dataDirectoryPath, downloadUrl, "html");
            document.Save(fileSavePath);
        }



        public static string GetFileSavePathAndCreateDirectoryIfNotExist(string dataDirectoryPath, string downloadUrl, string extension)

        {

            //HtmlWeb web = new HtmlWeb();
            //HtmlDocument document = web.Load(downloadUrl);

            dataDirectoryPath = dataDirectoryPath.Replace("/", @"\");//## following also ok  // dataDirectoryPath = dataDirectoryPath.Replace("/", "\\");

            string fileDirectoryPath = $"{dataDirectoryPath}";

            if (!Directory.Exists(fileDirectoryPath))
                Directory.CreateDirectory(fileDirectoryPath);


            string filename = ConvertFileUrlToFileName(downloadUrl);
            //document.Save(($"{dataDirectoryPath}\\{filename}.html"));
            return ($"{dataDirectoryPath}\\{filename}.{extension}");

        }
        public static List<string> getPlayer_hrefs(string url)
        {
            List<string> stringList = new();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(url);

            HtmlNodeCollection rows = document.DocumentNode.SelectNodes("//*[@id=\"TableBase\"]/div/div/table/tbody/tr");

            rows.ToList().ForEach(tr =>
            {
                HtmlNodeCollection playerHref = tr.SelectNodes("td[2]/span[2]/span/a");
                HtmlNode playerHrefNode = playerHref.ElementAt(0);

                string playerHrefUrl = playerHrefNode.GetAttributeValue("href", null);
                // Console.WriteLine(playerHrefUrl);
                stringList.Add(playerHrefUrl);
            });
            return stringList;

        }

        //public  void ConvertPlayerHtmlToXmlNode()
        //  {
        //    string downloadUrl = "https://www.cbssports.com/nfl/teams/BUF/buffalo-bills/roster/";
        //    string dataDirectoryPath = _Settings.DownLoadedFilesSavePath;
        //    //url = "https://www.cbssports.com/nfl/players/2131714/darrel-williams/";

        //    XmlDocument document = new XmlDocument();
        //    XmlElement rootnode = document.CreateElement("players");
        //    document.AppendChild(rootnode);


        //    XmlElement player = document.CreateElement("player");
        //    XmlElement pos = document.CreateElement("pos");
        //    XmlElement dob = document.CreateElement("DOB");
        //    XmlElement college = document.CreateElement("college");
        //    XmlElement ht = document.CreateElement("ht");
        //    XmlElement wt = document.CreateElement("wt");


        //    //*[@id="TableBase"]/div/div/table/tbody/tr[1]/td[2]/span[2]/span/a


        //    rootnode.AppendChild(player);
        //    player.AppendChild(pos);
        //    player.AppendChild(dob);
        //    player.AppendChild(college);
        //    player.AppendChild(ht);
        //    player.AppendChild(wt);

        //    string fileSavePath = GetFileSavePathAndCreateDirectoryIfNotExist(dataDirectoryPath, downloadUrl, "xml");
        //    document.Save(fileSavePath);

        //}

        public void ConvertPlayerHtmlToXmlNode()
        {
            string downloadUrl = "https://www.cbssports.com/nfl/teams/BUF/buffalo-bills/roster/";
            string dataDirectoryPath = _Settings.DownLoadedFilesSavePath;
            //url = "https://www.cbssports.com/nfl/players/2131714/darrel-williams/";

            XmlDocument document = new XmlDocument();
            XmlElement rootnode = document.CreateElement("players");
            document.AppendChild(rootnode);

            appendPlayerNodes(rootnode, downloadUrl, document);
            string fileSavePath = GetFileSavePathAndCreateDirectoryIfNotExist(dataDirectoryPath, downloadUrl, "xml");
            document.Save(fileSavePath);

        }


        private static void appendPlayerNodes(XmlElement rootnode, string url, XmlDocument xmlDocument)
        {
            //List<string> stringList = new();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(url);
           // HtmlNodeCollection rows = document.DocumentNode.SelectNodes("//*[@id=\"TableBase\"]/div/div/table/tbody/tr");
            HtmlNodeCollection rows = document.DocumentNode.SelectNodes("//*[@id=\"TableBase\"]/div[1]/div/table/tbody/tr");

            rows.ToList().ForEach(tr =>
            {




                XmlElement player = xmlDocument.CreateElement("player");
                rootnode.AppendChild(player);

                HtmlNodeCollection playerHref = tr.SelectNodes("td[2]/span[2]/span/a");
                HtmlNode playerHrefNode = playerHref.ElementAt(0);
                string namestring = playerHrefNode.GetDirectInnerText();
                XmlElement nameNode = xmlDocument.CreateElement("name");
                nameNode.InnerText = namestring;
                player.AppendChild(nameNode);


                HtmlNodeCollection playerProperty = tr.SelectNodes("td[3]");
                HtmlNode playerPropertyNode = playerProperty.ElementAt(0);
                string nodestring = playerPropertyNode.GetDirectInnerText();
                XmlElement posNode = xmlDocument.CreateElement("pos");
                posNode.InnerText = nodestring;
                player.AppendChild(posNode);

                
                playerProperty = tr.SelectNodes("td[4]");
                playerPropertyNode = playerProperty.ElementAt(0);
                nodestring = playerPropertyNode.GetDirectInnerText();
                XmlElement htNode = xmlDocument.CreateElement("ht");
                htNode.InnerText = nodestring;
                player.AppendChild(htNode);

                playerProperty = tr.SelectNodes("td[5]");
                playerPropertyNode = playerProperty.ElementAt(0);
                nodestring = playerPropertyNode.GetDirectInnerText();
                XmlElement wtNode = xmlDocument.CreateElement("wt");
                wtNode.InnerText = nodestring;
                player.AppendChild(wtNode);


                playerProperty = tr.SelectNodes("td[6]");
                playerPropertyNode = playerProperty.ElementAt(0);
                nodestring = playerPropertyNode.GetDirectInnerText();
                nodestring= nodestring.Trim();
                string[] stringsList = nodestring.Split(" ");
                nodestring = stringsList[0];
                XmlElement dobNode = xmlDocument.CreateElement("dob");
                dobNode.InnerText = nodestring;
                player.AppendChild(dobNode);

             

                playerProperty = tr.SelectNodes("td[8]");
                playerPropertyNode = playerProperty.ElementAt(0);
                nodestring = playerPropertyNode.GetDirectInnerText();
                XmlElement collegeNode = xmlDocument.CreateElement("college");
                collegeNode.InnerText = nodestring;
                player.AppendChild(collegeNode);


               
               

            });
        }
    }

}




























































