using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace STM_parsing
{
	internal static class Authorization
	{
		internal static async Task GetCookies(string userName, string userPass)
		{
			await Task.Run(() =>
			{
				var req = StaticSettings.Request;
				var resp =
					req.Get(
						@"http://stackthatmoney.com/amember/protect/new-rewrite")
						.ToString();

				var doc = new HtmlDocument();
				doc.LoadHtml(resp);

				var login_attempt_id =
					doc.DocumentNode.Descendants("input")
						.First(x => x.Attributes.Contains("name") && x.GetAttributeValue("name", string.Empty) == "login_attempt_id")
						.GetAttributeValue("value", string.Empty);

				req.AddParam("amember_login", userName);
				req.AddParam("amember_pass", userPass);
				req.AddParam("remember_login", "1");
				req.AddParam("login_attempt_id", login_attempt_id);
				req.AddParam("amember_redirect_url", @"http://stackthatmoney.com/forum/forum.php");

				req.Post(@"http://stackthatmoney.com/amember/login").None();
				StaticSettings.Request = req;
            });
		}
	}
}