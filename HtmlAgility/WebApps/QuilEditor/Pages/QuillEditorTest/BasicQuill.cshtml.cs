using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QuillEditor.Pages.CKEditorTest
{
    public class BasicQuillModel : PageModel
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public BasicQuillModel(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        [BindProperty]
        public string htmlTemplate { get; set; }
        public async Task<IActionResult> OnGet()
        {
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "savedContent.html");
            if (System.IO.File.Exists(filePath))
            {
                htmlTemplate = System.IO.File.ReadAllText(filePath);
            }
            else
            {
                htmlTemplate = "File not found.";
            }
            return Page();
        }
        public async Task<IActionResult> OnPost(string editorContent)
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "savedContent.html");
            await System.IO.File.WriteAllTextAsync(path, editorContent);

            return RedirectToPage(); // Redirect to refresh data
        }
    }
}
