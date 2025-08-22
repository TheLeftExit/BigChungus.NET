public static partial class Win32Macros
{
    // A lot of this is NOT menu-related, but there doesn't seem to be a definitive list of relevant macros online, so whatever for now.
    public const UINT MF_INSERT = 0x00000000;
    public const UINT MF_CHANGE = 0x00000080;
    public const UINT MF_APPEND = 0x00000100;
    public const UINT MF_DELETE = 0x00000200;
    public const UINT MF_REMOVE = 0x00001000;
    public const UINT MF_BYCOMMAND = 0x00000000;
    public const UINT MF_BYPOSITION = 0x00000400;
    public const UINT MF_SEPARATOR = 0x00000800;
    public const UINT MF_ENABLED = 0x00000000;
    public const UINT MF_GRAYED = 0x00000001;
    public const UINT MF_DISABLED = 0x00000002;
    public const UINT MF_UNCHECKED = 0x00000000;
    public const UINT MF_CHECKED = 0x00000008;
    public const UINT MF_USECHECKBITMAPS = 0x00000200;
    public const UINT MF_STRING = 0x00000000;
    public const UINT MF_BITMAP = 0x00000004;
    public const UINT MF_OWNERDRAW = 0x00000100;
    public const UINT MF_POPUP = 0x00000010;
    public const UINT MF_MENUBARBREAK = 0x00000020;
    public const UINT MF_MENUBREAK = 0x00000040;
    public const UINT MF_UNHILITE = 0x00000000;
    public const UINT MF_HILITE = 0x00000080;
    public const UINT MF_DEFAULT = 0x00001000;
    public const UINT MF_SYSMENU = 0x00002000;
    public const UINT MF_HELP = 0x00004000;
    public const UINT MF_RIGHTJUSTIFY = 0x00004000;
    public const UINT MF_MOUSESELECT = 0x00008000;
    public const UINT MF_END = 0x00000080;
    public const UINT MF_HSZ_INFO = 0x01000000;
    public const UINT MF_SENDMSGS = 0x02000000;
    public const UINT MF_POSTMSGS = 0x04000000;
    public const UINT MF_CALLBACKS = 0x08000000;
    public const UINT MF_ERRORS = 0x10000000;
    public const UINT MF_LINKS = 0x20000000;
    public const UINT MF_CONV = 0x40000000;
    public const UINT MF_MASK = 0xFF000000;
}
