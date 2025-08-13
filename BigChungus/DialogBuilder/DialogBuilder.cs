public class DialogBuilder<TViewModel> : IDialogProcedureBuilder<TViewModel>, IDialogTemplateBuilder
    where TViewModel : class
{
    private readonly DialogProcedureBuilder<TViewModel> _dialogProcedureBuilder = new();
    private readonly DialogTemplateBuilder _dialogTemplateBuilder = new();

    public DialogRunner<TViewModel> Build()
    {
        var template = _dialogTemplateBuilder.Build();
        var procedure = _dialogProcedureBuilder.Build();
        return new DialogRunner<TViewModel>(template, procedure);
    }

    public DialogProperties Properties => _dialogTemplateBuilder.Properties;
    public RadioGroupScope BeginRadioGroup() => _dialogTemplateBuilder.BeginRadioGroup();
    public void EndRadioGroup() => _dialogTemplateBuilder.EndRadioGroup();

    void IDialogProcedureBuilder<TViewModel>.AddBehavior(IDialogBehavior<TViewModel> behavior)
    {
        _dialogProcedureBuilder.AddBehavior(behavior);
    }

    DialogItemHandle<T> IDialogTemplateBuilder.AddItem<T>(RectangleDLU bounds, string? text, Action<T>? initialize)
    {
        return _dialogTemplateBuilder.AddItem(bounds, text, initialize);
    }
}

public class DialogRunner<TViewModel>
    where TViewModel : class
{
    private readonly DialogProcedure<TViewModel> _procedure;
    private readonly ReadOnlyMemory<byte> _template;

    public DialogRunner(ReadOnlyMemory<byte> template, DialogProcedure<TViewModel> dlgProc)
    {
        _template = template;
        _procedure = dlgProc;
    }

    public void Run(TViewModel viewModel)
    {
        DialogBoxHelper.DialogBox(_template.Span, _procedure.CreateDialogView(viewModel));
    }
}
