using System.Linq.Expressions;

public readonly struct ProgressBarControl : IControl<ProgressBarControl, NoCommand>
{
    private readonly nint _handle;
    public ProgressBarControl(nint handle) => _handle = handle;
    static ProgressBarControl IControl<ProgressBarControl>.Create(nint handle) => new(handle);
    nint IControl<ProgressBarControl>.Handle => _handle;

    public static bool IsCommandMessage(Message message, out NoCommand command)
    {
        command = default;
        return false;
    }

    public bool IsCommandSender(Message message, NoCommand command)
    {
        return false;
    }

    private const ushort MAX_VALUE = ushort.MaxValue;
    private void EnsureMaxValue()
    {
        var highLimit = Win32.SendMessage(_handle, PBM_GETRANGE, 0 /* FALSE */, 0 /* NULL */);
        if(highLimit != MAX_VALUE)
        {
            Win32.SendMessage(_handle, PBM_SETRANGE, 0, new DWord(0, MAX_VALUE));
        }
    }

    public double Value
    {
        get
        {
            var valueInternal = Win32.SendMessage(_handle, PBM_GETPOS, 0, 0);
            return (double)valueInternal / MAX_VALUE;
        }
        set
        {
            EnsureMaxValue();
            var valueInternal = (uint)(Math.Clamp(value, 0, 1) * MAX_VALUE);
            Win32.SendMessage(_handle, PBM_SETPOS, valueInternal, 0);
        }
    }

    public ProgressBarState State
    {
        get => (ProgressBarState)Win32.SendMessage(_handle, PBM_GETSTATE, 0, 0);
        set => Win32.SendMessage(_handle, PBM_SETSTATE, (nuint)value, 0);
    }
}

public enum ProgressBarState : uint
{
    Normal = PBST_NORMAL, // Green, animated
    Error = PBST_ERROR, // Red, static
    Paused = PBST_PAUSED, // Yellow-brown, static
}

public static partial class DialogProcedureBuilderExtensions
{
    public static void SetBinding<TViewModel, TValue>(
        this IDialogProcedureBuilder<TViewModel> builder,
        DialogItemHandle<ProgressBar> handle,
        Expression<Func<ProgressBarControl, TValue>> controlPropertySelector,
        Expression<Func<TViewModel, TValue>> viewModelPropertySelector
    )
        where TViewModel : class
    {
        builder.SetBinding<TViewModel, ProgressBarControl, NoCommand, TValue>(
            handle.Id,
            controlPropertySelector,
            viewModelPropertySelector
        );
    }
}
