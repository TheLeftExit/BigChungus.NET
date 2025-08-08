using System.Linq.Expressions;

public readonly struct LabelControl : IControl<LabelControl, NoCommand>
{
    private readonly nint _handle;
    public LabelControl(nint hWnd) => _handle = hWnd;
    static LabelControl IControl<LabelControl>.Create(nint handle) => new(handle);
    nint IControl<LabelControl>.Handle => _handle;

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
    public static void SetBinding<TViewModel, TValue>(
        this IDialogProcedureBuilder<TViewModel> builder,
        DialogItemHandle<Label> handle,
        Expression<Func<LabelControl, TValue>> controlPropertySelector,
        Expression<Func<TViewModel, TValue>> viewModelPropertySelector
    )
        where TViewModel : class
    {
        builder.SetBinding<TViewModel, LabelControl, NoCommand, TValue>(
            handle.Id,
            controlPropertySelector,
            viewModelPropertySelector
        );
    }
}
