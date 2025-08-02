public enum DialogBoxCommand
{

}

public readonly struct DialogBox : IControl<DialogBox, DialogBoxCommand>
{
    private readonly nint _handle;
    public DialogBox(nint handle) => this._handle = handle;
    static DialogBox IControl<DialogBox>.Create(nint handle) => new(handle);
    nint IControl<DialogBox>.Handle => this._handle;

    public static bool IsCommandMessage(Message message, out DialogBoxCommand command)
    {
        command = default;
        return false;
    }

    public bool IsCommandSender(Message message, DialogBoxCommand command)
    {
        return false;
    }

    public string? Text
    {
        get => new Control(_handle).Text;
        set => new Control(_handle).Text = value;
    }
}
