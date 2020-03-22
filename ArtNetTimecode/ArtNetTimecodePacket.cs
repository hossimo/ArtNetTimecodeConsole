using System;
using System.Runtime.InteropServices;

namespace ArtNetTimecode
{


    public enum Types : byte
    {
        FPS24 = 0,
        FPS25 = 1,
        FPS2997 = 2,
        FPS30 = 3
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ArtNetTimecodePacket
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string id;
        [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
        public ushort opcode;
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte versionHi;
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte versionLo;
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte filter1;
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte filter2;
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte frames;
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte seconds;
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte minutes;
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte hours;
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public Types type;
    }
}
