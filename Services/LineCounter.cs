namespace PSPApp.Services;

public class LineCounter : ILineCounter
{
    public LineCountResult CountFromFolder(string folderPath)
    {
        if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
            throw new Exception("Carpeta inválida.");

        var files = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories);
        if (files.Length == 0) throw new Exception("No se encontraron archivos .cs.");

        int code = 0, comments = 0, blanks = 0;
        foreach (var file in files)
            CountFile(file, ref code, ref comments, ref blanks);

        return new LineCountResult { LOCs = code, LOccs = comments, Blanks = blanks };
    }

    private void CountFile(string path, ref int code, ref int comments, ref int blanks)
    {
        bool inBlockComment = false;
        foreach (var raw in File.ReadLines(path))
        {
            var line = raw.Trim();

            if (line.Length == 0) { blanks++; continue; }

            if (inBlockComment)
            {
                comments++;
                if (line.Contains("*/")) inBlockComment = false;
                continue;
            }

            if (line.StartsWith("//")) { comments++; continue; }

            if (line.StartsWith("/*"))
            {
                comments++;
                if (!line.Contains("*/")) inBlockComment = true;
                continue;
            }

            code++;
        }
    }
}
