using Common.Data.Args;
using Common.Interop;
using Common.Interop.InteropValues;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Common.Hooks
{
    public class KeyboardHook
    {
        public static event EventHandler<KeyboardHookEventArgs> KeyDown;

        public static event EventHandler<KeyboardHookEventArgs> KeyUp;

        private static IntPtr HookId = IntPtr.Zero;

        private static readonly HookProc Proc = HookCallback;

        private static int VirtualKey;

        private static readonly IntPtr KeyDownIntPtr = (IntPtr)VALUES.WM_KEYDOWN;

        private static readonly IntPtr KeyUpIntPtr = (IntPtr)VALUES.WM_KEYUP;

        private static readonly IntPtr SyskeyDownIntPtr = (IntPtr)VALUES.WM_SYSKEYDOWN;

        private static readonly IntPtr SyskeyUpIntPtr = (IntPtr)VALUES.WM_SYSKEYUP;

        private static int Count;

        public static void Start()
        {
            if (HookId == IntPtr.Zero)
            {
                HookId = SetHook(Proc);
            }

            if (HookId != IntPtr.Zero)
            {
                Count++;
            }
        }

        public static void Stop()
        {
            Count--;
            if (Count < 1)
            {
                InteropMethods.UnhookWindowsHookEx(HookId);
                HookId = IntPtr.Zero;
            }
        }

        private static IntPtr SetHook(HookProc proc)
        {
            using (var curProcess = Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                if (curModule != null)
                {
                    return InteropMethods.SetWindowsHookEx((int)HOOKTYPE.WH_KEYBOARD_LL, proc,
                        InteropMethods.GetModuleHandle(curModule.ModuleName), 0);
                }
                return IntPtr.Zero;

            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                if (wParam == KeyDownIntPtr)
                {
                    var virtualKey = Marshal.ReadInt32(lParam);
                    if (VirtualKey != virtualKey)
                    {
                        VirtualKey = virtualKey;
                        KeyDown?.Invoke(null, new KeyboardHookEventArgs(virtualKey, false));
                    }
                }
                else if (wParam == SyskeyDownIntPtr)
                {
                    var virtualKey = Marshal.ReadInt32(lParam);
                    if (VirtualKey != virtualKey)
                    {
                        VirtualKey = virtualKey;
                        KeyDown?.Invoke(null, new KeyboardHookEventArgs(virtualKey, true));
                    }
                }
                else if (wParam == KeyUpIntPtr)
                {
                    var virtualKey = Marshal.ReadInt32(lParam);
                    VirtualKey = -1;
                    KeyUp?.Invoke(null, new KeyboardHookEventArgs(virtualKey, false));
                }
                else if (wParam == SyskeyUpIntPtr)
                {
                    var virtualKey = Marshal.ReadInt32(lParam);
                    VirtualKey = -1;
                    KeyUp?.Invoke(null, new KeyboardHookEventArgs(virtualKey, true));
                }
            }
            return InteropMethods.CallNextHookEx(HookId, nCode, wParam, lParam);
        }
    }
}
