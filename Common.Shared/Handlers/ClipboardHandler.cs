using Common.Interop;
using Common.Interop.InteropValues;
using System;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Interop;

namespace Common.Handlers
{
    public class ClipboardHandler
    {
        public static ClipboardHandler Instance { get; private set; } = new Lazy<ClipboardHandler>(() => new ClipboardHandler()).Value;

        private ClipboardHandler() { }

        public event Action ContentChanged;

        private HwndSource HWndSource;

        private IntPtr HookId = IntPtr.Zero;

        private int Count;

        public void Start()
        {
            if (HookId == IntPtr.Zero)
            {
                HookId = WindowHandler.CreateHandle();
                HWndSource = HwndSource.FromHwnd(HookId);
                if (HWndSource != null)
                {
                    HWndSource.AddHook(WinProc);
                    InteropMethods.AddClipboardFormatListener(HookId);
                }
            }

            if (HookId != IntPtr.Zero)
            {
                Count++;
            }
        }

        public void Stop()
        {
            Count--;
            if (Count < 1)
            {
                HWndSource.RemoveHook(WinProc);
                InteropMethods.RemoveClipboardFormatListener(HookId);

                HookId = IntPtr.Zero;
            }
        }


        //public void PasteToTargetWinByPtr(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        public void PasteToTargetWinByPtr(IntPtr hWnd)
        {
#if false
            // Send Paste message to target window
            InteropMethods.SendMessage(hWnd, VALUES.WM_PASTE, 0, 0);
#else 
            // Send Virtual Keys ( Ctrl + V ) to target window
            InteropMethods.SendMessage(hWnd, VALUES.WM_KEYDOWN, VIRTUAL_KEYS.VK_CONTROL, 0);
            InteropMethods.SendMessage(hWnd, VALUES.WM_KEYDOWN, VIRTUAL_KEYS.VK_V, 0);
            System.Threading.Thread.Sleep(10);
            InteropMethods.SendMessage(hWnd, VALUES.WM_KEYDOWN, VIRTUAL_KEYS.VK_CONTROL, 1);
            InteropMethods.SendMessage(hWnd, VALUES.WM_KEYDOWN, VIRTUAL_KEYS.VK_V, 1);
#endif
        }

        private IntPtr WinProc(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            if (msg == VALUES.WM_CLIPBOARDUPDATE)
            {
                ContentChanged?.Invoke();
            }
            return IntPtr.Zero;
        }

    }
}
