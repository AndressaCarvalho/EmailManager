namespace EmailManagerApplication.Models;

public class FileViewModel
{
    public IFormFile? File { get; set; }

    public List<FileModel>? Files { get; set; }
}
