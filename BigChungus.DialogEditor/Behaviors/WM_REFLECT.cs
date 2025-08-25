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
            WM_ENTERSIZEMOVE => HandleWmEnterSizeMove(messageCore, context),
            WM_EXITSIZEMOVE => HandleWmExitSizeMode(messageCore, context),
            WM_MOVING => HandleWmMoving(messageCore, context),
            WM_NCRBUTTONUP => HandleNcRButtonUp(messageCore, context),
            _ => DefSubclassProc(messageCore)
        };
    }

    private nint HandleNcRButtonUp(Message messageCore, IDialogContext<DialogEditorViewModel> context)
    {
        var viewModel = new SimpleMenuViewModel("Delete", "Nevermind");
        var view = new SimpleMenuView();
        view.ShowDialog(viewModel, messageCore.hWnd);
        if(viewModel.SelectedIndex == 0)
        {
            Win32.DestroyWindow(messageCore.hWnd);
        }
        return DefSubclassProc(messageCore);
    }

    // So, bummer - while WM_SIZING aggregates the cursor position relative to where the modal loop started, and always tries to move the window there,
    // WM_MOVING aggregates nothing and simply adds last-frame cursor movement to whatever was specified in lParam in the last message.
    // So, when we apply WM_SIZING logic, the window hardly moves at all, since it's rare for the cursor to jump by 4 DLUs in a single frame.
    // Instead, we'll handle WM_ENTERSIZEMOVE, capture the cursor position, and aggregate the cursor offset ourselves.
    private nint HandleWmMoving(Message message, IDialogContext<DialogEditorViewModel> context)
    {
        var dluHelper = new DLUHelper(context.DialogBoxHandle);

        Win32.GetCursorPos(out var cursorPos);
        Win32.ScreenToClient(message.hWnd, ref cursorPos);

        var deltaX = cursorPos.x - context.ViewModel.SizingLoopStartingCursorPositionRelativeToControl.x;
        var deltaY = cursorPos.y - context.ViewModel.SizingLoopStartingCursorPositionRelativeToControl.y;

        Win32.GetWindowRect(message.hWnd, out var bounds);
        bounds.left += deltaX;
        bounds.top += deltaY;
        bounds.right += deltaX;
        bounds.bottom += deltaY;

        //Win32.ClientToScreen(message.hWnd, ref bounds);
        Win32.ScreenToClient(context.DialogBoxHandle, ref bounds);
        var boundsDlu = dluHelper.PixelsToDLU(bounds);

        boundsDlu.left = Math.Clamp(
            (int)Math.Round(boundsDlu.left / (double)context.ViewModel.DLUGridSize.Width) * context.ViewModel.DLUGridSize.Width,
            0,
            context.ViewModel.Size.Width - context.ViewModel.SizingLoopStartingSize.Width
        );
        boundsDlu.top = Math.Clamp(
            (int)Math.Round(boundsDlu.top / (double)context.ViewModel.DLUGridSize.Height) * context.ViewModel.DLUGridSize.Height,
            0,
            context.ViewModel.Size.Height - context.ViewModel.SizingLoopStartingSize.Height
        );
        boundsDlu.right = boundsDlu.left + context.ViewModel.SizingLoopStartingSize.Width;
        boundsDlu.bottom = boundsDlu.top + context.ViewModel.SizingLoopStartingSize.Height;

        bounds = dluHelper.DLUToPixels(boundsDlu);
        Win32.ClientToScreen(context.DialogBoxHandle, ref bounds);
        *(Win32.RECT*)message.lParam = bounds;

        return DefSubclassProc(message);
    }

    private nint HandleWmEnterSizeMove(Message messageCore, IDialogContext<DialogEditorViewModel> context)
    {
        Win32.GetCursorPos(out var cursorPos);
        Win32.ScreenToClient(messageCore.hWnd, ref cursorPos);
        context.ViewModel.SizingLoopStartingCursorPositionRelativeToControl = cursorPos;

        Win32.GetWindowRect(messageCore.hWnd, out var bounds);
        Win32.ScreenToClient(context.DialogBoxHandle, ref bounds);
        var boundsDlu = new DLUHelper(context.DialogBoxHandle).PixelsToDLU(bounds);
        context.ViewModel.SizingLoopStartingSize = new((short)(boundsDlu.right - boundsDlu.left), (short)(boundsDlu.bottom - boundsDlu.top));
        // Without this size, DLU grid rounding may change the control's DLU size during WM_MOVING.

        return DefSubclassProc(messageCore);
    }

    private nint HandleWmExitSizeMode(Message messageCore, IDialogContext<DialogEditorViewModel> context)
    {
        context.ViewModel.SizingLoopStartingCursorPositionRelativeToControl = default;
        return DefSubclassProc(messageCore);
    }

    private nint HandleWmSizing(Message message, IDialogContext<DialogEditorViewModel> context)
    {
        var dluHelper = new DLUHelper(context.DialogBoxHandle);

        var newBounds = (Win32.RECT*)message.lParam;
        Win32.ScreenToClient(context.DialogBoxHandle, ref *newBounds);
        var newBoundsDlu = dluHelper.PixelsToDLU(*newBounds);
        newBoundsDlu.left = (int)Math.Round(newBoundsDlu.left / (double)context.ViewModel.DLUGridSize.Width) * context.ViewModel.DLUGridSize.Width;
        newBoundsDlu.top = (int)Math.Round(newBoundsDlu.top / (double)context.ViewModel.DLUGridSize.Height) * context.ViewModel.DLUGridSize.Height;
        newBoundsDlu.right = (int)Math.Round(newBoundsDlu.right / (double)context.ViewModel.DLUGridSize.Width) * context.ViewModel.DLUGridSize.Width;
        newBoundsDlu.bottom = (int)Math.Round(newBoundsDlu.bottom / (double)context.ViewModel.DLUGridSize.Height) * context.ViewModel.DLUGridSize.Height;

        if(newBoundsDlu.left >= newBoundsDlu.right)
        {
            var sizingDirection = message.wParam;
            if(sizingDirection is WMSZ_TOP or WMSZ_TOPLEFT or WMSZ_BOTTOMLEFT)
                newBoundsDlu.left = newBoundsDlu.right - context.ViewModel.DLUGridSize.Width;
            else if(sizingDirection is WMSZ_RIGHT or WMSZ_TOPRIGHT or WMSZ_BOTTOMRIGHT)
                newBoundsDlu.right = newBoundsDlu.left + context.ViewModel.DLUGridSize.Width;
        }

        if(newBoundsDlu.top >= newBoundsDlu.bottom)
        {
            var sizingDirection = message.wParam;
            if(sizingDirection is WMSZ_TOP or WMSZ_TOPLEFT or WMSZ_TOPRIGHT)
                newBoundsDlu.top = newBoundsDlu.bottom - context.ViewModel.DLUGridSize.Height;
            else if(sizingDirection is WMSZ_BOTTOM or WMSZ_BOTTOMLEFT or WMSZ_BOTTOMRIGHT)
                newBoundsDlu.bottom = newBoundsDlu.top + context.ViewModel.DLUGridSize.Height;
        }

        *newBounds = dluHelper.DLUToPixels(newBoundsDlu);
        Win32.ClientToScreen(context.DialogBoxHandle, ref *newBounds);

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
        Win32.ScreenToClient(context.DialogBoxHandle, ref controlRect);
        var controlRectDlu = dluHelper.PixelsToDLU(controlRect);

        var lParam = new DWord((nuint)message.lParam);
        var hitPoint = new Win32.POINT { x = lParam.XParam, y = lParam.YParam };
        Win32.ScreenToClient(context.DialogBoxHandle, ref hitPoint);
        var hitPointDlu = dluHelper.PixelsToDLU(hitPoint);

        int sizingDirectionX = 0;
        if(hitPointDlu.x - controlRectDlu.left <= context.ViewModel.SizingMargin.Width) sizingDirectionX = -1;
        else if(controlRectDlu.right - hitPointDlu.x <= context.ViewModel.SizingMargin.Width) sizingDirectionX = 1;
        int sizingDirectionY = 0;
        if(hitPointDlu.y - controlRectDlu.top <= context.ViewModel.SizingMargin.Height) sizingDirectionY = -1;
        else if(controlRectDlu.bottom - hitPointDlu.y <= context.ViewModel.SizingMargin.Height) sizingDirectionY = 1;

        return (sizingDirectionX, sizingDirectionY) switch
        {
            (1, 1) => (nint)HTBOTTOMRIGHT,
            (1, -1) => (nint)HTTOPRIGHT,
            (-1, -1) => (nint)HTTOPLEFT,
            (-1, 1) => (nint)HTBOTTOMLEFT,

            (0, 1) => (nint)HTBOTTOM,
            (1, 0) => (nint)HTRIGHT,
            (0, -1) => (nint)HTTOP,
            (-1, 0) => (nint)HTLEFT,
            
            (0, 0) => (nint)HTCAPTION,
            _ => throw new UnreachableException()
        };
    }
}
