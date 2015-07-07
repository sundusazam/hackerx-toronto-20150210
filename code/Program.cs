using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abot.Crawler;
using Abot.Poco;
using log4net.Config;
using log4net;
using System.IO;
using HtmlAgilityPack;
using Newtonsoft.Json;


namespace WebCrawlerScraper
{


   /// <remarks>
   /// Car class that will be used to store car details
   /// </remarks>
   public class Car
   {
      #region "Constructors"

      /// <summary>
      /// public constructor 
      /// </summary>
      public Car()
      { }

      #endregion

      #region "Properties"

      /// <summary>
      /// represents car title
      /// </summary>
      public string Title { get; set; }

      /// <summary>
      /// represents car price as a string value
      /// </summary>
      public string Price { get; set; }

      /// <summary>
      /// represents car mileage as a string value
      /// </summary>
      public object Kilometer { get; set; }

      #endregion
   }

   class Program : WebCrawler
   {
      private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


      static void Main(string[] args)
      {

         //Web crawler 
         Uri uriToCrawl = GetSiteToCrawl(args);

         WebCrawler crawler;

         crawler = GetDefaultWebCrawler();

         //This is where you process data about specific events of the crawl
         crawler.PageCrawlStartingAsync += crawler_ProcessPageCrawlStarting;
         crawler.PageCrawlCompletedAsync += crawler_ProcessPageCrawlCompleted;
         crawler.PageCrawlDisallowedAsync += crawler_PageCrawlDisallowed;
         crawler.PageLinksCrawlDisallowedAsync += crawler_PageLinksCrawlDisallowed;

         log4net.Config.XmlConfigurator.Configure();
         CrawlResult result = crawler.Crawl(uriToCrawl);

         //Web scraper
         Scraper();

         System.Console.WriteLine("press any key to exit:");
         System.Console.ReadKey();
      }

      private static WebCrawler GetDefaultWebCrawler()
      {
         WebCrawler webCrwl = new Program();
         return webCrwl;
      }


      private static Uri GetSiteToCrawl(string[] args)
      {
         string userInput = "";
         if (args.Length < 1)
         {
            //System.Console.WriteLine("Please enter ABSOLUTE url to crawl:");
           // userInput = System.Console.ReadLine();
            userInput = "http://www.autotrader.ca/cars/honda/civic/qc/lasalle/?prx=100&prv=Quebec&loc=H8N0C5&trans=Automatic&body=Sedan&fuel=Gasoline&sts=New-Used&pRng=%2c10000&hprc=True&wcp=True";
         }
         else
         {
            userInput = args[0];
         }

         if (string.IsNullOrWhiteSpace(userInput))
            throw new ApplicationException("Site url to crawl is as a required parameter");

         return new Uri(userInput);
      }

      private static void Scraper()
      {
         // System.Console.WriteLine("Enter a url:");
         //string url = System.Console.ReadLine();

         string url = "http://www.autotrader.ca/cars/honda/civic/qc/lasalle/?prx=100&prv=Quebec&loc=H8N0C5&trans=Automatic&body=Sedan&fuel=Gasoline&sts=New-Used&pRng=%2c10000&hprc=True&wcp=True";
         string sourceCode = WebScraper.getSourceCode(url);

         var webGet = new HtmlWeb();
         var doc = webGet.Load(url);

         List<Car> cars = new List<Car>(); 
         Car carObj;

         StreamWriter sw = new StreamWriter("scraper.txt", true);
         HtmlNode prices = null;
         HtmlNode kilometers = null;
         HtmlNode removeNode = null;

         foreach (HtmlNode heading in doc.DocumentNode.SelectNodes("//span[@class='resultTitle']"))
         {
            carObj = new Car();
            removeNode = heading.SelectSingleNode("//div[@class='at_strikeThroughPricing']");

            if (removeNode != null)
            {
               removeNode.Remove();
            }

            carObj.Title = !string.IsNullOrEmpty(heading.InnerHtml) ? heading.InnerHtml : "N/A";
            prices = doc.DocumentNode.SelectSingleNode("//div[@class='at_price']");
            kilometers = doc.DocumentNode.SelectSingleNode("//div[@class='at_km']");

            
            carObj.Price = prices != null ?  prices.InnerHtml : "N/A";
            carObj.Kilometer = kilometers != null ? kilometers.InnerHtml : "N/A";
            cars.Add(carObj);

         }
         //in the end, serialize the list of cars and store them as json in the text file!
         string jsonString = JsonConvert.SerializeObject(cars).Trim();
         jsonString = jsonString.Replace("\\r\\n", string.Empty);
         jsonString = jsonString.Replace("\\t", string.Empty);
         sw.WriteLine(jsonString);
         sw.Close();

      }



      static void crawler_ProcessPageCrawlStarting(object sender, PageCrawlStartingArgs e)
      {
         //Process data
      }

      static void crawler_ProcessPageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
      {
         //Process data
      }

      static void crawler_PageLinksCrawlDisallowed(object sender, PageLinksCrawlDisallowedArgs e)
      {
         //Process data
      }

      static void crawler_PageCrawlDisallowed(object sender, PageCrawlDisallowedArgs e)
      {
         //Process data
      }
   }
}
