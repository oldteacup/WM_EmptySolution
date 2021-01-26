using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Common.Interop.InteropValues.WindowBlur
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct WINDOWCOMPOSITIONATTRIBUTEDATA
    {
        public WINDOWCOMPOSITIONATTRIBUTE Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }
}
