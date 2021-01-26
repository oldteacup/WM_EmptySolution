using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Common.Data.Args
{
    public class KeyboardHookEventArgs : EventArgs
    {
        public bool IsSystemKey { get; }

        public Key Key { get; }

        public KeyboardHookEventArgs(int virtualKey, bool isSystemKey)
        {
            IsSystemKey = isSystemKey;
            Key = KeyInterop.KeyFromVirtualKey(virtualKey);
        }
    }
}
