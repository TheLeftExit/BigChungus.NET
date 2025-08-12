public sealed class DialogProperties
{
    private readonly DlgTemplate _template;

    public DialogProperties(DlgTemplate template)
    {
        _template = template;
    }

    public string? Text
    {
        get => _template.WindowText;
        set => _template.WindowText = value;
    }

    public SizeDLU Size
    {
        get => new SizeDLU(_template.Width, _template.Height);
        set => (_template.Width, _template.Height) = (value.Width, value.Height);
    }
}
