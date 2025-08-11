using System.Linq.Expressions;
using System.Reflection;

public enum ListViewCommand
{
    ItemClick = default,
    GetItemInfo
}

public readonly struct ListViewControl : IControl<ListViewControl, ListViewCommand>
{
    private readonly nint _handle;
    public ListViewControl(nint hWnd) => _handle = hWnd;
    static ListViewControl IControl<ListViewControl>.Create(nint handle) => new(handle);
    nint IControl<ListViewControl>.Handle => _handle;

    public static unsafe bool IsCommandMessage(Message message, out ListViewCommand command)
    {
        if (message.msg is WM_NOTIFY)
        {
            var nmhdr = (Win32.NMHDR*)message.lParam;
            if (nmhdr->code is LVN_GETDISPINFO)
            {
                command = ListViewCommand.GetItemInfo;
                return true;
            }
            if(nmhdr->code is LVN_ITEMACTIVATE)
            {
                command = ListViewCommand.ItemClick;
                return true;
            }
        }
        command = default;
        return false;
    }

    public unsafe bool IsCommandSender(Message message, ListViewCommand command)
    {
        return command switch
        {
            ListViewCommand.ItemClick or ListViewCommand.GetItemInfo => ((Win32.NMLVDISPINFO*)message.lParam)->hdr.hwndFrom == _handle,
            _ => false
        };
    }
}

public static partial class DialogProcedureBuilderExtensions
{
    public static void SetListViewBinding<TViewModel, TRow>(
        this IDialogProcedureBuilder<TViewModel> builder,
        DialogItemHandle<ListView> handle,
        Expression<Func<TViewModel, IList<TRow>>> viewModelCollectionPropertySelector,
        params ListViewColumn<TViewModel, TRow>[] columns
    )
        where TViewModel : class
        where TRow : class
    {
        var viewModelProperty = (viewModelCollectionPropertySelector.Body as MemberExpression)?.Member as PropertyInfo;
        if (viewModelProperty is null) throw new NotSupportedException();

        if (columns.Length is 0 or > 21) throw new ArgumentException(nameof(columns));

        var behavior = new ListViewDataBinding<TViewModel, TRow>
        {
            ItemId = handle.Id,
            ViewModelGetMethod = viewModelProperty.GetMethod!.CreateDelegate<ViewModelGetMethod<TViewModel, IList<TRow>?>>(),
            ViewModelPropertyName = viewModelProperty.Name,
            Columns = columns
        };
        builder.AddBehavior(behavior);
    }
}
