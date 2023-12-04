using SelectPdf;

class Program
{
    static void Main(string[] args)
    {
        string url = "https://example.com"; // Replace with your URL
        string outputPath = "output.pdf"; // Replace with your desired output path

        HtmlToPdf converter = new HtmlToPdf();
        PdfDocument doc = converter.ConvertUrl(url);

        doc.Save(outputPath);
        doc.Close();

        Console.WriteLine("PDF generated successfully.");
    }
}
