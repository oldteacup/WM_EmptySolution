using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Interop.InteropValues
{
    public static class VALUES
    {
        internal const int
            BITSPIXEL = 12,
            PLANES = 14,
            BI_RGB = 0,
            DIB_RGB_COLORS = 0,
            E_FAIL = unchecked((int)0x80004005),
            NIF_MESSAGE = 0x00000001,
            NIF_ICON = 0x00000002,
            NIF_TIP = 0x00000004,
            NIF_INFO = 0x00000010,
            NIM_ADD = 0x00000000,
            NIM_MODIFY = 0x00000001,
            NIM_DELETE = 0x00000002,
            NIIF_NONE = 0x00000000,
            NIIF_INFO = 0x00000001,
            NIIF_WARNING = 0x00000002,
            NIIF_ERROR = 0x00000003,
            WM_ACTIVATE = 0x0006,
            WM_QUIT = 0x0012,
            WM_GETMINMAXINFO = 0x0024,
            WM_WINDOWPOSCHANGING = 0x0046,
            WM_WINDOWPOSCHANGED = 0x0047,
            WM_SETICON = 0x0080,
            WM_NCCREATE = 0x0081,
            WM_NCDESTROY = 0x0082,
            WM_NCACTIVATE = 0x0086,
            WM_NCRBUTTONDOWN = 0x00A4,
            WM_NCRBUTTONUP = 0x00A5,
            WM_NCRBUTTONDBLCLK = 0x00A6,
            WM_NCUAHDRAWCAPTION = 0x00AE,
            WM_NCUAHDRAWFRAME = 0x00AF,
            WM_KEYDOWN = 0x0100,
            WM_KEYUP = 0x0101,
            WM_SYSKEYDOWN = 0x0104,
            WM_SYSKEYUP = 0x0105,
            WM_SYSCOMMAND = 0x112,
            WM_MOUSEMOVE = 0x0200,
            WM_LBUTTONUP = 0x0202,
            WM_LBUTTONDBLCLK = 0x0203,
            WM_RBUTTONUP = 0x0205,
            WM_PASTE = 0x0302,
            WM_CLIPBOARDUPDATE = 0x031D,
            WM_USER = 0x0400,
            WS_VISIBLE = 0x10000000,
            MF_BYCOMMAND = 0x00000000,
            MF_BYPOSITION = 0x400,
            MF_GRAYED = 0x00000001,
            MF_SEPARATOR = 0x800,
            TB_GETBUTTON = WM_USER + 23,
            TB_BUTTONCOUNT = WM_USER + 24,
            TB_GETITEMRECT = WM_USER + 29,
            VERTRES = 10,
            DESKTOPVERTRES = 117,
            LOGPIXELSX = 88,
            LOGPIXELSY = 90,
            SC_CLOSE = 0xF060,
            SC_SIZE = 0xF000,
            SC_MOVE = 0xF010,
            SC_MINIMIZE = 0xF020,
            SC_MAXIMIZE = 0xF030,
            SC_RESTORE = 0xF120,
            SRCCOPY = 0x00CC0020,
            MONITOR_DEFAULTTONEAREST = 0x00000002;
    }


    internal delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);

    internal delegate IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

}
