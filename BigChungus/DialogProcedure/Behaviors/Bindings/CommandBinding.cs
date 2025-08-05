public sealed class CommandBinding<TViewModel, TControl, TCommand> : DialogBinding<TViewModel, TControl>
    where TViewModel : class
    where TControl : struct, IControl<TControl, TCommand>
    where TCommand : struct, Enum
{
    public required TCommand ControlCommand { get; init; }
    public required Action<TViewModel> ViewModelCommand { get; init; }

    protected override void OnMessageReceived(Message message, nint dialogBoxHandle, TViewModel viewModel)
    {
        if (!TControl.IsCommandMessage(message, out var command)) return;
        var control = GetDialogItem(dialogBoxHandle);
        if (!control.IsCommandSender(message, command)) return;
        if (!EqualityComparer<TCommand>.Default.Equals(command, ControlCommand)) return;

        ViewModelCommand(viewModel);
    }
}

public static partial class DialogProcedureBuilderExtensions
{
    public static void SetCommand<TViewModel, TControl, TCommand>(
        this IDialogProcedureBuilder<TViewModel> builder,
        ushort? itemId,
        Action<TViewModel> viewModelCommand,
        TCommand controlCommand = default
    )
        where TViewModel : class
        where TControl : struct, IControl<TControl, TCommand>
        where TCommand : struct, Enum
    {
        var behavior = new CommandBinding<TViewModel, TControl, TCommand>
        {
            ItemId = itemId,
            ControlCommand = controlCommand,
            ViewModelCommand = viewModelCommand
        };
        builder.AddBehavior(behavior);
    }
}
