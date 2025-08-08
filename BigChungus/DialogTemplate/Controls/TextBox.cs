public class TextBox : DialogItem
{
    protected override string ClassName => "Edit";
    public TextBox()
    {
        TabStop = true;
        StyleHelper.SetFlag(ref style, WS_BORDER, true);
    }

    public bool ReadOnly
    {
        get => StyleHelper.GetFlag(style, ES_READONLY);
        set => StyleHelper.SetFlag(ref style, ES_READONLY, value);
    }
    public bool AllowHScroll
    {
        get => StyleHelper.GetFlag(style, ES_AUTOHSCROLL);
        set => StyleHelper.SetFlag(ref style, ES_AUTOHSCROLL, value);
    }
    public bool AllowVScroll
    {
        get => StyleHelper.GetFlag(style, ES_AUTOVSCROLL);
        set => StyleHelper.SetFlag(ref style, ES_AUTOVSCROLL, value);
    }
    public HAlignment TextHAlignment
    {
        get => StyleHelper.GetFlag(style, (HAlignment.Right, ES_RIGHT), (HAlignment.Center, ES_CENTER), (HAlignment.Left, ES_LEFT));
        set => StyleHelper.SetFlag(ref style, value, (HAlignment.Right, ES_RIGHT), (HAlignment.Center, ES_CENTER), (HAlignment.Left, ES_LEFT));
    }
    public bool Multiline
    {
        get => StyleHelper.GetFlag(style, ES_MULTILINE);
        set => StyleHelper.SetFlag(ref style, ES_MULTILINE, value);
    }
    public bool AcceptReturn
    {
        get => StyleHelper.GetFlag(style, ES_WANTRETURN);
        set => StyleHelper.SetFlag(ref style, ES_WANTRETURN, value);
    }
    public CharacterCase CharacterCase
    {
        get => StyleHelper.GetFlag(style, (CharacterCase.Upper, ES_UPPERCASE), (CharacterCase.Lower, ES_LOWERCASE), (CharacterCase.Any, 0u));
        set => StyleHelper.SetFlag(ref style, value, (CharacterCase.Upper, ES_UPPERCASE), (CharacterCase.Lower, ES_LOWERCASE), (CharacterCase.Any, 0u));
    }
}

public enum CharacterCase
{
    Any,
    Upper,
    Lower
}
