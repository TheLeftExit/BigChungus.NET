public interface IDialogView<TViewModel>
    where TViewModel : class
{
    void Run(TViewModel viewModel);
}

public class DialogViewCore<TViewModel> : IDialogView<TViewModel>
    where TViewModel : class
{
    private readonly DialogProcedure<TViewModel> _procedure;
    private readonly ReadOnlyMemory<byte> _template;

    public DialogViewCore(ReadOnlyMemory<byte> template, DialogProcedure<TViewModel> dlgProc)
    {
        _template = template;
        _procedure = dlgProc;
    }

    public void Run(TViewModel viewModel)
    {
        DialogBoxHelper.DialogBox(_template.Span, _procedure.CreateDlgProc(viewModel));
    }
}

public abstract class DialogViewBase<TViewModel> : IDialogView<TViewModel>
    where TViewModel : class
{
    private readonly IDialogView<TViewModel> _viewCore;
    public DialogViewBase()
    {
        var builder = new DialogBuilder<TViewModel>();
        Configure(builder);
        _viewCore = builder.Build();
    }
    public void Run(TViewModel viewModel) => _viewCore.Run(viewModel);
    protected abstract void Configure(DialogBuilder<TViewModel> builder);
}

public sealed class DialogView<TViewModel>(Action<DialogBuilder<TViewModel>> configure) : DialogViewBase<TViewModel>()
    where TViewModel : class
{
    protected override void Configure(DialogBuilder<TViewModel> builder) => configure(builder);
}
