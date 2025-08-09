public enum NoCommand;

public abstract class DialogBinding<TViewModel, TControl> : IDialogBehavior<TViewModel>
    where TViewModel : class
    where TControl : struct, IControl<TControl>
{
    public required ushort? ItemId { private get; init; }

    protected TControl GetDialogItem(nint dialogBoxHandle)
    {
        var controlHandle = ItemId switch
        {
            ushort itemId => Win32.GetDlgItem(dialogBoxHandle, itemId),
            _ => dialogBoxHandle
        };
        return TControl.Create(controlHandle);
    }

    protected virtual void OnMessageReceived(Message message, IDialogContext<TViewModel> context) { }

    nint? IDialogBehavior<TViewModel>.OnMessageReceived(Message message, IDialogContext<TViewModel> context)
    {
        OnMessageReceived(message, context);
        return null;
    }
}

public delegate T ControlGetMethod<TOwner, T>(in TOwner owner) where TOwner : struct;
public delegate void ControlSetMethod<TOwner, T>(in TOwner owner, T value) where TOwner : struct;

public delegate T ViewModelGetMethod<TOwner, T>(TOwner owner) where TOwner : class;
public delegate void ViewModelSetMethod<TOwner, T>(TOwner owner, T value) where TOwner : class;
