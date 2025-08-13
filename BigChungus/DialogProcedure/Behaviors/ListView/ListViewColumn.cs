
// https://github.com/dotnet/runtime/issues/103176 !!!!!11
public abstract class ListViewColumn<TRow>
{
    public string Caption { get; }
    public int Width { get; }
    public abstract void GetColumnValue(TRow row, Span<char> targetSpan);

    protected ListViewColumn(string caption, int width)
    {
        Caption = caption;
        Width = width;
    }
}

public class ListViewStringColumn<TRow> : ListViewColumn<TRow>
    where TRow : class
{
    private readonly Func<TRow, string> _columnValueSelector;
    public ListViewStringColumn(string caption, Func<TRow, string> columnValueSelector, int width) : base(caption, width)
    {
        _columnValueSelector = columnValueSelector;
    }

    public override void GetColumnValue(TRow row, Span<char> targetSpan)
    {
        var span = _columnValueSelector(row).AsSpan();
        if (span.Length >= targetSpan.Length)
        {
            span = span.Slice(0, targetSpan.Length - 1);
        }
        span.CopyTo(targetSpan);
        targetSpan[span.Length] = '\0';
    }
}

public class ListViewValueColumn<TRow, TValue> : ListViewColumn<TRow>
    where TRow : class
    where TValue : ISpanFormattable
{
    private readonly Func<TRow, TValue> _columnValueSelector;
    private readonly string? _formatString = default;
    private readonly IFormatProvider? _formatProvider = default;
    public ListViewValueColumn(string caption, Func<TRow, TValue> columnValueSelector, int width, string? formatString = null, IFormatProvider? formatProvider = null) : base(caption, width)
    {
        _columnValueSelector = columnValueSelector;
        _formatString = formatString;
        _formatProvider = formatProvider;
    }

    public override void GetColumnValue(TRow row, Span<char> targetSpan)
    {
        var value = _columnValueSelector(row);
        if (value.TryFormat(targetSpan, out var charsWritten, _formatString, _formatProvider))
        {
            targetSpan[charsWritten] = '\0';
            return;
        }
        throw new InvalidOperationException(); // I don't anticipate ISpanFormattable implementors to require more than ~200 characters.
    }
}
