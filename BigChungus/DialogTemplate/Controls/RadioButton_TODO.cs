public class RadioButton : DialogItem
{
    protected override string ClassName => "Button";
    public RadioButton()
    {
        FlagHelper.SetFlag(ref style, BS_AUTORADIOBUTTON, true);
    }

    public bool PushLike
    {
        get => FlagHelper.GetFlag(style, BS_PUSHLIKE);
        set => FlagHelper.SetFlag(ref style, BS_PUSHLIKE, value);
    }
    public bool TextMultiline
    {
        get => FlagHelper.GetFlag(style, BS_MULTILINE);
        set => FlagHelper.SetFlag(ref style, BS_MULTILINE, value);
    }
    public HAlignment TextHAlignment
    {
        get => FlagHelper.GetFlag(style, (HAlignment.Left, BS_LEFT), (HAlignment.Right, BS_RIGHT), (HAlignment.Center, 0u));
        set => FlagHelper.SetFlag(ref style, value, (HAlignment.Left, BS_LEFT), (HAlignment.Right, BS_RIGHT), (HAlignment.Center, 0u));
    }
    public VAlignment TextVAlignment
    {
        get => FlagHelper.GetFlag(style, (VAlignment.Top, BS_TOP), (VAlignment.Bottom, BS_BOTTOM), (VAlignment.Center, 0u));
        set => FlagHelper.SetFlag(ref style, value, (VAlignment.Top, BS_TOP), (VAlignment.Bottom, BS_BOTTOM), (VAlignment.Center, 0u));
    }
    public bool TextAfterButton
    {
        get => FlagHelper.GetFlag(style, BS_LEFTTEXT);
        set => FlagHelper.SetFlag(ref style, BS_LEFTTEXT, value);
    }
}
