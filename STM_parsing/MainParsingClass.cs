using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace STM_parsing
{
    public static class MainParsingClass
    {
        public static async Task GetForum(string name, string pass)
        {
            var forumPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Forum\";

            StaticSettings.Dop = 1;

            await Authorization.GetCookies(name, pass);
            await
                GetSubForum(@"http://stackthatmoney.com/forum/forumdisplay.php?11-General",
                    forumPath + "General");
        }

        private static async Task GetSubForum(string link, string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var subForumLinks = await GetForums.GetMainForums(link, path);
            await GetRecursionParsing(subForumLinks);
        }

        private static async Task GetRecursionParsing(Dictionary<string, string> subForumLinks)
        {
            foreach (var forumLink in subForumLinks)
            {
                if (!Directory.Exists(forumLink.Key))
                    Directory.CreateDirectory(forumLink.Key);

                var links = await GetForums.GetMainForums(forumLink.Value, forumLink.Key);
                await SaveThreads(forumLink.Value, forumLink.Key);
                if (links.Count != 0)
                    await GetRecursionParsing(links);
            }
        }

        private static async Task SaveThreads(string url, string path)
        {
            var linksList = await PageParsing.GetPageLinks(url);

            await linksList.ForEachAsync(StaticSettings.Dop, async link =>
            {
                await PageParsing.SavePage(path, link);
            });
        }
    }
}