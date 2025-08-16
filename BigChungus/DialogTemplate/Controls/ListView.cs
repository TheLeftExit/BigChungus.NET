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
        Visible = true;
        TabStop = true;
        StyleHelper.SetFlag(ref style, WS_BORDER, true);
        StyleHelper.SetFlag(ref style, LVS_OWNERDATA, true);
        StyleHelper.SetFlag(ref style, LVS_REPORT, true);
        StyleHelper.SetFlag(ref style, LVS_SINGLESEL, true);
    }
}
