using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Spire.Pdf;
using System.Net;

namespace HtmlToPdfWeb.Pages.Examples1
{
    public class SpirePdfModel : PageModel
    {
        private readonly IWebHostEnvironment _env;
        public SpirePdfModel(IWebHostEnvironment env)
        {
            _env = env;
        }
        public void OnGet()
        {
        }
        public void OnPost()
        {
            try
            {
                string htmlUrl = "http://localhost:5058/templates/01/index.html";
                string outputPath = Path.Combine(_env.WebRootPath, $"templates\\pdfs\\indexSpire.pdf");

                ConvertHtmlToPdf(htmlUrl, outputPath);
            }
            catch (Exception ex) { }
        }
        static void ConvertHtmlToPdf(string htmlUrl, string outputPath)
        {
            PdfDocument pdf = new PdfDocument();

            // Load HTML from URL
            //pdf.LoadFromHTML(htmlUrl, false, true, true);

            // Save to PDF file
            pdf.SaveToFile(outputPath, FileFormat.PDF);
        }
    }
}
