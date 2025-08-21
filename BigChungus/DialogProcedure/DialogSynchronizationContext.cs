using System.Runtime.InteropServices;

// This could have been (and used to be) a regular behavior,
// but I figured that correct operation of await/async is important enough to warrant dedicated logic.

public class DispatcherHelper
{
    [ThreadStatic]
    private static int _dialogCount;
    [ThreadStatic]
    private static SynchronizationContext? _previousContext;

    public static void ProcessWmInitDialog(Message message) {
        if(_dialogCount == 0)
        {
            _previousContext = SynchronizationContext.Current;
            var context = new DialogSynchronizationContext(message.hWnd);
            SynchronizationContext.SetSynchronizationContext(context);
        }
        _dialogCount++;
    }

    public static void ProcessWmDestroy(Message message) {
        _dialogCount--;

        if(_dialogCount == 0 && SynchronizationContext.Current is DialogSynchronizationContext context)
        {
            SynchronizationContext.SetSynchronizationContext(_previousContext);
            _previousContext = null;
        }
    }

    public static bool TryProcessWmInvoke(Message message)
    {
        if (message.msg is not WM_INVOKE) return false;

        var callbackHandle = GCHandle.FromIntPtr((nint)message.wParam);
        var stateHandle = GCHandle.FromIntPtr(message.lParam);

        var callback = (SendOrPostCallback)callbackHandle.Target!;
        var state = stateHandle.Target;
        callback(state);

        callbackHandle.Free();
        stateHandle.Free();

        return true;
    }
}

public class DialogSynchronizationContext : SynchronizationContext
{
    public nint DialogHandle { get; }
    public SynchronizationContext? PreviousContext { get; }

    public DialogSynchronizationContext(nint dialogHandle)
    {
        DialogHandle = dialogHandle;
        PreviousContext = Current;
    }

    public override void Post(SendOrPostCallback d, object? state)
    {
        var callbackHandle = GCHandle.Alloc(d, GCHandleType.Normal);
        var stateHandle = GCHandle.Alloc(state, GCHandleType.Normal);
        Win32.PostMessage(DialogHandle, WM_INVOKE, (nuint)GCHandle.ToIntPtr(callbackHandle), GCHandle.ToIntPtr(stateHandle));
    }
    public override void Send(SendOrPostCallback d, object? state) => throw new NotImplementedException();
    public override SynchronizationContext CreateCopy() => throw new NotImplementedException();
}
