using System.Linq.Expressions;
using System.Reflection;

public class RadioGroupBinding<TViewModel, TValue> : IDialogBehavior<TViewModel>
    where TViewModel : class
{
    public required (ushort ItemId, TValue value)[] ItemValueMap { get; init; }
    public required ViewModelGetMethod<TViewModel, TValue>? ViewModelGetMethod { private get; init; }
    public required ViewModelSetMethod<TViewModel, TValue>? ViewModelSetMethod { private get; init; }
    public required string? ViewModelPropertyName { private get; init; }

    nint? IDialogBehavior<TViewModel>.OnMessageReceived(Message message, nint dialogBoxHandle, TViewModel viewModel)
    {
        if(message.msg is WM_INITDIALOG)
        {
            Validate(dialogBoxHandle);
            return null;
        }

        if (ViewModelSetMethod is null) return null;

        if(!RadioButtonControl.IsCommandMessage(message, out var command)) return null;
        if (command is not ButtonCommand.Click) return null;

        foreach(var (itemId, value) in ItemValueMap)
        {
            var handle = Win32.GetDlgItem(dialogBoxHandle, itemId);
            var control = new RadioButtonControl(handle);
            if (!control.IsCommandSender(message, command)) continue;

            ViewModelSetMethod(viewModel, value);
            return null;
        }
        return null;
    }

    void IDialogBehavior<TViewModel>.OnPropertyChanged(string? propertyName, nint dialogBoxHandle, TViewModel viewModel)
    {
        if (ViewModelGetMethod is null || ViewModelPropertyName is null) return;
        if (propertyName is not null && propertyName != ViewModelPropertyName) return;
        var viewModelValue = ViewModelGetMethod(viewModel);

        foreach (var (itemId, value) in ItemValueMap)
        {
            var handle = Win32.GetDlgItem(dialogBoxHandle, itemId);
            var control = new RadioButtonControl(handle);
            control.IsChecked = EqualityComparer<TValue>.Default.Equals(value, viewModelValue);
        }
    }

    // Current binding API trades compile-time safety for simplicity, so we want to validate the user-selected radio group.
    private void Validate(nint dialogBoxHandle)
    {
        // Ensuring that items are sequential and don't duplicate
        var orderedItemIds = ItemValueMap.Select(x => (int)x.ItemId).Order().ToArray();
        if (!orderedItemIds.SequenceEqual(Enumerable.Range(orderedItemIds[0], ItemValueMap.Length)))
        {
            throw new InvalidOperationException("Invalid radio group specified.");
        }
        // Ensuring that the first item starts a group
        var firstButtonHandle = Win32.GetDlgItem(dialogBoxHandle, orderedItemIds[0]);
        var firstButtonStyle = (uint)Win32.GetWindowLongPtr(firstButtonHandle, GWLx_STYLE);
        if(!StyleHelper.GetFlag(firstButtonStyle, WS_GROUP))
        {
            throw new InvalidOperationException("Invalid radio group specified.");
        }
        // Ensuring that all other items don't start groups
        for(int i = 1; i < orderedItemIds.Length; i++)
        {
            var buttonHandle = Win32.GetDlgItem(dialogBoxHandle, orderedItemIds[i]);
            var buttonStyle = (uint)Win32.GetWindowLongPtr(buttonHandle, GWLx_STYLE);
            if (StyleHelper.GetFlag(buttonStyle, WS_GROUP))
            {
                throw new InvalidOperationException("Invalid radio group specified.");
            }
        }
        // Ensuring that the last item ends the group
        var nextControlHandle = Win32.GetDlgItem(dialogBoxHandle, orderedItemIds[^1] + 1);
        if(nextControlHandle != 0)
        {
            var nextControlStyle = (uint)Win32.GetWindowLongPtr(nextControlHandle, GWLx_STYLE);
            if (!StyleHelper.GetFlag(nextControlStyle, WS_GROUP))
            {
                throw new InvalidOperationException("Invalid radio group specified.");
            }
        }
    }
}

public static partial class DialogProcedureBuilderExtensions
{
    public static void SetRadioGroupBinding<TViewModel, TValue>(
        this IDialogProcedureBuilder<TViewModel> builder,
        Expression<Func<TViewModel, TValue>> viewModelPropertySelector,
        params ReadOnlySpan<(DialogItemHandle<RadioButton>, TValue)> itemValueMap
    )
        where TViewModel : class
    {
        var viewModelProperty = (viewModelPropertySelector.Body as MemberExpression)?.Member as PropertyInfo;
        if (viewModelProperty is null) throw new NotSupportedException();

        var mapArray = new (ushort, TValue)[itemValueMap.Length];
        for (int i = 0; i < itemValueMap.Length; i++)
        {
            var (handle, value) = itemValueMap[i];
            mapArray[i] = (handle.Id!.Value, value);
        }

        var behavior = new RadioGroupBinding<TViewModel, TValue>
        {
            ItemValueMap = mapArray,
            ViewModelGetMethod = viewModelProperty.GetMethod?.CreateDelegate<ViewModelGetMethod<TViewModel, TValue>>(),
            ViewModelSetMethod = viewModelProperty.SetMethod?.CreateDelegate<ViewModelSetMethod<TViewModel, TValue>>(),
            ViewModelPropertyName = viewModelProperty.Name
        };
        builder.AddBehavior(behavior);
    }
}
