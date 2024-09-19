using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PuppeteerSharp.Media;
using PuppeteerSharp;
using Microsoft.AspNetCore.Mvc;
namespace HtmlToPdfWeb.Pages.Examples1
{
    public class PuppeteerFormToPdfModel : PageModel
    {
        private readonly IWebHostEnvironment _env;
        public PuppeteerFormToPdfModel(IWebHostEnvironment env)
        {
            _env = env;
        }
        public void OnGet()
        {

        }
        public async Task<IActionResult> OnPost(string UniqueRegNumber, DateTime DateGenerated, string FullName, string AcademicYear, string Course, int Level)
        {
            // Load HTML template
            string htmlTemplatePath = Path.Combine(_env.WebRootPath, $"templates/form_to_pdf/index.html");
            string htmlTemplateContent = System.IO.File.ReadAllText(htmlTemplatePath);
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlTemplateContent);

            // Change HTML content
            ChangeSpan(htmlDoc, "uniqueRegNumber", UniqueRegNumber);
            ChangeSpan(htmlDoc, "dateGenerated", DateGenerated.ToString("dd.MM.yyyy"));
            ChangeSpan(htmlDoc, "courseName", Course);
            ChangeSpan(htmlDoc, "fullName", FullName);
            ChangeSpan(htmlDoc, "levelHemis", (Level - 2).ToString());
            ChangeSpan(htmlDoc, "level", Level.ToString());

            // Save modified HTML
            string generatedHtmlPath = Path.Combine(_env.WebRootPath, $"templates/form_to_pdf/result_html/{UniqueRegNumber}");
            if (!Directory.Exists(generatedHtmlPath))
                Directory.CreateDirectory(generatedHtmlPath);

            string htmlFilePath = Path.Combine(generatedHtmlPath, "index.html");
            if (System.IO.File.Exists(htmlFilePath))
                System.IO.File.Delete(htmlFilePath);
            htmlDoc.Save(htmlFilePath);

            // Set up Puppeteer options
            var launchOptions = new LaunchOptions
            {
                Headless = true,
                ExecutablePath = @"c:\Program Files\Google\Chrome\Application\chrome.exe"
            };

            string generatedPdfPath = Path.Combine(_env.WebRootPath, $"templates/form_to_pdf/result_pdf/{UniqueRegNumber}.pdf");

            using (IBrowser browser = await Puppeteer.LaunchAsync(launchOptions))
            {
                if (!Directory.Exists(Path.GetDirectoryName(generatedPdfPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(generatedPdfPath));

                GeneratePDF(htmlFilePath, generatedPdfPath, browser);
            }

            // Return PDF as a file result
            byte[] pdfBytes = await System.IO.File.ReadAllBytesAsync(generatedPdfPath);
            return File(pdfBytes, "application/pdf", $"{UniqueRegNumber}.pdf");
        }

        void ChangeSpan(HtmlDocument htmlDoc, string elementId, string newValue)
        {
            HtmlNode divElement = htmlDoc.DocumentNode.SelectSingleNode("//span[@id='" + elementId + "']");

            if (divElement != null)
            {
                divElement.InnerHtml = newValue;
            }
        }
        void GeneratePDF(string htmlPath, string pdfPath, IBrowser browser)
        {
            try
            {
                using (IPage page = browser.NewPageAsync().GetAwaiter().GetResult())
                {
                    page.GoToAsync(htmlPath).GetAwaiter().GetResult();

                    //page.WaitForSelectorAsync("link[href='styles.css']").GetAwaiter().GetResult();
                    //page.WaitForSelectorAsync(".wu-img-qr").GetAwaiter().GetResult();
                    //page.WaitForSelectorAsync(".background-img").GetAwaiter().GetResult();

                    page.PdfAsync(pdfPath, new PdfOptions
                    {
                        Format = PaperFormat.A4,
                        Landscape = false,
                    }).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
