using System;
using System.Threading.Tasks;
using STM_parsing;

namespace Tester
{
	internal static class Program
	{
		private static void Main()
		{
            Task.Run(async () =>
			{
				await MainParsingClass.GetForum("", "");
			}).Wait();
			Console.ReadKey();
		}
	}
}