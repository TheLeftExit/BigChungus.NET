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

    public IDlgProc Build(TViewModel viewModel)
    {
        return new DialogView<TViewModel>(viewModel, _behaviors.ToArray());
    }
}
