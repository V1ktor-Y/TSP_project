using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tsp.Models;
using tsp.Services;

namespace tsp.Controllers;

[Authorize]
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
        var accountId = CurrentAccountId();
        if (String.IsNullOrWhiteSpace(searchString))
        {
            files = await _fileService.GetFilesAsync(accountId);
        }
        else
        {
            files = await _fileService.GetByNameAsync(searchString, accountId);
        }
        return View(files);
    }

    // 2. Handle the Upload
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(IFormFile uploadedFile)
    {
        if (uploadedFile != null && uploadedFile.Length > 0)
        {
            using (var memoryStream = new MemoryStream())
            {
                await uploadedFile.CopyToAsync(memoryStream);
                await _fileService.SaveFileAsync(uploadedFile.FileName, memoryStream.ToArray(), CurrentAccountId());
            }
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Download(int id)
    {
        VFile? file = await _fileService.GetAsync(id, CurrentAccountId());
        if (file == null) return NotFound();
        return File(file.FileData!, "application/octet-stream", file.FileName);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _fileService.DeleteFileAsync(id, CurrentAccountId());
        return RedirectToAction(nameof(Index));
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private int CurrentAccountId()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
