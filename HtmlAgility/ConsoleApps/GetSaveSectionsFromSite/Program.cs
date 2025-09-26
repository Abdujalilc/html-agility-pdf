using System.Net.Http;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using AngleSharp.Html.Parser;

class Program
{
	static async Task Main(string[] args)
	{
		// Change this to your target URL
		string mainUrl = "https://csharp-video-tutorials.blogspot.com/p/free-sql-server-video-tutorials-for.html";
		var httpClient = new HttpClient();
		httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");

		// Download the main page
		string mainHtml = null;
		try
		{
			mainHtml = await httpClient.GetStringAsync(mainUrl);
		}
		catch (HttpRequestException ex)
		{
			Console.WriteLine($"HTTP error for main URL: {ex.Message}");
			return;
		}
		var doc = new HtmlDocument();
		doc.LoadHtml(mainHtml);

		Console.WriteLine(mainHtml);

		// Find the first <ol> with <li> elements containing <a>
		var ol = doc.DocumentNode.SelectSingleNode("//ol");
		if (ol == null)
		{
			Console.WriteLine("No <ol> found.");
			return;
		}

		var liNodes = doc.DocumentNode.SelectNodes("//li[a or descendant::a]");
		if (liNodes == null)
		{
			Console.WriteLine("No <li><a> found in <ol>.");
			return;
		}

		var firstLi = doc.DocumentNode.SelectSingleNode("//li[a or descendant::a]");
		if (firstLi != null)
		{
			var a = firstLi.SelectSingleNode(".//a");
			if (a != null)
			{
				var href = a.GetAttributeValue("href", string.Empty);
				Console.WriteLine($"First link: {href}");
			}
		}
		else
		{
			Console.WriteLine("No <li> with <a> found.");
		}

		// Get the folder name from the last part of the main URL
		var urlFolder = mainUrl.TrimEnd('/').Split('/').Last();
		urlFolder = Regex.Replace(urlFolder, "[^a-zA-Z0-9_-]", "_");
		var projectRoot = AppContext.BaseDirectory;
		// Go up until we find the .csproj file (project root)
		while (!Directory.GetFiles(projectRoot, "*.csproj").Any())
		{
			var parent = Directory.GetParent(projectRoot);
			if (parent == null) break;
			projectRoot = parent.FullName;
		}
		string outputDir = args.Length > 0
			? args[0]
			: Path.Combine(AppContext.BaseDirectory, urlFolder);
		Directory.CreateDirectory(outputDir);

		int order = 1;
		foreach (var li in liNodes)
		{
			var a = li.SelectSingleNode(".//a");
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
				var fileName = $"{order:D3}_{urlPart}.html";
				var filePath = Path.Combine(outputDir, fileName);
				await File.WriteAllTextAsync(filePath, postBody.OuterHtml);
				Console.WriteLine($"Saved: {filePath}");
			}
			catch (HttpRequestException httpEx)
			{
				Console.WriteLine($"HTTP error for {href}: {httpEx.Message}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error processing {href}: {ex.Message}");
			}
			order++;
		}
	}
}
