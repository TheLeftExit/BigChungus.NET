using System.Linq.Expressions;

public class ListViewBindingState<TViewModel, TRow>
    where TViewModel : class
    where TRow : class
{
    private readonly IList<TRow> _rows;
    private readonly IDialogContext<TViewModel> _context;
    private readonly List<TRow> _trackedRows;

    public ListViewBindingState(IList<TRow> rows, IDialogContext<TViewModel> context)
    {
        _rows = rows;
        _context = context;
        _trackedRows = new();
    }
}

public record ListViewColumn<TViewModel, TRow>(string Caption, Func<TRow, string> ColumnValueSelector)
    where TViewModel : class
    where TRow : class;

public class ListViewBinding<TViewModel, TRow> : DialogBinding<TViewModel, Control>
    where TViewModel : class
    where TRow : class
{
    public required Expression<Func<TViewModel, IList<TRow>>> ViewModelPropertySelector { get; init; }
    public required Func<TViewModel, Action<int>>? ItemCommand { get; init; }
    public required ListViewColumn<TViewModel, TRow>[] Columns { get; init; }

    protected override void OnMessageReceived(Message message, IDialogContext<TViewModel> context)
    {
        if(message.msg is WM_INITDIALOG)
        {

        }
    }

    private void BindToDataSource(IList<TRow> rows, IDialogContext<TViewModel> context)
    {
        throw new NotImplementedException();
    }
}
