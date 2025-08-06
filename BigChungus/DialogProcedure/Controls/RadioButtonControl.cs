using System.Linq.Expressions;

public readonly struct RadioButtonControl : IControl<RadioButtonControl, ButtonCommand>
{
    private readonly nint _handle;
    public RadioButtonControl(nint handle) => _handle = handle;
    static RadioButtonControl IControl<RadioButtonControl>.Create(nint handle) => new(handle);
    nint IControl<RadioButtonControl>.Handle => _handle;

    public static bool IsCommandMessage(Message message, out ButtonCommand command)
    {
        return ButtonControl.IsCommandMessage(message, out command);
    }

    public bool IsCommandSender(Message message, ButtonCommand command)
    {
        return new ButtonControl(_handle).IsCommandSender(message, command);
    }

    public string? Text
    {
        get => new Control(_handle).Text;
        set => new Control(_handle).Text = value;
    }

    public bool IsChecked
    {
        // No get method - users should use IDialogProcedureBuilder.SetRadioGroupBinding
        set => Win32.SendMessage(_handle, BM_SETCHECK, value ? BST_CHECKED : BST_UNCHECKED, 0);
    }
}

public static partial class DialogProcedureBuilderExtensions
{
    public static void SetCommand<TViewModel>(
        this IDialogProcedureBuilder<TViewModel> builder,
        DialogItemHandle<RadioButton> handle,
        Action<TViewModel> viewModelCommand,
        ButtonCommand controlCommand = default
    )
        where TViewModel : class
    {
        builder.SetCommand<TViewModel, RadioButtonControl, ButtonCommand>(
            handle.Id,
            viewModelCommand,
            controlCommand
        );
    }

    public static void SetBinding<TViewModel, TValue>(
        this IDialogProcedureBuilder<TViewModel> builder,
        DialogItemHandle<RadioButton> handle,
        Expression<Func<RadioButtonControl, TValue>> controlPropertySelector,
        Expression<Func<TViewModel, TValue>> viewModelPropertySelector,
        ViewModelUpdateMode viewModelUpdateMode = ViewModelUpdateMode.OnPropertyChanged,
        ControlUpdateMode controlUpdateMode = ControlUpdateMode.OnPropertyChanged
    )
        where TViewModel : class
    {
        builder.SetBinding<TViewModel, RadioButtonControl, ButtonCommand, TValue>(
            handle.Id,
            controlPropertySelector,
            viewModelPropertySelector,
            viewModelUpdateMode,
            controlUpdateMode
        );
    }

    public static void SetProperty<TViewModel, TValue>(
        this IDialogProcedureBuilder<TViewModel> builder,
        DialogItemHandle<RadioButton> handle,
        Expression<Func<RadioButtonControl, TValue>> controlPropertySelector,
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
