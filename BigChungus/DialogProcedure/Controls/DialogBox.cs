using System.Linq.Expressions;

public enum DialogBoxCommand
{

}

public readonly struct DialogBox : IControl<DialogBox, NoCommand>
{
    private readonly nint _handle;
    public DialogBox(nint handle) => this._handle = handle;
    static DialogBox IControl<DialogBox>.Create(nint handle) => new(handle);
    nint IControl<DialogBox>.Handle => this._handle;

    public static bool IsCommandMessage(Message message, out NoCommand command)
    {
        command = default;
        return false;
    }

    public bool IsCommandSender(Message message, NoCommand command)
    {
        return false;
    }

    public string? Text
    {
        get => new Control(_handle).Text;
        set => new Control(_handle).Text = value;
    }
}

public static partial class DialogProcedureBuilderExtensions
{
    public static void SetCommand<TViewModel>(
        this IDialogProcedureBuilder<TViewModel> builder,
        Action<TViewModel> viewModelCommand,
        NoCommand controlCommand = default
    )
    where TViewModel : class
    {
        builder.SetCommand<TViewModel, DialogBox, NoCommand>(
            null,
            viewModelCommand,
            controlCommand
        );
    }

    public static void SetBinding<TViewModel, TValue>(
        this IDialogProcedureBuilder<TViewModel> builder,
        Expression<Func<DialogBox, TValue>> controlPropertySelector,
        Expression<Func<TViewModel, TValue>> viewModelPropertySelector,
        ViewModelUpdateMode viewModelUpdateMode = ViewModelUpdateMode.OnLoseFocus,
        ControlUpdateMode controlUpdateMode = ControlUpdateMode.OnPropertyChanged
    )
        where TViewModel : class
    {
        builder.SetBinding<TViewModel, DialogBox, NoCommand, TValue>(
            null,
            controlPropertySelector,
            viewModelPropertySelector,
            viewModelUpdateMode,
            controlUpdateMode
        );
    }

    public static void SetProperty<TViewModel, TValue>(
        this IDialogProcedureBuilder<TViewModel> builder,
        Expression<Func<DialogBox, TValue>> controlPropertySelector,
        TValue value
    )
        where TViewModel : class
    {
        builder.SetProperty(
            null,
            controlPropertySelector,
            value
        );
    }
}
