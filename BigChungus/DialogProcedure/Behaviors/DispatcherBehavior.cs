using System.Runtime.InteropServices;

public interface IDispatcherBehavior
{
    void Post(SendOrPostCallback d, object? state, nint dialogHandle);
}

public class DispatcherBehavior<TViewModel> : IDialogBehavior<TViewModel>, IDispatcherBehavior
    where TViewModel : class
{
    nint? IDialogBehavior<TViewModel>.OnMessageReceived(Message message, IDialogContext<TViewModel> context)
    {
        if(message.msg is WM_INITDIALOG)
        {
            var dialogHandle = context.DialogBoxHandle;
            SynchronizationContext.SetSynchronizationContext(new DialogSynchronizationContext(dialogHandle, this));
            return null;
        }
        if(message.msg is WM_DESTROY)
        {
            var previousContext = ((DialogSynchronizationContext?)SynchronizationContext.Current)?.PreviousContext;
            SynchronizationContext.SetSynchronizationContext(previousContext);
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
        }

        return null;
    }

    void IDispatcherBehavior.Post(SendOrPostCallback d, object? state, nint dialogHandle)
    {
        var callbackHandle = GCHandle.Alloc(d, GCHandleType.Normal);
        var stateHandle = GCHandle.Alloc(state, GCHandleType.Normal);
        Win32.PostMessage(dialogHandle, WM_INVOKE, (nuint)GCHandle.ToIntPtr(callbackHandle), GCHandle.ToIntPtr(stateHandle));
    }
}

public class DialogSynchronizationContext : SynchronizationContext
{
    private readonly nint _dialogHandle;
    private readonly IDispatcherBehavior _behavior;

    public SynchronizationContext? PreviousContext { get; }

    public DialogSynchronizationContext(nint dialogHandle, IDispatcherBehavior behavior)
    {
        _dialogHandle = dialogHandle;
        _behavior = behavior;
        PreviousContext = Current;
    }

    public override void Send(SendOrPostCallback d, object? state) => throw new NotImplementedException();
    public override SynchronizationContext CreateCopy() => throw new NotImplementedException();
    public override void Post(SendOrPostCallback d, object? state) => _behavior.Post(d, state, _dialogHandle);
}
