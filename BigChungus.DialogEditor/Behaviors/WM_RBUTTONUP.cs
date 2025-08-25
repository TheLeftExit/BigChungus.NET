using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public unsafe class WmRButtonUpHandler : IDialogBehavior<DialogEditorViewModel>
{
    unsafe nint? IDialogBehavior<DialogEditorViewModel>.OnMessageReceived(Message message, IDialogContext<DialogEditorViewModel> context)
    {
        if (message.msg is not WM_RBUTTONUP) return null;

        var menuViewModel = new SimpleMenuViewModel()
        {
            Items = ["Create a text box", "Nevermind"]
        };
        var view = new SimpleMenuView();
        view.ShowDialog(menuViewModel, context.DialogBoxHandle);
        if (menuViewModel.SelectedIndex != 0) return null;

        var lParam = new DWord((nuint)message.lParam);
        var dluHelper = new DLUHelper(context.DialogBoxHandle);

        var xDlu = (int)Math.Floor(dluHelper.PixelsToDLUHorz(lParam.XParam) / context.ViewModel.DLUGridSize.Width) * context.ViewModel.DLUGridSize.Width;
        var yDlu = (int)Math.Floor(dluHelper.PixelsToDLUVert(lParam.YParam) / context.ViewModel.DLUGridSize.Height) * context.ViewModel.DLUGridSize.Height;
        var x = dluHelper.DLUToPixelsHorz(xDlu);
        var y = dluHelper.DLUToPixelsVert(yDlu);

        var right = dluHelper.DLUToPixelsHorz(xDlu + 48);
        var bottom = dluHelper.DLUToPixelsVert(yDlu + 12);

        var dialogItem = (IDialogItemProperties)new TextBox() { AllowHScroll = true };
        var handle = Win32.CreateWindowEx(
            dialogItem.ExStyle,
            dialogItem.ClassName,
            "Text",
            (dialogItem.Style | WS_CHILD),
            x, y, right - x, bottom - y,
            context.DialogBoxHandle,
            0,
            NativeLibrary.GetMainProgramHandle(),
            null
        ).ThrowIf(0);
        var font = Win32.SendMessage(context.DialogBoxHandle, WM_GETFONT, 0, 0);

        var properties = new ControlProperties(handle);
        Win32.SendMessage(handle, WM_SETFONT, (nuint)font, 1);
        SubclassHelper.Subclass(handle, context.DialogBoxHandle);

        return null;
    }
}

file static class SubclassHelper
{
    public static unsafe void Subclass(nint controlHandle, nint parentHandle)
    {
        Win32.SetWindowSubclass(controlHandle, &SubclassProc, WM_REFLECT, (nuint)parentHandle).ThrowIfFalse();

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        static nint SubclassProc(nint hWnd, uint msg, nuint wParam, nint lParam, nuint uIdSubclass, nuint dwRefData)
        {
            var message = new Message(hWnd, msg, wParam, lParam);
            return Win32.SendMessage((nint)dwRefData, WM_REFLECT, 0, (nint)(&message));
        }
    }
}
