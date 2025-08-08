public class GroupBox : DialogItem
{
    protected override string ClassName => "Button";
    public GroupBox()
    {
        StyleHelper.SetStyle(ref style, BS_TYPEMASK, BS_GROUPBOX);
    }

    public HAlignment TextHAlignment
    {
        get => StyleHelper.GetFlag(style, (HAlignment.Left, BS_LEFT), (HAlignment.Right, BS_RIGHT), (HAlignment.Center, 0u));
        set => StyleHelper.SetFlag(ref style, value, (HAlignment.Left, BS_LEFT), (HAlignment.Right, BS_RIGHT), (HAlignment.Center, 0u));
    }
}
