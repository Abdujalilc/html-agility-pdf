using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace HtmlToPdfWeb.Pages.Examples1
{
    public class PuppeteerHtmlToPdfModel : PageModel
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;
        public PuppeteerHtmlToPdfModel(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }
        public void OnGet()
        {

        }
        public async Task OnPost()
        {
            try
            {
                //string PathChrome = System.Configuration.ConfigurationManager.AppSettings.Get("PathChrome"); //for framwework
                //string PathChrome = _configuration["PathChrome"] ?? "";
                var launchOptions = new LaunchOptions
                {
                    Headless = true,
                    ExecutablePath = @"c:\Program Files\Google\Chrome\Application\chrome.exe"
                };

                using (IBrowser browser = await Puppeteer.LaunchAsync(launchOptions))
                using (IPage page = await browser.NewPageAsync())
                {
                    HttpRequest req = HttpContext.Request;
                    string baseUrl = $"{req.Scheme}://{req.Host}{req.PathBase}";
                    string htmlPath = baseUrl + "/templates/01/index.html";

                    string pdfPath = Path.Combine(_env.WebRootPath, $"templates/pdfs/index.pdf");

                    await page.GoToAsync(htmlPath);

                    await page.WaitForSelectorAsync("link[href='styles.css']");
                    await page.WaitForSelectorAsync(".wu-img-qr");
                    await page.WaitForSelectorAsync(".background-img");

                    {
                        await page.PdfAsync(pdfPath, new PdfOptions
                        {
                            Format = PaperFormat.A4,
                            Landscape = true,
                        });

                        Console.WriteLine($"PDF generated from URL: {pdfPath}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }
}
