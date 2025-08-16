public class DialogBuilder<TViewModel> : IDialogProcedureBuilder<TViewModel>, IDialogTemplateBuilder
    where TViewModel : class
{
    private readonly DialogProcedureBuilder<TViewModel> _dialogProcedureBuilder = new();
    private readonly DialogTemplateBuilder _dialogTemplateBuilder = new();

    public IDialogView<TViewModel> Build()
    {
        var template = _dialogTemplateBuilder.Build();
        var procedure = _dialogProcedureBuilder.Build();
        return new DialogViewCore<TViewModel>(template, procedure);
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
