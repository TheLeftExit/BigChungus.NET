public class DetailsView : DialogItem
{
    protected override string ClassName => "ListView";

    public DetailsView()
    {
        Visible = true;
        MultiSelect = false;
        StyleHelper.SetFlag(ref style, LVS_REPORT, true);
        StyleHelper.SetFlag(ref style, LVS_NOSORTHEADER, true);
        StyleHelper.SetFlag(ref exStyle, LVS_EX_DOUBLEBUFFER, true);
        StyleHelper.SetFlag(ref exStyle, LVS_EX_LABELTIP, true);
    }

    public bool MultiSelect
    {
        get => !StyleHelper.GetFlag(style, LVS_SINGLESEL);
        set => StyleHelper.SetFlag(ref style, LVS_SINGLESEL, !value);
    }

    public bool VirtualMode
    {
        get => StyleHelper.GetFlag(style, LVS_OWNERDATA);
        set => StyleHelper.SetFlag(ref style, LVS_OWNERDATA, value);
    }

    public bool SelectFirstColumnOnly
    {
        get => !StyleHelper.GetFlag(exStyle, LVS_EX_FULLROWSELECT);
        set => StyleHelper.SetFlag(ref exStyle, LVS_EX_FULLROWSELECT, !value);
    }

    public bool ShowGridLines
    {
        get => StyleHelper.GetFlag(exStyle, LVS_EX_GRIDLINES);
        set => StyleHelper.SetFlag(ref exStyle, LVS_EX_GRIDLINES, value);
    }

    public bool AllowColumnReordering
    {
        get => StyleHelper.GetFlag(exStyle, LVS_EX_HEADERDRAGDROP);
        set => StyleHelper.SetFlag(ref exStyle, LVS_EX_HEADERDRAGDROP, value);
    }

    public bool JustifyColumns // Couldn't pick a better name...
    {
        get => StyleHelper.GetFlag(exStyle, LVS_EX_JUSTIFYCOLUMNS);
        set => StyleHelper.SetFlag(ref exStyle, LVS_EX_JUSTIFYCOLUMNS, value);
    }
}
