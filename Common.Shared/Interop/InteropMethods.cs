using Common.Interop.InteropValues.WindowBlur;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Common.Interop
{
    internal class InteropMethods
    {
        internal static class ExternDll
        {
            public const string Activeds = "activeds.dll";
            public const string Fxassert = "Fxassert.dll";
            public const string Vsassert = "vsassert.dll";
            public const string Version = "version.dll";
            public const string Wtsapi32 = "wtsapi32.dll";
            public const string Winspool = "winspool.drv";
            public const string WinMM = "winmm.dll";
            public const string Uxtheme = "uxtheme.dll";
            public const string User32 = "user32.dll";
            public const string Shell32 = "shell32.dll";
            public const string Psapi = "psapi.dll";
            public const string Powrprof = "Powrprof.dll";
            public const string PerfCounter = "perfcounter.dll";
            public const string Olepro32 = "olepro32.dll";
            public const string Oleaut32 = "oleaut32.dll";
            public const string Shlwapi = "shlwapi.dll";
            public const string Oleacc = "oleacc.dll";
            public const string Ntdll = "ntdll.dll";
            public const string Mqrt = "mqrt.dll";
            public const string Msi = "msi.dll";
            public const string Clr = "clr.dll";
            public const string Mscoree = "mscoree.dll";
            public const string Loadperf = "Loadperf.dll";
            public const string Kernel32 = "kernel32.dll";
            public const string Imm32 = "imm32.dll";
            public const string Hhctrl = "hhctrl.ocx";
            public const string Gdiplus = "gdiplus.dll";
            public const string Gdi32 = "gdi32.dll";
            public const string Comdlg32 = "comdlg32.dll";
            public const string Comctl32 = "comctl32.dll";
            public const string Advapi32 = "advapi32.dll";
            public const string Ole32 = "ole32.dll";
            public const string Crypt32 = "crypt32.dll";
        }

        #region Common Methods


        [DllImport(ExternDll.Kernel32)]
        internal static extern bool SetProcessWorkingSetSize(IntPtr proc, int min, int max);


        [DllImport(ExternDll.User32)]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WINDOWCOMPOSITIONATTRIBUTEDATA data);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        internal static extern bool GetCursorPos(out InteropValues.POINT pt);


        [DllImport(ExternDll.User32, CharSet = CharSet.Auto, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int flags);

        [DllImport(ExternDll.User32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport(ExternDll.User32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool RemoveClipboardFormatListener(IntPtr hwnd);


        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        internal static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);


        [DllImport(ExternDll.User32, SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport(ExternDll.User32, SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport(ExternDll.User32, SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr GetActiveWindow();

        [DllImport(ExternDll.User32, SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport(ExternDll.User32, SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport(ExternDll.User32, SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr GetFocus();

        [DllImport(ExternDll.User32, SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool AttachThreadInput(in uint currentForegroundWindowThreadId, in uint thisWindowThreadId, bool isAttach);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr SetWindowsHookEx(int idHook, InteropValues.HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport(ExternDll.Kernel32, CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr GetModuleHandle(string lpModuleName);


        internal static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 4)
            {
                return SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
            }
            return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
        }

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto, EntryPoint = "SetWindowLong")]
        internal static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto, EntryPoint = "SetWindowLongPtr")]
        internal static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport(ExternDll.User32, CharSet = CharSet.Unicode)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport(ExternDll.Gdi32, SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool DeleteObject(IntPtr hObject);
        #endregion
    }
}
