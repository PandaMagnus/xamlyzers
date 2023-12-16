namespace xamalyzers.test;

public class ParseTests
{
    private const string GenericError = "An unspecified error occured.";

    [Fact]
    public void ParseXamlWithErrors_ByFilePath_ReturnsSpecificErrorMessage()
    {
        string message = Analyzer.AnalyzeXaml("D:\\Projects\\xamlyzers\\xamalyzers\\xamalyzers.test\\ExampleXaml.xaml");

        Assert.False(string.IsNullOrWhiteSpace(message), 
            "No message was returned when we expected one.");
        Assert.NotEqual(GenericError, message);
    }

    [Fact]
    public void ParseXamlWithoutErrors_ByFilePath_ReturnsEmptyString()
    {
        string message = Analyzer.AnalyzeXaml("D:\\Projects\\xamlyzers\\GoodXaml");

        Assert.False(string.IsNullOrWhiteSpace(message),
            "No message was returned when we expected one.");
        Assert.NotEqual(GenericError, message);
    }

    [Fact]
    public void ParseXamlWithErrors_ByDirectoryPath_ReturnsSpecificError()
    {
        string message = Analyzer.AnalyzeXaml("D:\\Projects\\xamlyzers\\xamalyzers\\xamalyzers.test");

        Assert.False(string.IsNullOrWhiteSpace(message),
            "No message was returned when we expected one.");
        Assert.NotEqual(GenericError, message);
        Assert.Contains("\r\n", message);
    }

    [Fact]
    public void ParseXamlWithErrors_WithoutPath_ReturnsSpecificError()
    {
        string message = Analyzer.AnalyzeXaml();

        Assert.False(string.IsNullOrWhiteSpace(message), 
            "No message was returned when we expected one.");
        Assert.NotEqual(GenericError, message);
        Assert.Contains("\r\n", message);
    }
}