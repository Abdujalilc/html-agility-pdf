using SelectPdf;
// create a new pdf document
PdfDocument doc = new PdfDocument();

// create a new pdf page
PdfPage page = doc.AddPage();

// create a new pdf font
PdfFont font = doc.AddFont(PdfStandardFont.Helvetica);
font.Size = 20;

// create a new pdf brush
PdfBrush brush = new PdfSolidBrush(System.Drawing.Color.Black);

// create a new pdf text element
PdfTextElement text = new PdfTextElement("Hello, World!", font, brush);

// add the text element to the pdf page
page.Add(text);

// create a new html to pdf converter
HtmlToPdf converter = new HtmlToPdf();

// set the converter options
converter.Options.PdfPageSize = PdfPageSize.A4;
converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;
converter.Options.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;
converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;

// convert html code to a pdf page
string html = "<html><body><h1>This is a HTML code</h1><p>This is a paragraph</p></body></html>";
PdfPage htmlPage = converter.ConvertHtmlString(html, doc);

// save the pdf document to a file
doc.Save("output.pdf");

// close the pdf document
doc.Close();
