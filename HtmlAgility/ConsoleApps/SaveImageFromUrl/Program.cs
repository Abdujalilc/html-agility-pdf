using HtmlAgilityPack;
using System;
using System.IO;
using System.Net;
using System.Xml;

class Program
{
    static void Main()
    {       
        HtmlDocument htmlDoc = new HtmlDocument();
        string htmlcontent = DownloadHtmlContent("http://localhost:3154/Content/HtmlCertificateTemplates/30/index.html");
        //
        htmlDoc.LoadHtml(htmlcontent);

        // Select the element that contains the background image
        HtmlNode firstDiv = htmlDoc.DocumentNode.SelectSingleNode("//div");

        if (firstDiv != null)
        {
            // Get the inline style attribute value
            string inlineStyle = firstDiv.GetAttributeValue("style", "");

            // Extract the URL from the inline style
            string backgroundImageUrl = ExtractBackgroundImageUrl(inlineStyle);

            if (!string.IsNullOrEmpty(backgroundImageUrl))
            {
                try
                {
                    // Download the image bytes
                    byte[] imageBytes = DownloadImageAsBytes(backgroundImageUrl);

                    // Use the image bytes as needed
                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        // Example: Save the image bytes to a file
                        string fileName = Path.GetFileName(backgroundImageUrl);
                        string filePath = Path.Combine(@"C:\Image", fileName); // Replace this with your desired directory
                        File.WriteAllBytes(filePath, imageBytes);
                        Console.WriteLine($"Image downloaded and saved to {filePath}.");
                    }
                    else
                    {
                        Console.WriteLine("Failed to download image bytes.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error downloading or saving image: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("No background image URL found.");
            }
        }
    }
    static string ExtractBackgroundImageUrl(string inlineStyle)
    {
        // Parse the inline style to extract the background-image URL
        string[] styleItems = inlineStyle.Split(';');

        foreach (string item in styleItems)
        {
            if (item.Trim().StartsWith("background-image"))
            {
                // Extract the URL from the background-image property
                int startIndex = item.IndexOf("url(");

                if (startIndex != -1)
                {
                    int endIndex = item.IndexOf(")", startIndex);
                    if (endIndex != -1)
                    {
                        string url = item.Substring(startIndex + 4, endIndex - startIndex - 5).Trim('\'', '"');
                        return url;
                    }
                }
            }
        }

        return null;
    }
    static byte[] DownloadImageAsBytes(string imageUrl)
    {
        // Download the image bytes
        using (WebClient webClient = new WebClient())
        {
            return webClient.DownloadData(imageUrl);
        }
    }
    static string DownloadHtmlContent(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().Result;
            }
            else
            {
                throw new Exception("Failed to download HTML content. Status code: " + response.StatusCode);
            }
        }
    }
}
