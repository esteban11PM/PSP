namespace PSPApp.Services;

public interface ILineCounter
{
    LineCountResult CountFromFolder(string folderPath);
}

public class LineCountResult
{
    public int LOCs { get; set; }
    public int LOccs { get; set; }
    public int Blanks { get; set; }
}
