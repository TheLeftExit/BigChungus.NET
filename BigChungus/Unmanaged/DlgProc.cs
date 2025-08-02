public interface IDlgProc
{
    nint? DlgProc(Message m);
}

public readonly struct Message
{
    public Message(nint hWnd, uint msg, nuint wParam, nint lParam)
    {
        this.hWnd = hWnd;
        this.msg = msg;
        this.wParam = wParam;
        this.lParam = lParam;
    }

    public readonly nint hWnd;
    public readonly uint msg;
    public readonly nuint wParam;
    public readonly nint lParam;
}

public class DlgProcDefaultModal : IDlgProc
{
    public static IDlgProc Shared => field ??= new DlgProcDefaultModal();
    public nint? DlgProc(Message m)
    {
        if (m.msg is WM_CLOSE) Win32.EndDialog(m.hWnd, 0);
        return null;
    }
}
public class DlgProcDefaultModeless : IDlgProc
{
    public static IDlgProc Shared => field ??= new DlgProcDefaultModeless();
    public nint? DlgProc(Message m)
    {
        if (m.msg is WM_CLOSE) Win32.DestroyWindow(m.hWnd);
        return null;
    }
}
