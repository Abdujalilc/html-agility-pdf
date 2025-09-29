namespace Services
{
    using System.Text.RegularExpressions;
    using HtmlAgilityPack;
    using AngleSharp.Html.Parser;

    public interface IMssqlTutorialService
    {
        Task GetMssqlTutorials(string mainUrl, string? outputDir = null);
    }

    public class MssqlTutorialService : IMssqlTutorialService
    {
        private readonly HttpClient _httpClient;
        public MssqlTutorialService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");
        }

        public async Task GetMssqlTutorials(string mainUrl, string? outputDir = null)
        {
            string mainHtml = string.Empty;
            try
            {
                mainHtml = await _httpClient.GetStringAsync(mainUrl);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP error for main URL: {ex.Message}");
                return;
            }
            var doc = new HtmlDocument();
            doc.LoadHtml(mainHtml);

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

            var urlFolder = mainUrl.TrimEnd('/').Split('/').Last();
            urlFolder = Regex.Replace(urlFolder, "[^a-zA-Z0-9_-]", "_");
            if (string.IsNullOrEmpty(outputDir))
                outputDir = Path.Combine(AppContext.BaseDirectory, urlFolder);
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
                    var pageHtml = await _httpClient.GetStringAsync(href);
                    var parser = new HtmlParser();
                    var angleDoc = parser.ParseDocument(pageHtml);
                    var postBody = angleDoc.All.FirstOrDefault(e => e.Id != null && e.Id.StartsWith("post-body-"));
                    if (postBody == null)
                    {
                        Console.WriteLine($"No post-body-* found for {href}");
                        continue;
                    }
                    var urlPart = href.TrimEnd('/').Split('/').Last();
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
}
