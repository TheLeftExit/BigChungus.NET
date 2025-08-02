public class DialogBuilder<TViewModel> : IDialogProcedureBuilder<TViewModel>, IDialogTemplateBuilder
    where TViewModel : class
{
    private readonly DialogProcedureBuilder<TViewModel> _dialogProcedureBuilder = new();
    private readonly DialogTemplateBuilder _dialogTemplateBuilder = new();

    public DialogItemHandle<T> AddItem<T>(RectangleDLU bounds, string? text = null, DialogItemInitialize<T>? initialize = null)
        where T : IDialogItemProperties, new()
    {
        return _dialogTemplateBuilder.AddItem(bounds, text, initialize);
    }

    public DialogItemHandle<T> AddItem<T>(RectangleDLU bounds, DialogItemInitialize<T>? initialize)
        where T : IDialogItemProperties, new()
    {
        return _dialogTemplateBuilder.AddItem(bounds, null, initialize);
    }

    public DialogRunner<TViewModel> Build(TViewModel viewModel)
    {
        var dlgProc = _dialogProcedureBuilder.Build(viewModel);
        var template = _dialogTemplateBuilder.Build();
        return new DialogRunner<TViewModel>(dlgProc, template);
    }

    void IDialogProcedureBuilder<TViewModel>.AddBehavior(IDialogBehavior<TViewModel> behavior)
    {
        _dialogProcedureBuilder.AddBehavior(behavior);
    }

    DialogItemHandle<T> IDialogTemplateBuilder.AddItem<T>(RectangleDLU bounds, string? text, DialogItemInitialize<T>? initialize)
    {
        return _dialogTemplateBuilder.AddItem(bounds, text, initialize);
    }
}

public class DialogRunner<TViewModel>
    where TViewModel : class
{
    private readonly IDlgProc _dlgProc;
    private readonly ReadOnlyMemory<byte> _template;

    public DialogRunner(IDlgProc dlgProc, ReadOnlyMemory<byte> template)
    {
        _dlgProc = dlgProc;
        _template = template;
    }

    public void Run()
    {
        DialogBoxHelper.DialogBox(_template.Span, _dlgProc);
    }
}
