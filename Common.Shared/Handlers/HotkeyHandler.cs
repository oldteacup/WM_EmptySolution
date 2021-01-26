using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Collections;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace Common.Handlers
{
    internal class HotKeyParameter
    {
        public int Control { get; set; } = 0;
        public Keys Vk { get; set; } = Keys.F1;

        public bool IsEnable { get; set; } = true;

        public static HotKeyParameter ToObject(string str)
        {
            return JsonConvert.DeserializeObject<HotKeyParameter>(str);
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(new
            {
                Control,
                Vk,
                IsEnable
            });
        }
    }

    public partial class HotKeyHandler
    {
        public System.Windows.Input.KeyEventArgs PreviewKeyDownEventHandler(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Delete || e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl
                || e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt || e.SystemKey == Key.LeftShift || e.SystemKey == Key.RightShift
                || e.SystemKey == Key.LWin || e.SystemKey == Key.RWin)
            {
                return null;
            }
            else if (Regex.IsMatch(e.Key.ToString(), "^([A-Z]|F[0-9]{1,2})$"))
            {
                return e;
            }
            return null;
        }


        public bool RegistHotKey(Window window, ref string hotkeyString, System.Windows.Input.KeyEventArgs e, Action callBack)
        {
            string str = string.Empty;
            Instance.UnRegistAll(window);
            var hkp = string.IsNullOrEmpty(hotkeyString) ? new HotKeyParameter() : HotKeyParameter.ToObject(hotkeyString);
            hkp.Control = (int)e.KeyboardDevice.Modifiers;
            hkp.Vk = (Keys)KeyInterop.VirtualKeyFromKey(e.SystemKey);
            if (hkp.Vk == Keys.None)
            {
                hkp.Vk = (Keys)KeyInterop.VirtualKeyFromKey(e.Key);
            }
            if (hkp.Vk != Keys.None && hotkeyString != hkp.ToString())
            {
                hotkeyString = hkp.ToString();
                return Instance.Regist(window, (int)e.KeyboardDevice.Modifiers, hkp.Vk, callBack);
            }
            return false;
        }
    }


    /// <summary>
    /// 直接构造类实例即可注册
    /// 自动完成注销
    /// 注意注册时会抛出异常
    /// </summary>
    public partial class HotKeyHandler
    //注册系统热键类
    //热键会随着程序结束自动解除,不会写入注册表
    {

        //引入系统API
        [DllImport("user32.dll")]
        static extern bool RegisterHotKey(IntPtr hWnd, int id, int modifiers, Keys vk);
        [DllImport("user32.dll")]
        static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        public static HotKeyHandler Instance { get; set; } = new HotKeyHandler();

        private List<HwndSource> _sourceList { get; set; } = new List<HwndSource>();


        Dictionary<int, Action> keymap = new Dictionary<int, Action>();   //每一个key对于一个处理函数

        //组合控制键
        public enum HotkeyModifiers
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            Win = 8
        }

        //注册快捷键
        public bool Regist(Window window, int modifiers, Keys vk, Action callBack)
        {
            IntPtr hWnd = new WindowInteropHelper(window).EnsureHandle();
            int id = modifiers + (int)vk * 10;
            if (!RegisterHotKey(hWnd, id, (int)modifiers, vk))
            {
                return false;
            }
            keymap[id] = callBack;

            //绑定钩子事件

            var source = _sourceList.FirstOrDefault(item => item.Handle == hWnd);
            if (source == null)
            {
                source = PresentationSource.FromVisual(window) as HwndSource;
                source.AddHook(WndProc);
                _sourceList.Add(source);
            }
            else
            {
                source.AddHook(WndProc);
            }
            return true;
        }

        // 注销指定快捷键
        public void UnRegistByKey(IntPtr hWnd, int keyid)
        {
            if (keymap.ContainsKey(keyid))
            {
                UnregisterHotKey(hWnd, keyid);
                keymap.Remove(keyid * 10);
            }
        }

        // 注销所有快捷键
        public void UnRegistAll(IntPtr hWnd)
        {
            foreach (var km in keymap)
            {
                UnregisterHotKey(hWnd, km.Key);
            }
        }
        public void UnRegistAll(Window window)
        {
            IntPtr hWnd = new WindowInteropHelper(window).Handle;
            foreach (var km in keymap)
            {
                UnregisterHotKey(hWnd, km.Key);
            }
        }

        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handle)
        {
            //Debug.WriteLine("hwnd:{0},msg:{1},wParam:{2},lParam{3}:,handle:{4}"
            //                ,hwnd,msg,wParam,lParam,handle);
            try
            {
                int id = wParam.ToInt32();
                if (keymap.ContainsKey(id))
                {
                    if (keymap.TryGetValue(id, out Action callback))
                    {
                        callback?.Invoke();
                    }
                }
                return IntPtr.Zero;
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

    }
}
