using HtmlAgilityPack;
using Microsoft.Extensions.Configuration; // tools>NugetPackageManager> install package Microsoft.Extensions.Configuration
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration.Json;// tools>NugetPackageManager> install package Microsoft.Extensions.Configuration.Json

namespace DataScraper
{
class Program
{

        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").Build();

            var _Settings = config.GetSection("Settings").Get<ISettings>(); // ## must install nuget package Microsoft.Extensions.Configuration.Binder

            if (!Validator.TryValidateObject(_Settings, new ValidationContext(_Settings), new List<ValidationResult>(), true))
            {
                throw new Exception("Unable to find all settings");
            }

           

                //  Program p = new Program();

                var processor = new Processor(_Settings);
                if (_Settings.Process_Option == 1)
                {
                    processor.ExecuteToDownLoadAndSaveFiles();
                }
                else
                {
                    processor.ConvertPlayerHtmlToXmlNode();
                }
            


        }



        //static void Main(string[] args)
        //{
        //    HtmlWeb web = new HtmlWeb();
        //    HtmlDocument document = web.Load("https://www.cbssports.com/nfl/teams/BUF/buffalo-bills/roster/");
        //    //var Number = document.DocumentNode.SelectNodes("//table/tr/td").First().InnerText;
        //    ////*[@id="TableBase"]/div/div/table/tbody/tr[1]/td[8]
        //    //var rows = document.DocumentNode.SelectNodes("//*[@id=\"TableBase\"]/div/div/table/tbody/tr");
        //  HtmlNodeCollection  rows = document.DocumentNode.SelectNodes("//*[@id=\"TableBase\"]/div/div/table/tbody/tr");
        //    HtmlNode i;
        //    i.
        //    rows.ToList().ForEach(i => Console.WriteLine(i.InnerText));

        //}
    }
}
