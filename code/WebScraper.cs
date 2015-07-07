using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace WebCrawlerScraper
{
   class WebScraper
   {
      public static string getSourceCode(string url)
      {
         HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
         HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

         StreamReader sr = new StreamReader(resp.GetResponseStream());
         string sourceCode = sr.ReadToEnd();
         sr.Close();
         resp.Close();
         return sourceCode;

      }
   }
}
