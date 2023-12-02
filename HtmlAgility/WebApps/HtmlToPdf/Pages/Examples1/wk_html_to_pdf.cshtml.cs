using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;

namespace HtmlToPdfWeb.Pages.Examples1
{
    public class wk_html_to_pdfModel : PageModel
    {
        private readonly IWebHostEnvironment _env;
        public wk_html_to_pdfModel(IWebHostEnvironment env)
        {
            _env = env;
        }
        public void OnGet()
        {

        }
        public void OnPost()
        {
            string assemblyPath = Path.Combine(_env.WebRootPath, "templates/wkhtmltopdf.exe");

            var scheme = HttpContext.Request.Scheme;
            var host = HttpContext.Request.Host; 
            var pathBase = HttpContext.Request.PathBase;

            // Combine the parts to get the base URL
            var baseUrl = $"{scheme}://{host}{pathBase}";
            string htmlPath = baseUrl+"/templates/01/index.html";
            string pdfPath = Path.Combine(_env.WebRootPath, $"templates/pdfs/index.pdf");

            GeneratePDF(assemblyPath, htmlPath, pdfPath);
        }
        private void GeneratePDF(string assemblyPath, string htmlPath, string pdfPath)
        {
            string options = "--enable-local-file-access --enable-forms --page-size A4 --orientation Landscape";
            ProcessStartInfo info = new ProcessStartInfo();
            info.CreateNoWindow = true;
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.UseShellExecute = false;
            info.FileName = assemblyPath;
            info.Arguments = options + " " + htmlPath + " " + pdfPath;
            Process.Start(info);
        }
    }
}
