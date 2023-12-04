using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SelectPdf;

namespace HtmlToPdfWeb.Pages.Examples1
{
    public class SelectPdfTestModel : PageModel
    {
        private readonly IWebHostEnvironment _env;
        public SelectPdfTestModel(IWebHostEnvironment env)
        {
            _env = env;
        }
        public void OnGet()
        {

        }
        public void OnPost()
        {
            string htmlUrl = "http://localhost:5058/templates/01/index.html";
            string outputPath = Path.Combine(_env.WebRootPath, $"templates\\pdfs\\indexSelectPdf.pdf");
            

            HtmlToPdf converter = new HtmlToPdf();
            PdfDocument doc = converter.ConvertUrl(htmlUrl);

            doc.Save(outputPath);
            doc.Close();

            Console.WriteLine("PDF generated successfully.");
        }
    }
}
