public class GroupBox : DialogItem
{
    protected override string ClassName => "Button";
    public GroupBox()
    {
        FlagHelper.SetFlag(ref style, BS_GROUPBOX, true);
    }

    public HAlignment TextHAlignment
    {
        get => FlagHelper.GetFlag(style, (HAlignment.Left, BS_LEFT), (HAlignment.Right, BS_RIGHT), (HAlignment.Center, 0u));
        set => FlagHelper.SetFlag(ref style, value, (HAlignment.Left, BS_LEFT), (HAlignment.Right, BS_RIGHT), (HAlignment.Center, 0u));
    }
}
