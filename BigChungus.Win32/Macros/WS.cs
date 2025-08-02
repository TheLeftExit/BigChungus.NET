public static partial class Win32Macros
{
    public const UINT WS_OVERLAPPED = 0x00000000;
    public const UINT WS_POPUP = 0x80000000;
    public const UINT WS_CHILD = 0x40000000;
    public const UINT WS_MINIMIZE = 0x20000000;
    public const UINT WS_VISIBLE = 0x10000000;
    public const UINT WS_DISABLED = 0x08000000;
    public const UINT WS_CLIPSIBLINGS = 0x04000000;
    public const UINT WS_CLIPCHILDREN = 0x02000000;
    public const UINT WS_MAXIMIZE = 0x01000000;
    public const UINT WS_CAPTION = 0x00C00000;
    public const UINT WS_BORDER = 0x00800000;
    public const UINT WS_DLGFRAME = 0x00400000;
    public const UINT WS_VSCROLL = 0x00200000;
    public const UINT WS_HSCROLL = 0x00100000;
    public const UINT WS_SYSMENU = 0x00080000;
    public const UINT WS_THICKFRAME = 0x00040000;
    public const UINT WS_GROUP = 0x00020000;
    public const UINT WS_TABSTOP = 0x00010000;
    public const UINT WS_MINIMIZEBOX = 0x00020000;
    public const UINT WS_MAXIMIZEBOX = 0x00010000;
    public const UINT WS_TILED = WS_OVERLAPPED;
    public const UINT WS_ICONIC = WS_MINIMIZE;
    public const UINT WS_SIZEBOX = WS_THICKFRAME;
    public const UINT WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW;
    public const UINT WS_OVERLAPPEDWINDOW = (WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX);
    public const UINT WS_POPUPWINDOW = (WS_POPUP | WS_BORDER | WS_SYSMENU);
    public const UINT WS_CHILDWINDOW = (WS_CHILD);
}