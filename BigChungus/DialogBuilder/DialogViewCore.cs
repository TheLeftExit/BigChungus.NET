public interface IDialogRunner<TViewModel>
    where TViewModel : class
{
    DialogResult ShowDialog(TViewModel viewModel, nint parentHandle = 0);
}

public sealed class DialogViewCore<TViewModel> : IDialogRunner<TViewModel>
    where TViewModel : class
{
    private readonly DialogProcedure<TViewModel> _procedure;
    private readonly ReadOnlyMemory<byte> _template;

    public DialogViewCore(ReadOnlyMemory<byte> template, DialogProcedure<TViewModel> dlgProc)
    {
        _template = template;
        _procedure = dlgProc;
    }

    public DialogResult ShowDialog(TViewModel viewModel, nint parentHandle = 0)
    {
        return (DialogResult)DialogBoxHelper.DialogBox(_template.Span, _procedure.CreateDlgProc(viewModel), parentHandle);
    }
}

public abstract class DialogViewBase<TViewModel> : IDialogRunner<TViewModel>
    where TViewModel : class
{
    private readonly IDialogRunner<TViewModel> _viewCore;
    public DialogViewBase()
    {
        var builder = new DialogBuilder<TViewModel>();
        Configure(builder);
        _viewCore = builder.Build();
    }
    public DialogResult ShowDialog(TViewModel viewModel, nint parentHandle = 0) => _viewCore.ShowDialog(viewModel, parentHandle);
    protected abstract void Configure(DialogBuilder<TViewModel> builder);
}

public sealed class DialogView<TViewModel>(Action<DialogBuilder<TViewModel>> configure) : DialogViewBase<TViewModel>()
    where TViewModel : class
{
    protected override void Configure(DialogBuilder<TViewModel> builder) => configure(builder);
}
