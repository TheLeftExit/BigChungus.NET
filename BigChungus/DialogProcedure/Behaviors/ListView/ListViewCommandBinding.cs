using System.Linq.Expressions;
using System.Reflection;

public sealed class ListViewCommandBinding<TViewModel> : DialogBinding<TViewModel, ListViewControl>
    where TViewModel : class
{
    public required Func<TViewModel, Action<int>> ViewModelCommandSelector { get; init; }

    protected override unsafe void OnMessageReceived(Message message, IDialogContext<TViewModel> context)
    {
        if (!ListViewControl.IsCommandMessage(message, out var command)) return;
        if(command is not ListViewCommand.ItemClick) return;
        var control = GetDialogItem(context);
        if (!control.IsCommandSender(message, ListViewCommand.ItemClick)) return;

        var info = (Win32.NMITEMACTIVATE*)message.lParam;
        ViewModelCommandSelector(context.ViewModel)(info->iItem);
    }
}

public static partial class DialogProcedureBuilderExtensions
{
    public static void SetListViewCommand<TViewModel>(
        this IDialogProcedureBuilder<TViewModel> builder,
        DialogItemHandle<ListView> handle,
        Func<TViewModel, Action<int>> viewModelCommandSelector
    )
        where TViewModel : class
    {
        var behavior = new ListViewCommandBinding<TViewModel>
        {
            ItemId = handle.Id,
            ViewModelCommandSelector = viewModelCommandSelector
        };
        builder.AddBehavior(behavior);
    }
}
