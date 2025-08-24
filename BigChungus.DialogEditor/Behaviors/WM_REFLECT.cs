using System.Diagnostics;

public unsafe class WmReflectHandler : IDialogBehavior<DialogEditorViewModel>
{
    private static nint DefSubclassProc(Message message)
    {
        return Win32.DefSubclassProc(message.hWnd, message.msg, message.wParam, message.lParam);
    }

    nint? IDialogBehavior<DialogEditorViewModel>.OnMessageReceived(Message message, IDialogContext<DialogEditorViewModel> context)
    {
        if (message.msg is not WM_REFLECT) return null;

        var messageCore = *(Message*)message.lParam;
        return messageCore.msg switch
        {
            WM_NCHITTEST => HandleWmNcHitTest(messageCore, context),
            WM_SIZING => HandleWmSizing(messageCore, context),
            WM_MOVING => HandleWmMoving(messageCore, context),
            _ => DefSubclassProc(messageCore)
        };
    }

    // TODO: constrain all movement/sizing to the DLU grid
    private nint HandleWmMoving(Message message, IDialogContext<DialogEditorViewModel> context)
    {
        return DefSubclassProc(message);
    }
    private nint HandleWmSizing(Message message, IDialogContext<DialogEditorViewModel> context)
    {
        return DefSubclassProc(message);
    }

    // So my gut was right - if we return custom values from a child window's WM_NCHITTEST, Windows will implement drag-drop resizing/moving for us!
    private nint HandleWmNcHitTest(Message message, IDialogContext<DialogEditorViewModel> context)
    {
        // ChatGPT and my tests show that Windows probably doesn't send WM_NCHITTEST for arbitrary points on the screen, but just in case.
        var defaultResult = (uint)Win32.DefSubclassProc(message.hWnd, message.msg, message.wParam, message.lParam);
        if(defaultResult is HTNOWHERE)
            return (nint)defaultResult;

        var dluHelper = new DLUHelper(context.DialogBoxHandle);

        Win32.GetWindowRect(message.hWnd, out var controlRect);
        var controlPoint = new Win32.POINT { x = controlRect.left, y = controlRect.top };
        var controlPointOld = controlPoint;
        Win32.ScreenToClient(context.DialogBoxHandle, ref controlPoint);

        controlRect.left += controlPoint.x - controlPointOld.x;
        controlRect.top += controlPoint.y - controlPointOld.y;
        controlRect.right += controlPoint.x - controlPointOld.x;
        controlRect.bottom += controlPoint.y - controlPointOld.y;

        var controlRectDlu = new Win32.RECT
        {
            left = (int)dluHelper.PixelsToDLUHorz(controlRect.left),
            top = (int)dluHelper.PixelsToDLUVert(controlRect.top),
            right = (int)dluHelper.PixelsToDLUHorz(controlRect.right),
            bottom = (int)dluHelper.PixelsToDLUVert(controlRect.bottom)
        };

        var lParam = new DWord((nuint)message.lParam);
        var hitPoint = new Win32.POINT { x = lParam.XParam, y = lParam.YParam };
        Win32.ScreenToClient(context.DialogBoxHandle, ref hitPoint);
        var hitXDlu = (int)dluHelper.PixelsToDLUHorz(hitPoint.x);
        var hitYDlu = (int)dluHelper.PixelsToDLUVert(hitPoint.y);

        int sizingDirectionX = 0;
        if(hitXDlu - controlRectDlu.left <= context.ViewModel.SizingMargin.Width) sizingDirectionX = -1;
        else if(controlRectDlu.right - hitXDlu <= context.ViewModel.SizingMargin.Width) sizingDirectionX = 1;
        int sizingDirectionY = 0;
        if(hitYDlu - controlRectDlu.top <= context.ViewModel.SizingMargin.Height) sizingDirectionY = -1;
        else if(controlRectDlu.bottom - hitYDlu <= context.ViewModel.SizingMargin.Height) sizingDirectionY = 1;

        return (sizingDirectionX, sizingDirectionY) switch
        {
            (0, 0) => (nint)HTCAPTION,
            (-1, -1) => (nint)HTTOPLEFT,
            (0, -1) => (nint)HTTOP,
            (1, -1) => (nint)HTTOPRIGHT,
            (-1, 0) => (nint)HTLEFT,
            (1, 0) => (nint)HTRIGHT,
            (-1, 1) => (nint)HTBOTTOMLEFT,
            (0, 1) => (nint)HTBOTTOM,
            (1, 1) => (nint)HTBOTTOMRIGHT,
            _ => throw new UnreachableException()
        };
    }
}
