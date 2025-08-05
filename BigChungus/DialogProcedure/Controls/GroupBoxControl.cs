using System.Linq.Expressions;

public readonly struct GroupBoxControl : IControl<GroupBoxControl, NoCommand>
{
    private readonly nint _handle;
    public GroupBoxControl(nint handle) => _handle = handle;
    static GroupBoxControl IControl<GroupBoxControl>.Create(nint handle) => new(handle);
    nint IControl<GroupBoxControl>.Handle => _handle;

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
        DialogItemHandle<GroupBox> handle,
        Expression<Func<GroupBoxControl, TValue>> controlPropertySelector,
        Expression<Func<TViewModel, TValue>> viewModelPropertySelector
    )
        where TViewModel : class
    {
        builder.SetBinding<TViewModel, GroupBoxControl, NoCommand, TValue>(
            handle.Id,
            controlPropertySelector,
            viewModelPropertySelector
        );
    }
}
