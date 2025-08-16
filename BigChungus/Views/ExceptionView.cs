public class ExceptionView : DialogViewBase<ExceptionViewModel>
{
    public static ExceptionView Instance { get; } = new();

    protected override void Configure(DialogBuilder<ExceptionViewModel> builder)
    {
        builder.Properties.Text = "Unhandled exception";
        builder.Properties.Size = new SizeDLU(200, 160);

        var label = builder.AddItem<Label>(new RectangleDLU(4, 4, 192, 20), "(exception message)");
        var textBox = builder.AddItem<TextBox>(new RectangleDLU(4, 24, 192, 132), "(stack trace)", x =>
        {
            x.Multiline = true;
            x.ReadOnly = true;
            x.AllowVScroll = true;
        });

        builder.SetBinding(label, x => x.Text, vm => vm.Message);
        builder.SetBinding(textBox, x => x.Text, vm => vm.StackTrace);
    }
}

public class ExceptionViewModel : ViewModelBase<ExceptionView, ExceptionViewModel>
{
    private readonly Exception _exception;
    public ExceptionViewModel(Exception exception)
    {
        _exception = exception;
    }

    public string Message => _exception.Message;
    public string StackTrace => _exception.StackTrace ?? "(no stack trace)";
}
