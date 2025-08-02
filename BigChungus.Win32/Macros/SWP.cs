public static partial class Win32Macros
{
    public const UINT SWP_NOSIZE = 0x0001;
    public const UINT SWP_NOMOVE = 0x0002;
    public const UINT SWP_NOZORDER = 0x0004;
    public const UINT SWP_NOREDRAW = 0x0008;
    public const UINT SWP_NOACTIVATE = 0x0010;
    public const UINT SWP_FRAMECHANGED = 0x0020;
    public const UINT SWP_SHOWWINDOW = 0x0040;
    public const UINT SWP_HIDEWINDOW = 0x0080;
    public const UINT SWP_NOCOPYBITS = 0x0100;
    public const UINT SWP_NOOWNERZORDER = 0x0200;
    public const UINT SWP_NOSENDCHANGING = 0x0400;
    public const UINT SWP_DRAWFRAME = SWP_FRAMECHANGED;
    public const UINT SWP_NOREPOSITION = SWP_NOOWNERZORDER;
    public const UINT SWP_DEFERERASE = 0x2000;
    public const UINT SWP_ASYNCWINDOWPOS = 0x4000;
}
