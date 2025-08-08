global using System.Runtime.InteropServices;

// Type aliases to simplify manual interop declaration and verification

// BOOL/HRERSULT is declared in specific files (Boolean for LibraryImport, Int32 elsewhere)
global using UINT = uint;
global using DWORD = uint;
global using LONG = int;
global using LRESULT = nint;
global using HWND = nint;
global using WPARAM = nuint;
global using LPARAM = nint;
global using HBRUSH = nint;
global using ATOM = ushort;
global using HINSTANCE = nint;
global using HICON = nint;
global using HCURSOR = nint;
global using HMENU = nint;
global using LONG_PTR = nint;
global using LANGID = ushort;
global using ULONG = uint;
global using USHORT = ushort;
global using HMODULE = nint;
global using HANDLE = nint;
global using UINT_PTR = nuint;
global using ULONG_PTR = nuint;
global using HFONT = nint;
global using BYTE = byte;
global using WCHAR = char;
global using DWORD_PTR = nuint;
global using ULONGLONG = ulong;
global using INT_PTR = nint;
global using WORD = ushort;
global using HDC = nint;
global using COLORREF = uint;
global using HBITMAP = nint;
global using HGDIOBJ = nint;

global using unsafe PVOID = void*;
global using unsafe LPVOID = void*;
global using unsafe LPCWSTR = char*;
global using unsafe LPCDLGTEMPLATE = void*;

// Callback aliases should use the type aliases above, but C# won't let us cross-reference using statements

/// <summary>
/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nc-winuser-wndproc"/>
/// </summary>
global using unsafe WNDPROC = delegate* unmanaged[Stdcall]<nint, uint, nuint, nint, nint>;

// https://learn.microsoft.com/en-us/previous-versions/windows/desktop/legacy/ms633496(v=vs.85)
global using unsafe WNDENUMPROC = delegate* unmanaged[Stdcall]<nint, nint, int>;

// https://learn.microsoft.com/en-us/windows/win32/api/commctrl/nc-commctrl-subclassproc
global using unsafe SUBCLASSPROC = delegate* unmanaged[Stdcall]<nint, uint, nuint, nint, nuint, nuint, nint>;

// https://learn.microsoft.com/en-us/windows/win32/api/winuser/nc-winuser-dlgproc
global using unsafe DLGPROC = delegate* unmanaged[Stdcall]<nint, uint, nuint, nint, nint>;

public static unsafe partial class Win32Macros
{
    private static char* MAKEINTRESOURCE(nint atom) => (char*)atom;
}
