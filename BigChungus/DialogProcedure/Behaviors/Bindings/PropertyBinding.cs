using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Each update mode also reacts to messages matching all subsequent update modes, hence explicit values.
public enum ViewModelUpdateMode
{
    OnPropertyChanged = 0,
    OnValidation = 1,
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
    public required ControlGetMethod<TControl, TValue> ControlGetMethod { private get; init; }
    public required ControlSetMethod<TControl, TValue>? ControlSetMethod { private get; init; }
    public required TCommand? ControlCommand { private get; init; }

    public required ControlUpdateMode ControlUpdateMode { private get; init; }
    public required ViewModelGetMethod<TViewModel, TValue> ViewModelGetMethod { private get; init; }
    public required ViewModelSetMethod<TViewModel, TValue>? ViewModelSetMethod { private get; init; }
    public required string? ViewModelPropertyName { private get; init; }

    protected override void OnMessageReceived(Message message, IDialogContext<TViewModel> context)
    {
        if (message.msg is WM_INITDIALOG)
        {
            var control = GetDialogItem(context);
            SubclassHelper.SubclassForLostFocus(control.Handle); // Repeat SetWindowSubclass calls are allowed by comctl32.
            Push(PushDirection.ToControl, context);
            return;

        }
        if (ControlUpdateMode == ControlUpdateMode.OnPropertyChanged && IsViewModelPropertyChanged(message, context))
        {
            Push(PushDirection.ToControl, context);
            return;
        }

        if(ViewModelUpdateMode <= ViewModelUpdateMode.OnPropertyChanged && IsControlPropertyChangedMessage(message, context))
        {
            Push(PushDirection.ToViewModel, context);
            return;
        }

        if(ViewModelUpdateMode <= ViewModelUpdateMode.OnValidation && IsLoseFocusMessage(message, context))
        {
            Push(PushDirection.ToViewModel, context);
            return;
        }

        if(ViewModelUpdateMode <= ViewModelUpdateMode.OnDialogClose && IsDialogCloseMessage(message, context))
        {
            Push(PushDirection.ToViewModel, context);
            return;
        }
    }

    private enum PushDirection { ToControl, ToViewModel }

    private bool ValuesMatch(IDialogContext<TViewModel> context)
    {
        var control = GetDialogItem(context);
        var controlValue = ControlGetMethod(control);
        var viewModelValue = ViewModelGetMethod(context.ViewModel);
        return EqualityComparer<TValue>.Default.Equals(viewModelValue, controlValue);
    }

    private bool _isBouncing = false;
    private void Push(PushDirection direction, IDialogContext<TViewModel> context)
    {
        if (_isBouncing) return;
        _isBouncing = true;
        while (!ValuesMatch(context))
        {
            var pushed = direction switch
            {
                PushDirection.ToControl => TryPushToControl(context),
                PushDirection.ToViewModel => TryPushToViewModel(context),
                _ => throw new UnreachableException()
            };
            if (!pushed) return;
            direction = direction == PushDirection.ToControl ? PushDirection.ToViewModel : PushDirection.ToControl;
        }
        _isBouncing = false;
    }
    [MemberNotNullWhen(true, nameof(ControlSetMethod))]
    private bool CanPushToControl() => ControlUpdateMode is not ControlUpdateMode.Never && ControlSetMethod is not null;
    private bool TryPushToControl(IDialogContext<TViewModel> context)
    {
        if (!CanPushToControl()) return false;
        var control = GetDialogItem(context);
        var value = ViewModelGetMethod(context.ViewModel);
        ControlSetMethod(control, value);
        return true;
    }
    [MemberNotNullWhen(true, nameof(ViewModelSetMethod))]
    private bool CanPushToViewModel() => ViewModelUpdateMode is not ViewModelUpdateMode.Never && ViewModelSetMethod is not null;
    private bool TryPushToViewModel(IDialogContext<TViewModel> context)
    {
        if (!CanPushToViewModel()) return false;
        var control = GetDialogItem(context);
        var value = ControlGetMethod(control);
        ViewModelSetMethod(context.ViewModel, value);
        return true;
    }

    private bool IsViewModelPropertyChanged(Message message, IDialogContext<TViewModel> context)
    {
        if (!PropertyChangedEventArgs.Parse(message, out var e)) return false;
        return e.PropertyName is null || e.PropertyName == ViewModelPropertyName;
    }
    private bool IsControlPropertyChangedMessage(Message message, IDialogContext<TViewModel> context)
    {
        if (!TControl.IsCommandMessage(message, out var command)) return false;
        if (!EqualityComparer<TCommand?>.Default.Equals(command, ControlCommand)) return false;
        var control = GetDialogItem(context);
        return control.IsCommandSender(message, command);
    }
    private bool IsLoseFocusMessage(Message message, IDialogContext<TViewModel> context)
    {
        if (message.msg is not WM_KILLFOCUS_REFLECT) return false;
        var control = GetDialogItem(context);
        return message.lParam == control.Handle;
    }
    private bool IsDialogCloseMessage(Message message, IDialogContext<TViewModel> context)
    {
        return message.msg is WM_DESTROY;
    }
}

file static class SubclassHelper // Separate `file static` because PropertyBinding is generic and UnmanagedCallersOnly doesn't like that.
{
    public static unsafe void SubclassForLostFocus(nint controlHandle)
    {
        Win32.SetWindowSubclass(controlHandle, &SubclassProc, WM_KILLFOCUS_REFLECT, 0).ThrowIfFalse();

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        static nint SubclassProc(nint hWnd, uint msg, nuint wParam, nint lParam, nuint uIdSubclass, nuint dwRefData)
        {
            if (msg == WM_KILLFOCUS)
            {
                var parentHandle = Win32.GetParent(hWnd);
                if (parentHandle != 0)
                {
                    // Showing dialogs and moving focus inside WM_KILLFOCUS is a big no-no, hence PostMessage.
                    Win32.PostMessage(parentHandle, WM_KILLFOCUS_REFLECT, wParam, hWnd);
                }
            }
            return Win32.DefSubclassProc(hWnd, msg, wParam, lParam);
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
        ViewModelUpdateMode viewModelUpdateMode = ViewModelUpdateMode.OnValidation,
        ControlUpdateMode controlUpdateMode = ControlUpdateMode.OnPropertyChanged
    )
        where TViewModel : class
        where TControl : struct, IControl<TControl, TCommand>
        where TCommand : struct, Enum
    {
        if (controlPropertySelector is not { Body: MemberExpression { Member: PropertyInfo controlProperty and { GetMethod: not null } } })
        {
            throw new NotSupportedException();
        }
        if (viewModelPropertySelector is not { Body: MemberExpression { Member: PropertyInfo viewModelProperty and { GetMethod: not null } } })
        {
            throw new NotSupportedException();
        }

        var behavior = new PropertyBinding<TViewModel, TControl, TCommand, TValue>
        {
            ItemId = itemId,
            ControlGetMethod = controlProperty.GetMethod.CreateDelegate<ControlGetMethod<TControl, TValue>>(),
            ControlSetMethod = controlProperty.SetMethod?.CreateDelegate<ControlSetMethod<TControl, TValue>>(),
            ControlCommand = controlProperty.GetCustomAttribute<PropertyChangedCommandAttribute<TCommand>>()?.Command,
            ViewModelGetMethod = viewModelProperty.GetMethod.CreateDelegate<ViewModelGetMethod<TViewModel, TValue>>(),
            ViewModelSetMethod = viewModelProperty.SetMethod?.CreateDelegate<ViewModelSetMethod<TViewModel, TValue>>(),
            ViewModelPropertyName = viewModelProperty.Name,
            ControlUpdateMode = controlUpdateMode,
            ViewModelUpdateMode = viewModelUpdateMode
        };
        builder.AddBehavior(behavior);
    }
}
