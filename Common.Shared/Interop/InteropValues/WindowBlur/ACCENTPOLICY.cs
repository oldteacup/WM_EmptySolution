using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Common.Interop.InteropValues.WindowBlur
{

    [StructLayout(LayoutKind.Sequential)]
    internal struct ACCENTPOLICY
    {
        public ACCENTSTATE AccentState;
        public int AccentFlags;
        public int GradientColor;
        public int AnimationId;
    }
}
