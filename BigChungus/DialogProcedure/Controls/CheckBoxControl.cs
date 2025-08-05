using System.Linq.Expressions;

public readonly struct CheckBoxControl : IControl<CheckBoxControl, ButtonCommand>
{
    private readonly nint _handle;
    public CheckBoxControl(nint handle) => _handle = handle;
    static CheckBoxControl IControl<CheckBoxControl>.Create(nint handle) => new(handle);
    nint IControl<CheckBoxControl>.Handle => _handle;

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

    [PropertyChangedCommand<ButtonCommand>(ButtonCommand.Click)]
    public bool IsChecked
    {
        get => Win32.SendMessage(_handle, BM_GETCHECK, 0, 0) == BST_CHECKED;
        set => Win32.SendMessage(_handle, BM_SETCHECK, value ? BST_CHECKED : BST_UNCHECKED, 0);
    }
}

public static partial class DialogProcedureBuilderExtensions
{
    public static void SetCommand<TViewModel>(
        this IDialogProcedureBuilder<TViewModel> builder,
        DialogItemHandle<CheckBox> handle,
        Action<TViewModel> viewModelCommand,
        ButtonCommand controlCommand = default
    )
        where TViewModel : class
    {
        builder.SetCommand<TViewModel, CheckBoxControl, ButtonCommand>(
            handle.Id,
            viewModelCommand,
            controlCommand
        );
    }

    public static void SetBinding<TViewModel, TValue>(
        this IDialogProcedureBuilder<TViewModel> builder,
        DialogItemHandle<CheckBox> handle,
        Expression<Func<CheckBoxControl, TValue>> controlPropertySelector,
        Expression<Func<TViewModel, TValue>> viewModelPropertySelector,
        ViewModelUpdateMode viewModelUpdateMode = ViewModelUpdateMode.OnPropertyChanged,
        ControlUpdateMode controlUpdateMode = ControlUpdateMode.OnPropertyChanged
    )
        where TViewModel : class
    {
        builder.SetBinding<TViewModel, CheckBoxControl, ButtonCommand, TValue>(
            handle.Id,
            controlPropertySelector,
            viewModelPropertySelector,
            viewModelUpdateMode,
            controlUpdateMode
        );
    }

    public static void SetProperty<TViewModel, TValue>(
        this IDialogProcedureBuilder<TViewModel> builder,
        DialogItemHandle<CheckBox> handle,
        Expression<Func<CheckBoxControl, TValue>> controlPropertySelector,
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
