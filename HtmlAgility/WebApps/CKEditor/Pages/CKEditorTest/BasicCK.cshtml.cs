using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CKEditor.Pages.CKEditorTest
{
    public class BasicCKModel : PageModel
    {
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPost(string content)
        {
            // Define the path to save the file
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "savedContent.html");

            // Write the content to the file
            await System.IO.File.WriteAllTextAsync(path, content);
            return Page();
        }
    }
}
