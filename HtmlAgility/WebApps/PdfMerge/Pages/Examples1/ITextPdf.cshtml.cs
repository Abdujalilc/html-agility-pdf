using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PdfMerge.Pages.Examples1
{
    public class ITextPdfModel : PageModel
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ITextPdfModel(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public void OnGet()
        {

        }
        public void OnPost()
        {
            try
            {
                string folderPath = Path.Combine(_webHostEnvironment.ContentRootPath, "PdfFiles");

                List<string> fileNames = new List<string>
                {
                    folderPath+ "\\23628.pdf",
                    folderPath+ "\\23629.pdf",
                    folderPath + "\\23630.pdf"
                };

                string outputFileName = "merged.pdf";

                MergePDFs(fileNames, outputFileName);
            }
            catch (Exception ex)
            {

            }
        }
        private void MergePDFs(List<string> fileNames, string outputFileName)
        {
            try
            {
                if (fileNames.Count < 2)
                {
                    Console.WriteLine("At least two PDF files are required for merging.");
                    return;
                }

                PdfDocument outputPdf = new PdfDocument(new PdfWriter(outputFileName));

                foreach (string fileName in fileNames)
                {
                    PdfDocument inputPdf = new PdfDocument(new PdfReader(fileName));
                    //pageFrom, pageTo, finalFile
                    inputPdf.CopyPagesTo(1, inputPdf.GetNumberOfPages(), outputPdf);
                    inputPdf.Close();
                }

                outputPdf.Close();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
