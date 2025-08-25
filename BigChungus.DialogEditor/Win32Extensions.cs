public static class Win32Extensions
{
    extension(Win32)
    {
        public static bool ScreenToClient(nint hWnd, ref Win32.RECT rect)
        {
            var point = new Win32.POINT { x = rect.left, y = rect.top };
            var pointOld = point;
            if (!Win32.ScreenToClient(hWnd, ref point)) return false;
            var offsetX = point.x - pointOld.x;
            var offsetY = point.y - pointOld.y;

            rect.left += offsetX;
            rect.top += offsetY;
            rect.right += offsetX;
            rect.bottom += offsetY;
            return true;
        }

        public static bool ClientToScreen(nint hWnd, ref Win32.RECT rect)
        {
            var point = new Win32.POINT { x = rect.left, y = rect.top };
            var pointOld = point;
            if (!Win32.ClientToScreen(hWnd, ref point)) return false;
            var offsetX = point.x - pointOld.x;
            var offsetY = point.y - pointOld.y;

            rect.left += offsetX;
            rect.top += offsetY;
            rect.right += offsetX;
            rect.bottom += offsetY;
            return true;
        }
    }
}
