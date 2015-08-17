using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace STM_parsing
{
	internal static class SubForumsParsing
	{
		internal static async Task<Dictionary<string, string>> GetSubForums(string address)
		{
			return await Task.Run(() =>
			{
				var req = StaticSettings.Request;
				var resp = req.Get(address).ToString();
				var doc = new HtmlDocument();
				doc.LoadHtml(resp);

				var forums =
					doc.GetElementbyId("forumbits")
						.Descendants("a")
						.Where(x => x.Attributes.Contains("href") &&
									x.Attributes["href"].Value.StartsWith("forumdisplay.php") &&
									x.ParentNode.Name == "h2")
						.ToDictionary(x => x.InnerText, x => @"http://stackthatmoney.com/forum/" + x.Attributes["href"].Value);

				return forums;
			});
		}
	}
}
