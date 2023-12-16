using System.IO;
using System.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace xamalyzers;

public static class Analyzer
{
    public static string AnalyzeXaml(string path)
    {
        FileInfo file = new(path);
        DirectoryInfo directory = new(path);

        string error = "An unspecified error occured. ";
        if (file.Exists)
        {
            error = CheckXamlForErrors(file);
        }
        else if (directory.Exists)
        {
            error = CheckXamlForErrors(directory);
        }
        else
        {
            error = $"File or directory not found: {path}";
        }

        return error;
    }

    public static string AnalyzeXaml()
    {
        string error = "An unspecified error occured.";
        string root = AppDomain.CurrentDomain.BaseDirectory;
        bool foundSln = false;
        do
        {
            foundSln = Directory.EnumerateFiles(root, "*.sln").Any();
            root = Path.Combine(root, "..");
        }
        while (!foundSln && Directory.Exists(root));

        root = root.TrimEnd('.');
        
        DirectoryInfo searchDirectory = new(root);
        error = CheckXamlForErrors(searchDirectory);

        return error;
    }

    private static string CheckXamlForErrors(DirectoryInfo searchDirectory)
    {
        string error = $"An unspecified error occurred when recusrively searching directory {searchDirectory.FullName} for *.xaml files.";
        IEnumerable<FileInfo> files = searchDirectory.EnumerateFiles("*.xaml", SearchOption.AllDirectories);
        List<string> errors = new();
        foreach (FileInfo foundFile in files)
        {
            errors.Add(CheckXamlForErrors(foundFile));
        }
        if(errors.Any())
        {
            error = string.Join("\r\n", errors);
        }
        return error;
    }

    // Or should it take in a FileInfo?
    private static string CheckXamlForErrors(FileInfo file)
    {
        using FileStream stream = file.OpenRead();
        XmlDocument xaml = new();
        xaml.Load(stream);
        XmlNodeList? nodes = xaml.SelectNodes("//*");

        if (nodes is null
            || nodes.Count is 0)
        {
            return $"{file.FullName}: No elements found in xaml.";
        }

        List<string> errors = new();

        foreach (XmlNode node in nodes)
        {
            XmlAttributeCollection? attributes = node.Attributes;
            if (attributes is not null)
            {
                foreach (XmlAttribute attribute in attributes)
                {
                    // Does this need to be configurable?
                    if (attribute.Name.ToUpperInvariant() is "TEXT"
                        && !attribute.Value.StartsWith("{")
                        && !attribute.Value.EndsWith("}"))
                    {
                        errors.Add($"{file.FullName}: Node appears to use a hard coded vallue for attribute {attribute.Name}. Node: {node.OuterXml}");
                    }
                }
            }
        }

        return string.Join("\r\n", errors);
    }
}
