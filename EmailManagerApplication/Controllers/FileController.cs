using Microsoft.AspNetCore.Mvc;

namespace EmailManagerApplication.Controllers;

public class FileController : Controller
{
    private readonly ILogger<FileController> _logger;

    public FileController(ILogger<FileController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Download([FromQuery]string path, string name)
    {
        try 
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);

            return File(fileBytes, "application/force-download", name);
        } 
        catch (Exception e) 
        {
            _logger.LogError(e, "Error on download file!");

            return RedirectToAction("Error", "Home");
        }
    }
}
