using System.Linq.Expressions;
using System.Reflection;

public enum ListViewCommand
{
    ItemClick = default,
    GetItemInfo,
    StateChanged,
    ItemChanged
}

public readonly struct ListViewControl : IControl<ListViewControl, ListViewCommand>
{
    private readonly nint _handle;
    public ListViewControl(nint hWnd) => _handle = hWnd;
    static ListViewControl IControl<ListViewControl>.Create(nint handle) => new(handle);
    nint IControl<ListViewControl>.Handle => _handle;

    public static unsafe bool IsCommandMessage(Message message, out ListViewCommand command)
    {
        if (message.msg is WM_NOTIFY)
        {
            var nmhdr = (Win32.NMHDR*)message.lParam;
            if (nmhdr->code is LVN_GETDISPINFO)
            {
                command = ListViewCommand.GetItemInfo;
                return true;
            }
            if(nmhdr->code is LVN_ITEMACTIVATE)
            {
                command = ListViewCommand.ItemClick;
                return true;
            }
            if(nmhdr->code is LVN_ODSTATECHANGED)
            {
                command = ListViewCommand.StateChanged;
                return true;
            }
            if(nmhdr->code is LVN_ITEMCHANGED)
            {
                command = ListViewCommand.ItemChanged;
                return true;
            }
        }
        command = default;
        return false;
    }

    public unsafe bool IsCommandSender(Message message, ListViewCommand command)
    {
        return command switch
        {
            ListViewCommand.ItemClick
            or ListViewCommand.GetItemInfo
            or ListViewCommand.StateChanged
            or ListViewCommand.ItemChanged => ((Win32.NMLVDISPINFO*)message.lParam)->hdr.hwndFrom == _handle,
            _ => false
        };
    }
}
