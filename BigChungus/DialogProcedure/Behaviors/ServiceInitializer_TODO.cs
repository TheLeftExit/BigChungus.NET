using System.Linq.Expressions;
using System.Reflection;

// TODO: reconsider this whole design
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

    nint? IDialogBehavior<TViewModel>.OnMessageReceived(Message message, IDialogContext<TViewModel> context)
    {
        if (message.msg is WM_INITDIALOG)
        {
            var service = TService.Create(context.DialogBoxHandle);
            ViewModelSetMethod(context.ViewModel, service);
        }
        return null;
    }
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
