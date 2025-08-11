using System.Runtime.InteropServices;

public enum NoCommand;

public abstract class DialogBinding<TViewModel, TControl> : IDialogBehavior<TViewModel>
    where TViewModel : class
    where TControl : struct, IControl<TControl>
{
    public required ushort? ItemId { private get; init; }

    protected TControl GetDialogItem(IDialogContext<TViewModel> context)
    {
        return GetDialogItem(context, out _);
    }
    protected TControl GetDialogItem(IDialogContext<TViewModel> context, out nint handle)
    {
        handle = ItemId switch
        {
            ushort itemId => Win32.GetDlgItem(context.DialogBoxHandle, itemId),
            _ => context.DialogBoxHandle
        };
        return TControl.Create(handle);
    }

    protected ControlProperties GetProperties(IDialogContext<TViewModel> context)
    {
        var control = GetDialogItem(context);
        return ControlProperties.FromControl(control);
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
