public enum TextBoxCommand
{
    TextChanged = default
}

public readonly struct TextBoxControl : IControl<TextBoxControl, TextBoxCommand>
{
    private readonly nint _handle;
    public TextBoxControl(nint handle) => this._handle = handle;
    static TextBoxControl IControl<TextBoxControl>.Create(nint handle) => new(handle);
    nint IControl<TextBoxControl>.Handle => _handle;

    public static bool IsCommandMessage(Message message, out TextBoxCommand command)
    {
        if (message.msg == WM_COMMAND && new DWord(message.wParam).High == EN_CHANGE)
        {
            command = TextBoxCommand.TextChanged;
            return true;
        }
        command = default;
        return false;
    }

    public bool IsCommandSender(Message message, TextBoxCommand command)
    {
        return command switch
        {
            TextBoxCommand.TextChanged => message.lParam == _handle,
            _ => false
        };
    }

    [PropertyChangedCommand<TextBoxCommand>(TextBoxCommand.TextChanged)]
    public string? Text
    {
        get => new Control(_handle).Text;
        set => new Control(_handle).Text = value;
    }
}
