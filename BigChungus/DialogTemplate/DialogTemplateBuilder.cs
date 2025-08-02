public delegate void DialogItemInitialize<T>(ref T item) where T : IDialogItemProperties;

public interface IDialogTemplateBuilder
{
    DialogItemHandle<T> AddItem<T>(RectangleDLU bounds, string? text, DialogItemInitialize<T>? initialize)
        where T : IDialogItemProperties, new();
}

public record struct DialogItemHandle<T>(ushort? Id) where T : IDialogItemProperties;

public class DialogTemplateBuilder : IDialogTemplateBuilder
{
    private readonly DlgTemplate _template = new();

    private ushort nextItemId = 1000;

    public DialogItemHandle<T> AddItem<T>(RectangleDLU bounds, string? text, DialogItemInitialize<T>? initialize)
        where T : IDialogItemProperties, new()
    {
        var properties = new T();
        properties.SetDefault();
        if(initialize is not null)
        {
            initialize(ref properties);
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
