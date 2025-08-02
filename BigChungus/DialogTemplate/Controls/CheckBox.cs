public struct CheckBox(uint style, uint exStyle) : IDialogItemProperties
{
    uint IDialogItemProperties.Style { get => style; init => style = value; }
    uint IDialogItemProperties.ExStyle { get => exStyle; init => exStyle = value; }
    void IDialogItemProperties.SetDefault()
    {
        Visible = true;
        FlagHelper.SetFlag(ref style, BS_AUTOCHECKBOX, true);
    }
    string IDialogItemProperties.ClassName => "Button";

    public bool TabStop
    {
        get => StyleHelper.GetTabStop(style);
        set => StyleHelper.SetTabStop(ref style, value);
    }
    public bool Enabled
    {
        get => StyleHelper.GetEnabled(style);
        set => StyleHelper.SetEnabled(ref style, value);
    }
    public bool Visible
    {
        get => StyleHelper.GetVisible(style);
        set => StyleHelper.SetVisible(ref style, value);
    }

    public bool PushLike
    {
        get => FlagHelper.GetFlag(style, BS_PUSHLIKE);
        set => FlagHelper.SetFlag(ref style, BS_PUSHLIKE, value);
    }
    public bool ThreeState
    {
        get => FlagHelper.GetFlag(style, (true, BS_AUTO3STATE), (false, BS_AUTOCHECKBOX));
        set => FlagHelper.SetFlag(ref style, value, (true, BS_AUTO3STATE), (false, BS_AUTOCHECKBOX));
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
