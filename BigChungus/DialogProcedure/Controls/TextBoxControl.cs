using System.Linq.Expressions;

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
    public int MaxLength
    {
        get => (int)Win32.SendMessage(_handle, EM_GETLIMITTEXT, 0, 0);
        set => Win32.SendMessage(_handle, EM_SETLIMITTEXT, (nuint)value, 0);
    }
    public char PasswordChar
    {
        get => (char)Win32.SendMessage(_handle, EM_GETPASSWORDCHAR, 0, 0);
        set => Win32.SendMessage(_handle, EM_SETPASSWORDCHAR, value, 0);
    }
}

public static partial class DialogProcedureBuilderExtensions
{
    public static void SetCommand<TViewModel>(
        this IDialogProcedureBuilder<TViewModel> builder,
        DialogItemHandle<TextBox> handle,
        Action<TViewModel> viewModelCommand,
        TextBoxCommand controlCommand = default
    )
        where TViewModel : class
    {
        builder.SetCommand<TViewModel, TextBoxControl, TextBoxCommand>(
            handle.Id,
            viewModelCommand,
            controlCommand
        );
    }

    public static void SetBinding<TViewModel, TValue>(
        this IDialogProcedureBuilder<TViewModel> builder,
        DialogItemHandle<TextBox> handle,
        Expression<Func<TextBoxControl, TValue>> controlPropertySelector,
        Expression<Func<TViewModel, TValue>> viewModelPropertySelector,
        ViewModelUpdateMode viewModelUpdateMode = ViewModelUpdateMode.OnValidation,
        ControlUpdateMode controlUpdateMode = ControlUpdateMode.OnPropertyChanged
    )
        where TViewModel : class
    {
        builder.SetBinding<TViewModel, TextBoxControl, TextBoxCommand, TValue>(
            handle.Id,
            controlPropertySelector,
            viewModelPropertySelector,
            viewModelUpdateMode,
            controlUpdateMode
        );
    }

    public static void SetProperty<TViewModel, TValue>(
        this IDialogProcedureBuilder<TViewModel> builder,
        DialogItemHandle<TextBox> handle,
        Expression<Func<TextBoxControl, TValue>> controlPropertySelector,
        TValue value
    )
        where TViewModel : class
    {
        builder.SetProperty(
            handle.Id,
            controlPropertySelector,
            value
        );
    }
}
