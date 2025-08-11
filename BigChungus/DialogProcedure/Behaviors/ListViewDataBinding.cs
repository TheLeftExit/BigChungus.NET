using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public class ListViewDataBindingState<TViewModel, TRow> : IDisposable
    where TViewModel : class
    where TRow : class
{
    private readonly IList<TRow> _rows;
    private readonly List<INotifyPropertyChanged> _trackedRows;

    private readonly nint _controlHandle;

    public ListViewDataBindingState(IList<TRow> rows, nint controlHandle)
    {
        _rows = rows;
        _trackedRows = new();
        _controlHandle = controlHandle;

        if (_rows is INotifyCollectionChanged observableCollection)
        {
            observableCollection.CollectionChanged += OnCollectionChanged;
        }

        OnCollectionChanged(this, EventArgs.Empty);
    }

    private void OnCollectionChanged(object? sender, EventArgs e)
    {
        Win32.SendMessage(_controlHandle, LVM_SETITEMCOUNT, (nuint)_rows.Count, (nint)LVSICF_NOSCROLL);
    }

    public TRow GetRow(int index)
    {
        var row = _rows[index];
        if(row is INotifyPropertyChanged trackedRow && !_trackedRows.Contains(trackedRow))
        {
            trackedRow.PropertyChanged += OnCollectionChanged;
        }
        return row;
    }

    public void Dispose()
    {
        if (_rows is INotifyCollectionChanged observableCollection)
        {
            observableCollection.CollectionChanged -= OnCollectionChanged;
        }
        foreach(var row in _trackedRows)
        {
            row.PropertyChanged -= OnCollectionChanged;
        }
    }
}

public record ListViewColumn<TViewModel, TRow>(string Caption, Func<TRow, string> ColumnValueSelector, int Width)
    where TViewModel : class
    where TRow : class;

public class ListViewDataBinding<TViewModel, TRow> : DialogBinding<TViewModel, ListViewControl>
    where TViewModel : class
    where TRow : class
{
    public required ViewModelGetMethod<TViewModel, IList<TRow>?> ViewModelGetMethod { get; init; }
    public required string ViewModelPropertyName { get; init; }
    public required ListViewColumn<TViewModel, TRow>[] Columns { get; init; }

    private const string ListPropertyName = "BigChungus.ListViewList";

    protected override void OnMessageReceived(Message message, IDialogContext<TViewModel> context)
    {
        if (message.msg is WM_INITDIALOG)
        {
            InitColumns(context);
            SetDataSource(context);
        }

        if (ProcessGetDisplayInfoNotification(message, context)) return;

        if (PropertyChangedEventArgs.Parse(message, out var e))
        {
            RemoveDataSource(context);
            SetDataSource(context);
        }

        if(message.msg is WM_DESTROY)
        {
            RemoveDataSource(context);
        }
    }

    private unsafe void InitColumns(IDialogContext<TViewModel> context)
    {
        GetDialogItem(context, out var handle);
        for (int i = 0; i < Columns.Length; i++)
        {
            var column = Columns[i];
            fixed(char* columnNamePtr = column.Caption)
            {
                var lvColumn = new Win32.LVCOLUMN
                {
                    mask = LVCF_TEXT | LVCF_SUBITEM | LVCF_WIDTH,
                    pszText = columnNamePtr,
                    iSubItem = i,
                    cx = column.Width,
                };
                Win32.SendMessage(handle, LVM_INSERTCOLUMN, (nuint)i, (nint)(&lvColumn));
            }
        }
    }

    private unsafe bool ProcessGetDisplayInfoNotification(Message message, IDialogContext<TViewModel> context)
    {
        if (!ListViewControl.IsCommandMessage(message, out var command)) return false;
        if (command != ListViewCommand.GetItemInfo) return false;
        if (!GetDialogItem(context).IsCommandSender(message, command)) return false;

        var item = &((Win32.NMLVDISPINFO*)message.lParam)->item;
        var row = GetProperties(context)
            .GetProperty<ListViewDataBindingState<TViewModel, TRow>>(ListPropertyName)!
            .GetRow(item->iItem);

        if(StyleHelper.GetFlag(item->mask, LVIF_COLUMNS))
        {
            item->cColumns = (uint)Columns.Length - 1;
        }
        if (StyleHelper.GetFlag(item->mask, LVIF_TEXT))
        {
            var text = Columns[item->iSubItem].ColumnValueSelector(row);
            var targetSpan = new Span<char>(item->pszText, item->cchTextMax);
            for(int i = 0; i < text.Length && i < targetSpan.Length; i++)
            {
                targetSpan[i] = text[i];
            }
            targetSpan[text.Length] = '\0'; // Null-terminate the string
        }
        return true;
    }

    private void SetDataSource(IDialogContext<TViewModel> context)
    {
        var collection = ViewModelGetMethod(context.ViewModel);
        GetDialogItem(context, out var handle);
        if (collection is null)
        {
            Win32.SendMessage(handle, LVM_SETITEMCOUNT, 0, 0);
            return;
        }
        var newState = new ListViewDataBindingState<TViewModel, TRow>(collection, handle);
        GetProperties(context).SetProperty(ListPropertyName, newState);
    }

    private void RemoveDataSource(IDialogContext<TViewModel> context)
    {
        var oldState = GetProperties(context).RemoveProperty<ListViewDataBindingState<TViewModel, TRow>>(ListPropertyName);
        oldState?.Dispose();
    }
}
