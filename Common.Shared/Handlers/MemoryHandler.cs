using System;
using System.Collections.Generic;
using System.Text;
using Common.Interop;

namespace Common.Handlers
{
    public class MemoryHandler
    {
        public static void FlushMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                InteropMethods.SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }
    }
}
