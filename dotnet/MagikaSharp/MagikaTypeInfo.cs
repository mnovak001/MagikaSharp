namespace MagikaSharp;

public sealed class MagikaTypeInfo
{
    public string Label { get; init; }
    public string MimeType { get; init; }
    public string Group { get; init; }
    public string Description { get; init; }
    public string[] Extensions { get; init; }
    public bool IsText { get; init; }
}