using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public static partial class DialogBoxHelper
{
    public unsafe static nint DialogBox(ReadOnlySpan<byte> template, IDlgProc? handler = null, nint parentHandle = 0)
    {
        handler ??= DlgProcDefaultModal.Shared;
        using var paddedMemory = new PaddedMemory(template);
        return Win32.DialogBoxIndirectParam(
            hInstance: NativeLibrary.GetMainProgramHandle(),
            hDialogTemplate: paddedMemory.Pointer,
            hWndParent: parentHandle,
            lpDialogFunc: &DlgProc,
            dwInitParam: (nint)GCHandle.Alloc(handler)
        );
    }

    public unsafe static nint CreateDialog(ReadOnlySpan<byte> template, IDlgProc? handler = null, nint parentHandle = 0)
    {
        handler ??= DlgProcDefaultModeless.Shared;
        using var paddedMemory = new PaddedMemory(template);
        return Win32.CreateDialogIndirectParam(
            hInstance: NativeLibrary.GetMainProgramHandle(),
            hDialogTemplate: paddedMemory.Pointer,
            hWndParent: parentHandle,
            lpDialogFunc: &DlgProc,
            dwInitParam: (nint)GCHandle.Alloc(handler)
        );
    }

    // Should address buffer overflows from invalid templates, and make it meaningful to accept a span rather than a ref byte.
    private unsafe readonly ref struct PaddedMemory
    {
        public readonly void* Pointer => _memoryHandle.Pointer;
        private readonly IMemoryOwner<byte> _memoryOwner;
        private readonly MemoryHandle _memoryHandle;
        public PaddedMemory(ReadOnlySpan<byte> bytes)
        {
            _memoryOwner = MemoryPool<byte>.Shared.Rent(bytes.Length + 1024);
            _memoryHandle = _memoryOwner.Memory.Pin();
            bytes.CopyTo(_memoryOwner.Memory.Span);
        }

        public void Dispose()
        {
            _memoryHandle.Dispose();
            _memoryOwner.Dispose();
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static nint DlgProc(nint hWnd, uint msg, nuint wParam, nint lParam)
    {
        if (msg is WM_INITDIALOG)
        {
            Win32.SetWindowLongPtr(hWnd, DWLx_USER, lParam);
        }

        var dwlxUser = Win32.GetWindowLongPtr(hWnd, DWLx_USER);
        if (dwlxUser is 0) return 0;
        var gcHandle = (GCHandle)dwlxUser;
        var handler = (IDlgProc)gcHandle.Target!;

        var result = handler.DlgProc(new(hWnd, msg, wParam, lParam));

        if (msg is WM_NCDESTROY)
        {
            gcHandle.Free();
        }

        switch (msg)
        {
            case WM_CHARTOITEM:
            case WM_VKEYTOITEM:
                return result ?? -1;
            case WM_COMPAREITEM:
                return result ?? 0;
            case WM_CTLCOLORBTN:
            case WM_CTLCOLORDLG:
            case WM_CTLCOLOREDIT:
            case WM_CTLCOLORLISTBOX:
            case WM_CTLCOLORSCROLLBAR:
            case WM_CTLCOLORSTATIC:
            case WM_QUERYDRAGICON:
                return result ?? 0;
            case WM_INITDIALOG:
                return result ?? 1;
            default:
                Win32.SetWindowLongPtr(hWnd, DWLx_MSGRESULT, result ?? 0);
                return result.HasValue ? 1 : 0;
        }
    }
}
