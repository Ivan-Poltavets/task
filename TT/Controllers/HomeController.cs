using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TT.Models;
using TT.Models.Context;
using TT.Services;

namespace TT.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly FolderService _service;

    public HomeController(ILogger<HomeController> logger, FolderService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet("{*path}")]
    public async Task<IActionResult> Index(string path = "/")
    {
        var folders = await _service.GetFoldersAsync(path);
        return View(folders);
    }

    [HttpPost]
    public async Task<IActionResult> ImportFoldersAsync(IFormFile formFile, string path)
    {
        var folders = await _service.ImportFoldersAsync(formFile, path);
        await _service.AddFoldersAsync(folders);
        return Redirect(path);
    }

    [HttpPost]
    public async Task<IActionResult> ExportFoldersAsync(string path)
    {
        await _service.ExportFoldersAsync(path);
        return Redirect(path);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

