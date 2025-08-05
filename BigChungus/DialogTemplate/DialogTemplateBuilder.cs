public interface IDialogTemplateBuilder
{
    DialogItemHandle<T> AddItem<T>(RectangleDLU bounds, string? text, Action<T>? initialize)
        where T : IDialogItemProperties, new();
}

public record struct DialogItemHandle<T>(ushort? Id) where T : IDialogItemProperties;

public class DialogTemplateBuilder : IDialogTemplateBuilder
{
    private readonly DlgTemplate _template = new();
    private ushort nextItemId = 1000;

    public DialogProperties Properties => field ??= new DialogProperties(_template);

    public DialogItemHandle<T> AddItem<T>(RectangleDLU bounds, string? text, Action<T>? initialize)
        where T : IDialogItemProperties, new()
    {
        var properties = new T();
        if(initialize is not null)
        {
            initialize(properties);
        }

        var item = new DlgItemTemplate()
        {
            Style = properties.Style,
            ExStyle = properties.ExStyle,
            X = bounds.X,
            Y = bounds.Y,
            Width = bounds.Width,
            Height = bounds.Height,
            Id = nextItemId++,
            WindowText = text,
            WindowClass = properties.ClassName,
        };
        _template.Items.Add(item);
        return new DialogItemHandle<T>(item.Id);
    }

    public ReadOnlyMemory<byte> Build()
    {
        return _template.ToMemory();
    }
}

public static partial class DialogTemplateBuilderExtensions
{
    public static DialogItemHandle<T> AddItem<T>(
        this IDialogTemplateBuilder builder,
        RectangleDLU bounds,
        string? text = null,
        Action<T>? initialize = null
    )
        where T : IDialogItemProperties, new()
    {
        return builder.AddItem(bounds, text, initialize);
    }

    public static DialogItemHandle<T> AddItem<T>(
        this IDialogTemplateBuilder builder,
        RectangleDLU bounds,
        Action<T>? initialize
    )
        where T : IDialogItemProperties, new()
    {
        return builder.AddItem(bounds, null, initialize);
    }
}
