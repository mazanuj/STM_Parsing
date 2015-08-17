using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace STM_parsing
{
    internal static class GetForums
    {
        internal static async Task<Dictionary<string, string>> GetMainForums(string link, string path)
        {
            return await Task.Run(() =>
            {
                var req = StaticSettings.Request;
                var resp = req.Get(link).ToString();
                var doc = new HtmlDocument();
                doc.LoadHtml(resp);

                try
                {
                    return doc.DocumentNode
                        .Descendants("a")
                        .Where(x => x.Attributes.Contains("href") &&
                                    x.Attributes["href"].Value.StartsWith("forumdisplay.php") &&
                                    x.ParentNode.Name == "h2")
                        .ToDictionary(x => $"{path}\\{x.InnerText.GetValidPath()}",
                            x => @"http://stackthatmoney.com/forum/" + x.Attributes["href"].Value);
                }
                catch (Exception)
                {
                    return new Dictionary<string, string>();
                }
            });
        }
    }
}