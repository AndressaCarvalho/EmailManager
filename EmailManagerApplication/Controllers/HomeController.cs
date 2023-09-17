using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EmailManagerApplication.Models;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;

namespace EmailManagerApplication.Controllers;

public class HomeController : Controller
{
    private readonly IWebHostEnvironment _hostingEnv;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IWebHostEnvironment hostingEnv, ILogger<HomeController> logger)
    {
        _hostingEnv = hostingEnv;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SplitFile([FromForm]FileViewModel model)
    {
        try 
        {
            var splitedOriginalFileName = Path.GetFileName(model.File.FileName).Split('.');
            var originalFileName = splitedOriginalFileName[0] + "-" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + "." + splitedOriginalFileName[1];
            var originalFilePath = Path.Combine(_hostingEnv.WebRootPath, "uploadedFiles", originalFileName);

            using (var fileSteam = new FileStream(originalFilePath, FileMode.Create))
            {
                await model.File.CopyToAsync(fileSteam);
            }
            
            var originalFile = new StreamReader(originalFilePath).ReadToEnd();
            List<string> listNewFileLinesContents = new List<string>();
            FileViewModel newFileViewModel = new FileViewModel 
            {
                Files = new List<FileModel>()
            };
            var linesContents = originalFile.Replace("\r", "").Split('\n');
            linesContents = linesContents.Select(l => l.Trim()).Where(IsValidEmail).Distinct().ToArray();
            var dateTimeToNewFileName = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            int countLines = 0, countNewFileLines = 0, newFileNumberName = 1, quantLines = linesContents.Count() - 1;
            string newFileName = string.Empty, newFilePath = string.Empty;

            while (countLines <= quantLines) 
            {
                countNewFileLines++;

                if (countNewFileLines == 6) 
                {
                    countNewFileLines = 1;
                    newFileNumberName++;
                    dateTimeToNewFileName = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                }

                newFileName = "file" + newFileNumberName + "-" + dateTimeToNewFileName + ".txt";
                newFilePath = Path.Combine(_hostingEnv.WebRootPath, "generatedFiles", newFileName);

                using (StreamWriter outputFile = new StreamWriter(newFilePath, true))
                {
                    outputFile.WriteLine(linesContents[countLines]);
                }

                listNewFileLinesContents.Add(linesContents[countLines]);

                countLines++;

                if (countLines > quantLines || countNewFileLines == 5) 
                {
                    FileModel newFile = new FileModel(newFileName, newFilePath, listNewFileLinesContents);
                    newFileViewModel.Files.Add(newFile);

                    listNewFileLinesContents = new List<string>();
                }
            }

            return View("../File/Index", newFileViewModel);
        } 
        catch (Exception e) 
        {
            _logger.LogError(e, "Error generating files!");

            return RedirectToAction("Error");
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private bool IsValidEmail(string email)
	{
		string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
		
		if (string.IsNullOrEmpty(email))
			return false;
		
		Regex regex = new Regex(emailPattern);
		return regex.IsMatch(email);
	}
}
