using System.Linq.Expressions;
using System.Reflection;

public interface IDialogService<TSelf>
    where TSelf : IDialogService<TSelf>
{
    abstract static TSelf Create(nint dialogBoxHandle);
}

public class ServiceInitializer<TViewModel, TService> : IDialogBehavior<TViewModel>
    where TViewModel : class
    where TService : IDialogService<TService>
{
    public required ViewModelSetMethod<TViewModel, TService> ViewModelSetMethod { get; init; }

    nint? IDialogBehavior<TViewModel>.OnMessageReceived(Message message, nint dialogBoxHandle, TViewModel viewModel)
    {
        if(message.msg is WM_INITDIALOG)
        {
            var service = TService.Create(dialogBoxHandle);
            ViewModelSetMethod(viewModel, service);
        }
        return null;
    }

    void IDialogBehavior<TViewModel>.OnPropertyChanged(string? propertyName, nint dialogBoxHandle, TViewModel viewModel) { }
}

public static partial class DialogProcedureBuilderExtensions
{
    public static void RegisterService<TViewModel, TService>(
        this IDialogProcedureBuilder<TViewModel> builder,
        Expression<Func<TViewModel, TService>> viewModelPropertySelector
    )
        where TViewModel : class
        where TService : IDialogService<TService>
    {
        var viewModelProperty = (viewModelPropertySelector.Body as MemberExpression)?.Member as PropertyInfo;
        if (viewModelProperty?.SetMethod is not MethodInfo setMethod) throw new NotSupportedException();

        var behavior = new ServiceInitializer<TViewModel, TService>
        {
            ViewModelSetMethod = setMethod.CreateDelegate<ViewModelSetMethod<TViewModel, TService>>()
        };
        builder.AddBehavior(behavior);
    }
}
