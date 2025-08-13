using System.Linq.Expressions;
using System.Reflection;

// This could be rewritten into taking any Action<TControl> and running it on WM_INITDIALOG,
// but I love how all existing API so far is purely declarative and doesn't even let you escape a control instance from lambdas,
// and most of it doesn't assume more than one line per instructions.
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
            var control = GetDialogItem(context);
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
        if (controlPropertySelector is not { Body: MemberExpression { Member: PropertyInfo { SetMethod: MethodInfo controlPropertyGetMethod } } })
        {
            throw new NotSupportedException();
        }

        var behavior = new PropertyInitializer<TViewModel, TControl, TValue>
        {
            ItemId = itemId,
            ControlSetMethod = controlPropertyGetMethod.CreateDelegate<ControlSetMethod<TControl, TValue>>(),
            Value = value
        };
        builder.AddBehavior(behavior);
    }
}
