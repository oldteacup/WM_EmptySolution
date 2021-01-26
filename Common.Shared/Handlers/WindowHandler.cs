using Common.Interop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace Common.Handlers
{
    public static class WindowHandler
    {
        public static IntPtr CreateHandle() => new WindowInteropHelper(new Window()).EnsureHandle();

        public static IntPtr GetHandle(this Window window) => new WindowInteropHelper(window).EnsureHandle();

        public static HwndSource GetHwndSource(this Window window) => HwndSource.FromHwnd(window.GetHandle());

        /// <summary>
        /// 让窗口激活作为前台最上层窗口
        /// </summary>
        /// <param name="window"></param>
        public static void SetWindowToForegroundWithAttachThreadInput(Window window)
        {
            // [WPF 让窗口激活作为前台最上层窗口的方法 - lindexi - 博客园](https://www.cnblogs.com/lindexi/p/12749671.html)
            var interopHelper = new WindowInteropHelper(window);
            var thisWindowThreadId = InteropMethods.GetWindowThreadProcessId(interopHelper.Handle, out _);
            var currentForegroundWindow = InteropMethods.GetForegroundWindow();
            var currentForegroundWindowThreadId = InteropMethods.GetWindowThreadProcessId(currentForegroundWindow, out _);

            // [c# - Bring a window to the front in WPF - Stack Overflow](https://stackoverflow.com/questions/257587/bring-a-window-to-the-front-in-wpf )
            // [SetForegroundWindow的正确用法 - 子坞 - 博客园](https://www.cnblogs.com/ziwuge/archive/2012/01/06/2315342.html )
            /*
               　　1.得到窗口句柄FindWindow 
            　　　　2.切换键盘输入焦点AttachThreadInput 
            　　　　3.显示窗口ShowWindow(有些窗口被最小化/隐藏了) 
            　　　　4.更改窗口的Z Order，SetWindowPos使之最上，为了不影响后续窗口的Z Order,改完之后，再还原 
            　　　　5.最后SetForegroundWindow 
             */

            InteropMethods.AttachThreadInput(currentForegroundWindowThreadId, thisWindowThreadId, true);

            window.Show();
            window.Activate();
            // 去掉和其他线程的输入链接
            InteropMethods.AttachThreadInput(currentForegroundWindowThreadId, thisWindowThreadId, false);

            // 用于踢掉其他的在上层的窗口
            if (window.Topmost != true)
            {
                window.Topmost = true;
                window.Topmost = false;
            }
        }
    }
}
