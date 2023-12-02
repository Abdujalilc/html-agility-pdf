using DinkToPdf.Contracts;
using DinkToPdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HtmlToPdfWeb.Pages.Examples1
{
    public class DinkToPdfTestModel : PageModel
    {
        private readonly IWebHostEnvironment _env;
        public DinkToPdfTestModel(IWebHostEnvironment env)
        {
            _env = env;
        }
        public void OnGet()
        {

        }
        public void OnPost()
        {
            var converter = new BasicConverter(new PdfTools());

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings() { Top = 10 },
                    Out = Path.Combine(_env.WebRootPath, $"templates\\pdfs\\indexDinkPdf.pdf")
                },
                Objects = {
                    new ObjectSettings()
                    {
                        Page = "http://localhost:5058/templates/01/index.html",
                    },
                },
                
            };
            converter.Convert(doc);
        }      
    }
}
