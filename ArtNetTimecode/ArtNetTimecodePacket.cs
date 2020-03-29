using System;
using System.Net;
using System.Runtime.InteropServices;
//https://artisticlicence.com/WebSiteMaster/User%20Guides/art-net.pdf

namespace ArtNetTimecode
{
    public enum Types : byte
    {
        FPS24 = 0,
        FPS25 = 1,
        FPS2997 = 2,
        FPS30 = 3
    }

    public enum OpCode : ushort
    {
        OpPoll = 0x2000,
        OpPollReply = 0x2100,
        OpDiagData = 0x2300,
        OpCommand = 0x2400,
        OpOutput = 0x5000,
        OpNzs = 0x5100,
        OpSync = 0x5200,
        OpAddress = 0x6000,
        OpInput = 0x7000,
        OpTodRequest = 0x8000,
        OpTodData = 0x8100,
        OpTodControl = 0x8200,
        OpRdm = 0x8300,
        OpRdmSub = 0x8400,
        OpVideoSetup = 0xa010,
        OpVideoPalette = 0xa020,
        OpVideoData = 0xa040,
        OpMacMaster = 0xf000,
        OpMacSlave = 0xf100,
        OpFirmwareMaster = 0xf200,
        OpFirmwareReply = 0xf300,
        OpFileTnMaster = 0xf400,
        OpFileFnMaster = 0xf500,
        OpFileFnReply = 0xf600,
        OpIpProg = 0xf800,
        OpIpProgReply = 0xf900,
        OpMedia = 0x9000,
        OpMediaPatch = 0x9100,
        OpMediaControl = 0x9200,
        OpMediaContrlReply = 0x9300,
        OpTimeCode = 0x9700,
        OpTrigger = 0x9900,
        OpDirectory = 0x9a00,
        OpDirectoryReply = 0x9b00
    }

    [Flags]
    public enum ReplyStatus : byte
    {
        ubeaPresent             = 0b00000001,
        rdmPresent              = 0b00000010,
        bootFromRom             = 0b00000100,
        //                      = 0b00001000, // not used
        addrFromFrontPanel      = 0b00010000,
        addrFromNetwork         = 0b00100000,
        indLocate               = 0b01000000,
        indMute                 = 0b10000000,
        indNormal               = 0b11000000  // this is probably a bug, since this is set as a flag
    }

    [Flags]
    public enum TalkToMe : byte
    { 
        AutoInform = 0x01,
        SendDiagnostics = 0x02,
        UnicastDiagnostics = 0x04,
        DisableVLC = 0x08,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ArtNetStub
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string id;
        [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
        public OpCode opcode;
        [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
        public ushort version;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ArtNetTimecodePacket
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string id;
        [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
        public OpCode opcode;
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

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ArtNetPollPacket
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string id;
        [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
        public OpCode opcode;
        [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
        public ushort version;
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public TalkToMe talkToMe;
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte priority;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ArtNetPollReplyPacket
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string id;
        [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
        public OpCode opcode;
        [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
        public byte[] address;
        [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
        public ushort port;
        [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
        public ushort version;
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte netSwitch;
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte subSwitch;
        [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
        public ushort oem;
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public byte ubeaVersion;
        [MarshalAs(UnmanagedType.U1, SizeConst = 1)]
        public ReplyStatus status;
        [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
        public ushort estaMan;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 18)]
        public string shortName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string longName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string report;
        [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
        public ushort ports;
        [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
        public byte[] portTypes;
        [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
        public byte[] goodInput;
        [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
        public byte[] goodOutput;
        //[MarshalAs(UnmanagedType.U4, SizeConst = 4)]
        //public byte[] goodOutput;

    }

}
