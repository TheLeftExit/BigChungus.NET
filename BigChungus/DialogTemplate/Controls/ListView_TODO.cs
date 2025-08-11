public class ListView : DialogItem
{
    static unsafe ListView()
    {
        var c = new Win32.INITCOMMONCONTROLSEX
        {
            dwSize = (uint)sizeof(Win32.INITCOMMONCONTROLSEX),
            dwICC = ICC_LISTVIEW_CLASSES
        };
        Win32.InitCommonControlsEx(c);
    }

    protected override string ClassName => "SysListView32";

    public ListView()
    {
        StyleHelper.SetFlag(ref style, WS_BORDER, true);
        Visible = true;
        //MultiSelect = false;
        //JustifyColumns = true;
        //SelectFirstColumnOnly = false;
        StyleHelper.SetFlag(ref style, LVS_OWNERDATA, true);
        StyleHelper.SetFlag(ref style, LVS_REPORT, true);
        //StyleHelper.SetFlag(ref style, LVS_NOSORTHEADER, true);
        //StyleHelper.SetFlag(ref exStyle, LVS_EX_DOUBLEBUFFER, true);
        //StyleHelper.SetFlag(ref exStyle, LVS_EX_LABELTIP, true);
    }
    // For some reason, most flags don't work, and LVS_EX_LABELTIP shifts the scroll bar to the right!?!?!?
    // I'll deal with those later.
    /*
    public bool MultiSelect
    {
        get => !StyleHelper.GetFlag(style, LVS_SINGLESEL);
        set => StyleHelper.SetFlag(ref style, LVS_SINGLESEL, !value);
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
    */
}
