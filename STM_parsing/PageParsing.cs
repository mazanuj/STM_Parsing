using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Collections.Generic;
using xNet.Net;

namespace STM_parsing
{
    internal static class PageParsing
    {
        internal static async Task SavePage(string path, string link)
        {
            try
            {
                var req = StaticSettings.Request;

                var resp = req.Get(link).ToString();
                path += "\\" + Regex.Match(link, @"(?<=\d+\-).+(?=\-\d+|$)").Value.GetValidPath();

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                try
                {
                    var lastPage = await StaticSettings.GetLastPage(resp);
                    for (var i = lastPage; i > 1; i--)
                        using (var sw = new StreamWriter($"{path}\\page{i}.html", false))
                            sw.Write(req.Get($"{link}/page{i}.html"));

                    throw new Exception();
                }
                catch (Exception)
                {
                    using (var sw = new StreamWriter(path + "\\page1.html", false))
                        sw.Write(resp);
                }
            }
            catch (Exception)
            {
            }
        }

        internal static async Task<List<string>> GetPageLinks(string link)
        {
            var lastPage = 1;
            var doc = new HtmlDocument();
            var req = StaticSettings.Request;
            var linksList = new List<string>();

            do
            {
                var resp = req.Get(link + $"/page{lastPage}").ToString();

                doc.LoadHtml(resp);
                linksList.AddRange(
                    doc.DocumentNode.Descendants("a")
                        .Where(x => x.Attributes.Contains("id") && x.Attributes["id"].Value.StartsWith("thread_title_"))
                        .Select(x => @"http://stackthatmoney.com/forum/" + x.Attributes["href"].Value));

                if (lastPage-- == 1)
                    lastPage = await StaticSettings.GetLastPage(resp);
            } while (lastPage > 1);

            return linksList;
        }
    }
}