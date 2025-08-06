public interface IDialogTemplateBuilder
{
    DialogItemHandle<T> AddItem<T>(RectangleDLU bounds, string? text, Action<T>? initialize)
        where T : IDialogItemProperties, new();
}

public record struct DialogItemHandle<T>(ushort? Id) where T : IDialogItemProperties;

public ref struct RadioGroupScope
{
    private readonly DialogTemplateBuilder _builder;
    public RadioGroupScope(DialogTemplateBuilder builder)
    {
        _builder = builder;
    }
    public void Dispose()
    {
        _builder.EndRadioGroup();
    }
}

public class DialogTemplateBuilder : IDialogTemplateBuilder
{
    private readonly DlgTemplate _template = new();
    private ushort nextItemId = 1000;

    public DialogProperties Properties => field ??= new DialogProperties(_template);

    private bool _inRadioGroup = false;
    private bool _atRadioGroupBoundary = false;

    public RadioGroupScope BeginRadioGroup()
    {
        if (_inRadioGroup) throw new InvalidOperationException("Recursive 'BeginRadioGroup' scope.");
        _inRadioGroup = true;
        _atRadioGroupBoundary = true;
        return new RadioGroupScope(this);
    }

    public void EndRadioGroup()
    {
        if(!_inRadioGroup) throw new InvalidOperationException("Call to 'EndRadioGroup' outside of a 'BeginRadioGroup' scope.");
        if(_atRadioGroupBoundary) throw new InvalidOperationException("Radio group does not contain any items.");
        _inRadioGroup = false;
        _atRadioGroupBoundary = true;
    }

    public DialogItemHandle<T> AddItem<T>(RectangleDLU bounds, string? text, Action<T>? initialize)
        where T : IDialogItemProperties, new()
    {
        if (!_inRadioGroup && typeof(T) == typeof(RadioButton))
        {
            throw new InvalidOperationException("RadioButton must be added inside a 'BeginRadioGroup' scope.");
        }
        if(_inRadioGroup && typeof(T) != typeof(RadioButton))
        {
            throw new InvalidOperationException("Only RadioButton can be added inside a 'BeginRadioGroup' scope.");
        }

        var properties = new T();
        if(initialize is not null)
        {
            initialize(properties);
        }

        var(style, exStyle) = (properties.Style, properties.ExStyle);

        if (_atRadioGroupBoundary)
        {
            style |= WS_GROUP;
            _atRadioGroupBoundary = false;
        }

        var item = new DlgItemTemplate()
        {
            Style = style,
            ExStyle = exStyle,
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
