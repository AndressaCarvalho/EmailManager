using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EmailManagerApplication.Models;
using Microsoft.AspNetCore.Authorization;

namespace EmailManagerApplication.Controllers;

public class FileController : Controller
{
    private readonly IWebHostEnvironment _hostingEnv;
    private readonly ILogger<HomeController> _logger;

    public FileController(IWebHostEnvironment hostingEnv, ILogger<HomeController> logger)
    {
        _hostingEnv = hostingEnv;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Download([FromQuery]string path, string name)
    {
        byte[] fileBytes = System.IO.File.ReadAllBytes(path);

        return File(fileBytes, "application/force-download", name);
    }
}
