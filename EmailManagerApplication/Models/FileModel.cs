namespace EmailManagerApplication.Models;

public class FileModel
{
    public string Name { get; set; }

    public string Path { get; set; }

    public List<string> Lines { get; set; }

    public FileModel(string name, string path, List<string> lines) {
        Name = name;
        Path = path;
        Lines = lines;
    }
}
