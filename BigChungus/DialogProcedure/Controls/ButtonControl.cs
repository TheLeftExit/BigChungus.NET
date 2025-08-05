using System.Linq.Expressions;

public enum ButtonCommand
{
    Click = default
}

public readonly struct ButtonControl : IControl<ButtonControl, ButtonCommand>
{
    private readonly nint _handle;
    public ButtonControl(nint hWnd) => _handle = hWnd;
    static ButtonControl IControl<ButtonControl>.Create(nint handle) => new(handle);
    nint IControl<ButtonControl>.Handle => _handle;

    public static bool IsCommandMessage(Message message, out ButtonCommand command)
    {
        if (message.msg == WM_COMMAND && new DWord(message.wParam).High == BN_CLICKED)
        {
            command = ButtonCommand.Click;
            return true;
        }
        command = default;
        return false;
    }

    public bool IsCommandSender(Message message, ButtonCommand command)
    {
        return command switch
        {
            ButtonCommand.Click => message.lParam == _handle,
            _ => false
        };
    }

    public string? Text
    {
        get => new Control(_handle).Text;
        set => new Control(_handle).Text = value;
    }

    public bool ShowShield
    {
        get => throw new NotSupportedException();
        set => Win32.SendMessage(_handle, BCM_SETSHIELD, 0, value ? 1 : 0);
    }
}

public static partial class DialogProcedureBuilderExtensions
{
    public static void SetCommand<TViewModel>(
        this IDialogProcedureBuilder<TViewModel> builder,
        DialogItemHandle<Button> handle,
        Action<TViewModel> viewModelCommand,
        ButtonCommand controlCommand = default
    )
        where TViewModel : class
    {
        builder.SetCommand<TViewModel, ButtonControl, ButtonCommand>(
            handle.Id,
            viewModelCommand,
            controlCommand
        );
    }

    public static void SetBinding<TViewModel, TValue>(
        this IDialogProcedureBuilder<TViewModel> builder,
        DialogItemHandle<Button> handle,
        Expression<Func<ButtonControl, TValue>> controlPropertySelector,
        Expression<Func<TViewModel, TValue>> viewModelPropertySelector
    )
        where TViewModel : class
    {
        builder.SetBinding<TViewModel, ButtonControl, ButtonCommand, TValue>(
            handle.Id,
            controlPropertySelector,
            viewModelPropertySelector
        );
    }

    public static void SetProperty<TViewModel, TValue>(
        this IDialogProcedureBuilder<TViewModel> builder,
        DialogItemHandle<Button> handle,
        Expression<Func<ButtonControl, TValue>> controlPropertySelector,
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
