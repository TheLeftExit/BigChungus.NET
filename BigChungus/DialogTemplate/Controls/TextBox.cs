public class TextBox : DialogItem
{
    protected override string ClassName => "Edit";
    public TextBox()
    {
        TabStop = true;
        FlagHelper.SetFlag(ref style, WS_BORDER, true);
    }

    public bool ReadOnly
    {
        get => FlagHelper.GetFlag(style, ES_READONLY);
        set => FlagHelper.SetFlag(ref style, ES_READONLY, value);
    }
    public bool AllowHScroll
    {
        get => FlagHelper.GetFlag(style, ES_AUTOHSCROLL);
        set => FlagHelper.SetFlag(ref style, ES_AUTOHSCROLL, value);
    }
    public bool AllowVScroll
    {
        get => FlagHelper.GetFlag(style, ES_AUTOVSCROLL);
        set => FlagHelper.SetFlag(ref style, ES_AUTOVSCROLL, value);
    }
    public HAlignment TextHAlignment
    {
        get => FlagHelper.GetFlag(style, (HAlignment.Right, ES_RIGHT), (HAlignment.Center, ES_CENTER), (HAlignment.Left, ES_LEFT));
        set => FlagHelper.SetFlag(ref style, value, (HAlignment.Right, ES_RIGHT), (HAlignment.Center, ES_CENTER), (HAlignment.Left, ES_LEFT));
    }
    public bool Multiline
    {
        get => FlagHelper.GetFlag(style, ES_MULTILINE);
        set => FlagHelper.SetFlag(ref style, ES_MULTILINE, value);
    }
    public bool AcceptReturn
    {
        get => FlagHelper.GetFlag(style, ES_WANTRETURN);
        set => FlagHelper.SetFlag(ref style, ES_WANTRETURN, value);
    }
    public CharacterCase CharacterCase
    {
        get => FlagHelper.GetFlag(style, (CharacterCase.Upper, ES_UPPERCASE), (CharacterCase.Lower, ES_LOWERCASE), (CharacterCase.Any, 0u));
        set => FlagHelper.SetFlag(ref style, value, (CharacterCase.Upper, ES_UPPERCASE), (CharacterCase.Lower, ES_LOWERCASE), (CharacterCase.Any, 0u));
    }
}

public enum CharacterCase
{
    Any,
    Upper,
    Lower
}
