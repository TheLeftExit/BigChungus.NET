using System.Linq.Expressions;
using System.Reflection;

public sealed class PropertyInitializer<TViewModel, TControl, TValue> : DialogBinding<TViewModel, TControl>
    where TViewModel : class
    where TControl : struct, IControl<TControl>
{
    public required ControlSetMethod<TControl, TValue> ControlSetMethod { get; init; }
    public required TValue Value { get; init; }

    protected override void OnMessageReceived(Message message, IDialogContext<TViewModel> context)
    {
        if(message.msg is WM_INITDIALOG)
        {
            var control = GetDialogItem(context.DialogBoxHandle);
            ControlSetMethod(control, Value);
        }
    }
}

public static partial class DialogProcedureBuilderExtensions
{
    public static void SetProperty<TViewModel, TControl, TValue>(
        this IDialogProcedureBuilder<TViewModel> builder,
        ushort? itemId,
        Expression<Func<TControl, TValue>> controlPropertySelector,
        TValue value
    )
        where TViewModel : class
        where TControl : struct, IControl<TControl>
    {
        var controlProperty = (controlPropertySelector.Body as MemberExpression)?.Member as PropertyInfo;
        if (controlProperty?.SetMethod is null) throw new NotSupportedException();

        var behavior = new PropertyInitializer<TViewModel, TControl, TValue>
        {
            ItemId = itemId,
            ControlSetMethod = controlProperty.SetMethod.CreateDelegate<ControlSetMethod<TControl, TValue>>(),
            Value = value
        };
        builder.AddBehavior(behavior);
    }
}
