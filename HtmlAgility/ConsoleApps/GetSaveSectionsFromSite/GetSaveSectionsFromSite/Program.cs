
using System.Net.Http;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using AngleSharp.Html.Parser;

class Program
{
	static async Task Main(string[] args)
	{
		// Change this to your target URL
		string mainUrl = "http://csharp-video-tutorials.blogspot.co.uk/2012/08/sql-server-tutorial-for-beginners.html";
		var httpClient = new HttpClient();

		// Download the main page
		var mainHtml = await httpClient.GetStringAsync(mainUrl);
		var doc = new HtmlDocument();
		doc.LoadHtml(mainHtml);

		// Find the first <ol> with <li> elements containing <a>
		var ol = doc.DocumentNode.SelectSingleNode("//ol[li/a]");
		if (ol == null)
		{
			Console.WriteLine("No <ol> with <li><a> found.");
			return;
		}

		var liNodes = ol.SelectNodes("li[a]");
		if (liNodes == null)
		{
			Console.WriteLine("No <li><a> found in <ol>.");
			return;
		}

		int order = 1;
		foreach (var li in liNodes)
		{
		    var a = li.SelectSingleNode("a");
		    if (a == null) continue;
		    var href = a.GetAttributeValue("href", string.Empty);
		    if (string.IsNullOrEmpty(href)) continue;

			Console.WriteLine($"Processing {order}: {href}");
			try
			{
				var pageHtml = await httpClient.GetStringAsync(href);
				// Use AngleSharp to parse and querySelector
				var parser = new HtmlParser();
				var angleDoc = parser.ParseDocument(pageHtml);
				// Find element with id starting with 'post-body-'
				var postBody = angleDoc.All.FirstOrDefault(e => e.Id != null && e.Id.StartsWith("post-body-"));
				if (postBody == null)
				{
					Console.WriteLine($"No post-body-* found for {href}");
					continue;
				}

				// Get last part of URL (after last /)
				var urlPart = href.TrimEnd('/').Split('/').Last();
				// Clean urlPart for file name
				urlPart = Regex.Replace(urlPart, "[^a-zA-Z0-9_-]", "_");
				var fileName = $"{order:D2}_{urlPart}.html";
				await File.WriteAllTextAsync(fileName, postBody.OuterHtml);
				Console.WriteLine($"Saved: {fileName}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error processing {href}: {ex.Message}");
			}
			order++;
		}
	}
}
