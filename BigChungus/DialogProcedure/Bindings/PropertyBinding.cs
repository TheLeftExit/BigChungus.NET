using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

// Each update mode also reacts to messages matching all subsequent update modes, hence explicit values.
public enum ViewModelUpdateMode
{
    OnPropertyChanged = 0,
    OnLoseFocus = 1,
    OnDialogClose = 2,
    Never = 3
}

public enum ControlUpdateMode
{
    OnPropertyChanged,
    Never
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class PropertyChangedCommandAttribute<T> : Attribute
    where T : struct, Enum
{
    public T Command { get; }
    public PropertyChangedCommandAttribute(T command) => Command = command;
}

public sealed class PropertyBinding<TViewModel, TControl, TCommand, TValue> : DialogBinding<TViewModel, TControl>
    where TViewModel : class
    where TControl : struct, IControl<TControl, TCommand>
    where TCommand : struct, Enum
{
    public required ViewModelUpdateMode ViewModelUpdateMode { private get; init; }
    public required ControlGetMethod<TControl, TValue>? ControlGetMethod { private get; init; }
    public required ControlSetMethod<TControl, TValue>? ControlSetMethod { private get; init; }
    public required TCommand? ControlCommand { private get; init; }

    public required ControlUpdateMode ControlUpdateMode { private get; init; }
    public required ViewModelGetMethod<TViewModel, TValue>? ViewModelGetMethod { private get; init; }
    public required ViewModelSetMethod<TViewModel, TValue>? ViewModelSetMethod { private get; init; }
    public required string? ViewModelPropertyName { private get; init; }

    private bool isUpdating = false;

    protected override void OnMessageReceived(Message message, nint dialogBoxHandle, TViewModel viewModel)
    {
        if (isUpdating) return;
        using var updateLock = new UpdateLock(ref isUpdating);

        if(message.msg is WM_INITDIALOG)
        {
            var control = GetDialogItem(dialogBoxHandle);
            DialogBoxHelper.SubclassToReflectOwnKillFocusToParent(control.Handle); // Repeat SetWindowSubclass calls are allowed by comctl32.
            PushToControl(control, viewModel);
            return;
        }

        if (!CanPushToViewModel()) return;

        if (ProcessPropertyChangedMessage(message, dialogBoxHandle, viewModel)) return;
        if (ProcessLoseFocusMessage(message, dialogBoxHandle, viewModel)) return;
        if (ProcessDialogCloseMessage(message, dialogBoxHandle, viewModel)) return;
    }

    protected override void OnPropertyChanged(string? propertyName, nint dialogBoxHandle, TViewModel viewModel)
    {
        if (isUpdating) return;
        using var updateLock = new UpdateLock(ref isUpdating);

        if (!CanPushToControl()) return;
        
        var push = propertyName is null || propertyName == ViewModelPropertyName;
        if (!push) return;

        var control = GetDialogItem(dialogBoxHandle);
        PushToControl(control, viewModel);
    }

    [MemberNotNullWhen(true, nameof(ControlSetMethod), nameof(ViewModelGetMethod))]
    private bool CanPushToControl() =>
        ControlUpdateMode is not ControlUpdateMode.Never
        && ControlSetMethod is not null
        && ViewModelGetMethod is not null;

    private void PushToControl(TControl control, TViewModel viewModel)
    {
        if (!CanPushToControl()) return;
        var value = ViewModelGetMethod(viewModel);
        ControlSetMethod(control, value);
    }

    [MemberNotNullWhen(true, nameof(ControlGetMethod), nameof(ViewModelSetMethod))]
    private bool CanPushToViewModel() =>
        ViewModelUpdateMode is not ViewModelUpdateMode.Never
        && ControlGetMethod is not null
        && ViewModelSetMethod is not null;

    private void PushToViewModel(TControl control, TViewModel viewModel)
    {
        if (!CanPushToViewModel()) return;
        var value = ControlGetMethod(control);
        ViewModelSetMethod(viewModel, value);
    }

    private bool ProcessPropertyChangedMessage(Message message, nint dialogBoxHandle, TViewModel viewModel)
    {
        if (!TControl.IsCommandMessage(message, out var command)) return false;
        if (ViewModelUpdateMode > ViewModelUpdateMode.OnPropertyChanged) return true;

        if (!EqualityComparer<TCommand?>.Default.Equals(command, ControlCommand)) return true;

        var control = GetDialogItem(dialogBoxHandle);
        if (control.IsCommandSender(message, command))
        {
            PushToViewModel(control, viewModel);
        }
        return true;
    }

    private bool ProcessLoseFocusMessage(Message message, nint dialogBoxHandle, TViewModel viewModel)
    {
        if (message.msg is not WM_KILLFOCUS_REFLECT) return false;
        if (ViewModelUpdateMode > ViewModelUpdateMode.OnLoseFocus) return true;

        var control = GetDialogItem(dialogBoxHandle);
        if(message.lParam == control.Handle)
        {
            PushToViewModel(control, viewModel);
        }
        return true;
    }

    private bool ProcessDialogCloseMessage(Message message, nint dialogBoxHandle, TViewModel viewModel)
    {
        if (message.msg is not WM_DESTROY) return false;
        if (ViewModelUpdateMode > ViewModelUpdateMode.OnDialogClose) return true;

        var control = GetDialogItem(dialogBoxHandle);
        PushToViewModel(control, viewModel);
        return true;

    }

    private readonly ref struct UpdateLock
    {
        private readonly ref bool isLocked;
        public UpdateLock(ref bool isLocked)
        {
            this.isLocked = ref isLocked;
            isLocked = true;
        }
        public void Dispose()
        {
            isLocked = false;
        }
    }
}

public static partial class DialogProcedureBuilderExtensions
{
    public static void SetBinding<TViewModel, TControl, TCommand, TValue>(
        this IDialogProcedureBuilder<TViewModel> builder,
        ushort? itemId,
        Expression<Func<TControl, TValue>> controlPropertySelector,
        Expression<Func<TViewModel, TValue>> viewModelPropertySelector,
        ViewModelUpdateMode viewModelUpdateMode = ViewModelUpdateMode.OnLoseFocus,
        ControlUpdateMode controlUpdateMode = ControlUpdateMode.OnPropertyChanged
    )
        where TViewModel : class
        where TControl : struct, IControl<TControl, TCommand>
        where TCommand : struct, Enum
    {
        var controlProperty = (controlPropertySelector.Body as MemberExpression)?.Member as PropertyInfo;
        if (controlProperty is null) throw new NotSupportedException();

        var viewModelProperty = (viewModelPropertySelector.Body as MemberExpression)?.Member as PropertyInfo;
        if (viewModelProperty is null) throw new NotSupportedException();

        var behavior = new PropertyBinding<TViewModel, TControl, TCommand, TValue>
        {
            ItemId = itemId,
            ControlGetMethod = controlProperty.GetMethod?.CreateDelegate<ControlGetMethod<TControl, TValue>>(),
            ControlSetMethod = controlProperty.SetMethod?.CreateDelegate<ControlSetMethod<TControl, TValue>>(),
            ControlCommand = controlProperty.GetCustomAttribute<PropertyChangedCommandAttribute<TCommand>>()?.Command,
            ViewModelGetMethod = viewModelProperty.GetMethod?.CreateDelegate<ViewModelGetMethod<TViewModel, TValue>>(),
            ViewModelSetMethod = viewModelProperty.SetMethod?.CreateDelegate<ViewModelSetMethod<TViewModel, TValue>>(),
            ViewModelPropertyName = viewModelProperty.Name,
            ControlUpdateMode = controlUpdateMode,
            ViewModelUpdateMode = viewModelUpdateMode
        };
        builder.AddBehavior(behavior);
    }
}
