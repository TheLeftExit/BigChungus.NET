using System.Linq.Expressions;
using System.Reflection;

public sealed class ServiceInitializer<TViewModel> : IDialogBehavior<TViewModel>
    where TViewModel : class
{
    public required ViewModelSetMethod<TViewModel, DialogService> ViewModelSetMethod { get; init; }


    nint? IDialogBehavior<TViewModel>.OnMessageReceived(Message message, IDialogContext<TViewModel> context)
    {
        if (message.msg is WM_INITDIALOG)
        {
            var service = new DialogService(context.DialogBoxHandle);
            ViewModelSetMethod(context.ViewModel, service);
            return null;
        }
        if(message.msg is WM_DESTROY)
        {
            ViewModelSetMethod(context.ViewModel, default);
            return null;
        }
        return null;
    }
}

public readonly struct DialogService
{
    private readonly nint _handle;
    public DialogService(nint handle)
    {
        _handle = handle;
    }

    public bool IsReady => _handle != 0;

    private void GuardHandle()
    {
        if (_handle == 0) throw new InvalidOperationException();
    }

    public DialogResult ShowDialog<TViewModel>(IDialogRunner<TViewModel> view, TViewModel viewModel)
        where TViewModel : class
    {
        GuardHandle();
        return view.ShowDialog(viewModel, _handle);
    }

    public DialogResult ShowDialog<TViewModel>(TViewModel viewModel)
        where TViewModel : class, IDialogRunner<TViewModel>
    {
        GuardHandle();
        return viewModel.ShowDialog(viewModel, _handle);
    }

    public void Close(DialogResult result)
    {
        GuardHandle();
        Win32.EndDialog(_handle, (int)result);
    }

    /*
    public Task<T> InvokeAsync<T>(Func<T> callback)
    {
        GuardHandle();
        throw new NotImplementedException();
    }

    public Task InvokeAsync(Action callback)
    {
        GuardHandle();
        throw new NotImplementedException();
    }
    */
}

public enum DialogResult
{
    OK = IDOK,
    Cancel = IDCANCEL,
    Abort = IDABORT,
    Retry = IDRETRY,
    Ignore = IDIGNORE,
    Yes = IDYES,
    No = IDNO,
    Close = IDCLOSE,
    Help = IDHELP,
    TryAgain = IDTRYAGAIN,
    Continue = IDCONTINUE
}

public static partial class DialogProcedureBuilderExtensions
{
    public static void InjectDialogService<TViewModel>(
        this IDialogProcedureBuilder<TViewModel> builder,
        Expression<Func<TViewModel, DialogService>> viewModelPropertySelector
    )
        where TViewModel : class
    {
        if (viewModelPropertySelector is not { Body: MemberExpression { Member: PropertyInfo { SetMethod: MethodInfo controlPropertyGetMethod } } })
        {
            throw new NotSupportedException();
        }

        var behavior = new ServiceInitializer<TViewModel>
        {
            ViewModelSetMethod = controlPropertyGetMethod.CreateDelegate<ViewModelSetMethod<TViewModel, DialogService>>(),
        };
        builder.AddBehavior(behavior);
    }
}
