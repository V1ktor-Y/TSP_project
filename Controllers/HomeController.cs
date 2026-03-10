using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using tsp.Models;
using tsp.Services;

namespace tsp.Controllers;

public class HomeController : Controller
{
    private readonly IVFileService _fileService;

    public HomeController(IVFileService fileService)
    {
        _fileService = fileService;
    }
    // 1. List all files
    public async Task<IActionResult> Index(string? searchString)
    {
        IEnumerable<VFile> files;
        if (String.IsNullOrWhiteSpace(searchString))
        {
            files = await _fileService.GetFilesAsync();
        }
        else
        {
            files = await _fileService.GetByNameAsync(searchString);
        }
        return View(files);
    }

    // 2. Handle the Upload
    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile uploadedFile)
    {
        if (uploadedFile != null && uploadedFile.Length > 0)
        {
            using (var memoryStream = new MemoryStream())
            {
                await uploadedFile.CopyToAsync(memoryStream);
                await _fileService.SaveFileAsync(uploadedFile.FileName, memoryStream.ToArray());
            }
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Download(int id)
    {
        VFile file = await _fileService.GetAsync(id);
        if (file == null) return NotFound();
        return File(file.FileData!, "application/octet-stream", file.FileName);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
