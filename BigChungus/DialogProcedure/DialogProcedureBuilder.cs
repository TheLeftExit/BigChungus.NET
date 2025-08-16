public interface IDialogProcedureBuilder<TViewModel>
    where TViewModel : class
{
    void AddBehavior(IDialogBehavior<TViewModel> behavior);
}

public static partial class DialogProcedureBuilderExtensions;

public interface IDialogBehavior<TViewModel>
    where TViewModel : class
{
    nint? OnMessageReceived(Message message, IDialogContext<TViewModel> context);
}

public sealed class DialogProcedureBuilder<TViewModel> : IDialogProcedureBuilder<TViewModel>
    where TViewModel : class
{
    private readonly List<IDialogBehavior<TViewModel>> _behaviors = new();

    public void AddBehavior(IDialogBehavior<TViewModel> behavior)
    {
        _behaviors.Add(behavior);
    }

    // It's architecturally beautiful to convert a list of behaviors and a view model into a final IDlgProc in one method,
    // but it makes for a crappy API where you need to specify the view model as soon as you build the dialog. Hence this.
    public DialogProcedure<TViewModel> Build()
    {
        return new(_behaviors.ToArray());
    }
}

public sealed class DialogProcedure<TViewModel>
    where TViewModel : class
{
    private readonly IDialogBehavior<TViewModel>[] _behaviors;
    public DialogProcedure(IDialogBehavior<TViewModel>[] behaviors)
    {
        _behaviors = behaviors;
    }
    public IDlgProc CreateDlgProc(TViewModel viewModel)
    {
        return new DlgProc<TViewModel>(viewModel, _behaviors);
    }
}
