using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using xNet.Net;

namespace STM_parsing
{
    internal static class StaticSettings
    {
        internal static HttpRequest Request { get; set; }
        internal static int Dop { get; set; }

        static StaticSettings()
        {
            Dop = 1;
            Request = new HttpRequest
            {
                AllowAutoRedirect = true,
                Cookies = new CookieDictionary(),
                EnableAdditionalHeaders = true,
                EnableEncodingContent = true,
                MaximumAutomaticRedirections = 50,
                UserAgent = HttpHelper.ChromeUserAgent(),
                Proxy = new HttpProxyClient("127.0.0.1", 8888) //todo
            };
        }

        public static async Task<int> GetLastPage(string response)
        {
            return await Task.Run(() =>
            {
                int lastPage;
                var doc = new HtmlDocument();
                doc.LoadHtml(response);

                try
                {
                    var lastPageElem =
                        doc.DocumentNode.Descendants("a")
                            .First(
                                x =>
                                    x.Attributes.Contains("href") && x.Attributes.Contains("class") &&
                                    x.Attributes["href"].Value.StartsWith(@"javascript:") &&
                                    x.Attributes["class"].Value.StartsWith(@"popupctrl") &&
                                    x.InnerText.StartsWith("Page 1 of"));

                    lastPage = int.Parse(Regex.Match(lastPageElem.InnerText, @"\d+$").Value);
                }
                catch (Exception)
                {
                    lastPage = 1;
                }
                return lastPage;
            });
        }

        public static string GetValidPath(this string str)
        {
            str = Regex.Replace(str, @"(?![- .&])\W", string.Empty);
            return str;
        }

        public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> source)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            foreach (var element in source)
                target.Add(element);
        }

        public static Task ForEachAsync<T>(this IEnumerable<T> source, int dop, Func<T, Task> body)
        {
            return Task.WhenAll(
                from partition in Partitioner.Create(source).GetPartitions(dop)
                select Task.Run(async delegate
                {
                    using (partition)
                        while (partition.MoveNext())
                            await body(partition.Current);
                }));
        }
    }
}