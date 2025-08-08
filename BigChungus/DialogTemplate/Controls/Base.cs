public interface IDialogItemProperties
{
    uint Style { get; init; }
    uint ExStyle { get; init; }
    string ClassName { get; }
}

public abstract class DialogItem : IDialogItemProperties
{
    protected uint style;
    protected uint exStyle;

    protected abstract string ClassName { get; }
    
    protected DialogItem()
    {
        Visible = true;
    }

    public bool TabStop
    {
        get => StyleHelper.GetFlag(style, WS_TABSTOP);
        set => StyleHelper.SetFlag(ref style, WS_TABSTOP, value);
    }

    public bool Enabled
    {
        get => !StyleHelper.GetFlag(style, WS_DISABLED);
        set => StyleHelper.SetFlag(ref style, WS_DISABLED, !value);
    }

    public bool Visible
    {
        get => StyleHelper.GetFlag(style, WS_VISIBLE);
        set => StyleHelper.SetFlag(ref style, WS_VISIBLE, value);
    }

    protected bool StartGroup
    {
        get => StyleHelper.GetFlag(style, WS_GROUP);
        set => StyleHelper.SetFlag(ref style, WS_GROUP, value);
    }

    uint IDialogItemProperties.Style { get => style; init => style = value; }
    uint IDialogItemProperties.ExStyle { get => exStyle; init => exStyle = value; }
    string IDialogItemProperties.ClassName => ClassName;
}

public enum HAlignment { Left, Center, Right }

public enum VAlignment { Top, Center, Bottom }
