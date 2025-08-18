using System.Runtime.InteropServices;

public class DispatcherBehavior<TViewModel> : IDialogBehavior<TViewModel>
    where TViewModel : class
{
    nint? IDialogBehavior<TViewModel>.OnMessageReceived(Message message, IDialogContext<TViewModel> context)
    {
        if(message.msg is WM_INITDIALOG)
        {
            var dialogHandle = context.DialogBoxHandle;
            SynchronizationContext.SetSynchronizationContext(new DialogSynchronizationContext(dialogHandle));
            return null;
        }
        if(message.msg is WM_DESTROY)
        {
            var currentContext = SynchronizationContext.Current as DialogSynchronizationContext;
            if(currentContext is not null)
            {
                SynchronizationContext.SetSynchronizationContext(currentContext.PreviousContext);
            }
            return null;
        }
        if(message.msg is WM_INVOKE)
        {
            var callbackHandle = GCHandle.FromIntPtr((nint)message.wParam);
            var stateHandle = GCHandle.FromIntPtr(message.lParam);
            
            var callback = (SendOrPostCallback)callbackHandle.Target!;
            var state = stateHandle.Target;
            callback(state);

            callbackHandle.Free();
            stateHandle.Free();

            return 0;
        }

        return null;
    }
}

public class DialogSynchronizationContext : SynchronizationContext
{
    private readonly nint _dialogHandle;

    public SynchronizationContext? PreviousContext { get; }

    public DialogSynchronizationContext(nint dialogHandle)
    {
        _dialogHandle = dialogHandle;
        PreviousContext = Current;
    }

    public override void Post(SendOrPostCallback d, object? state)
    {
        var callbackHandle = GCHandle.Alloc(d, GCHandleType.Normal);
        var stateHandle = GCHandle.Alloc(state, GCHandleType.Normal);
        Win32.PostMessage(_dialogHandle, WM_INVOKE, (nuint)GCHandle.ToIntPtr(callbackHandle), GCHandle.ToIntPtr(stateHandle));
    }
    public override void Send(SendOrPostCallback d, object? state) => throw new NotImplementedException();
    public override SynchronizationContext CreateCopy() => throw new NotImplementedException();
}

public static partial class DialogProcedureBuilderExtensions
{
    public static void UseDispatcher<TViewModel>(
        this IDialogProcedureBuilder<TViewModel> builder
    )
        where TViewModel : class
    {
        var behavior = new DispatcherBehavior<TViewModel>();
        builder.AddBehavior(behavior);
    }
}
