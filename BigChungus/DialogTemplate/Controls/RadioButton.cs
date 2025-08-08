public class RadioButton : DialogItem
{
    protected override string ClassName => "Button";
    public RadioButton()
    {
        TabStop = true;
        StyleHelper.SetStyle(ref style, BS_TYPEMASK, BS_AUTORADIOBUTTON);
    }

    public bool PushLike
    {
        get => StyleHelper.GetFlag(style, BS_PUSHLIKE);
        set => StyleHelper.SetFlag(ref style, BS_PUSHLIKE, value);
    }
    public bool TextMultiline
    {
        get => StyleHelper.GetFlag(style, BS_MULTILINE);
        set => StyleHelper.SetFlag(ref style, BS_MULTILINE, value);
    }
    public HAlignment TextHAlignment
    {
        get => StyleHelper.GetFlag(style, (HAlignment.Left, BS_LEFT), (HAlignment.Right, BS_RIGHT), (HAlignment.Center, 0u));
        set => StyleHelper.SetFlag(ref style, value, (HAlignment.Left, BS_LEFT), (HAlignment.Right, BS_RIGHT), (HAlignment.Center, 0u));
    }
    public VAlignment TextVAlignment
    {
        get => StyleHelper.GetFlag(style, (VAlignment.Top, BS_TOP), (VAlignment.Bottom, BS_BOTTOM), (VAlignment.Center, 0u));
        set => StyleHelper.SetFlag(ref style, value, (VAlignment.Top, BS_TOP), (VAlignment.Bottom, BS_BOTTOM), (VAlignment.Center, 0u));
    }
    public bool TextAfterButton
    {
        get => StyleHelper.GetFlag(style, BS_LEFTTEXT);
        set => StyleHelper.SetFlag(ref style, BS_LEFTTEXT, value);
    }
}
