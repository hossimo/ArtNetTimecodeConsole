using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
//using System.Collections.Concurrent;

namespace ArtNetTimecode
{
    public class ArtNetTimecodeSender : ArtnetIO
    {
        public IPAddress sendToAddress;
        //public ConcurrentQueue<DateTime> queue = new ConcurrentQueue<DateTime>();
        private DateTime _lastSent; // I think this if safe since there is a single reader and writer
        private Types _type;

        public float Frames { get; private set; }

        // Initalize ArtNetTimecode Packet
        ArtNetTimecodePacket packet = new ArtNetTimecodePacket
        {
            id = "Art-Net",
            opcode = (OpCode)0x9700,
            versionHi = 14,
            versionLo = 14,
            filter1 = 0,
            filter2 = 0,
            hours = 0,
            minutes = 0,
            seconds = 0,
            frames = 0,
            type = Types.FPS30
        };

        // Make a byte array for our structure
        static readonly int size = Marshal.SizeOf(typeof(ArtNetTimecodePacket));
        byte[] sendBuffer = new byte[size];
        IntPtr ptr = Marshal.AllocHGlobal(size);

        // Constructor
        public ArtNetTimecodeSender(IPAddress toAddress, Types packetType)
        {
            sendToAddress = toAddress;
            SetFrames(packetType);
        }

        public void SetFrames (Types type)
        {
            packet.type = type;
            this._type = type;
            Frames = type switch
            {
                Types.FPS24 => 24,
                Types.FPS25 => 25,
                Types.FPS2997 => 29.97f,
                Types.FPS30 => 30,
                _ => 30,
            };
        }

        public DateTime LastSent()
        {
            return _lastSent;
        }

        public void ThreadProc()
        {
            UdpClient udpClient = new UdpClient()
            {
                EnableBroadcast = true
            };

            IPEndPoint endPoint = new IPEndPoint(sendToAddress, ARTNET_PORT);
            Console.WriteLine($"Sender Thread Started to {endPoint} as {_type}");


            while (running)
            {
                DateTime time = DateTime.Now;
                //queue.Enqueue(time);
                _lastSent = time;
                packet.hours = (byte)time.Hour;
                packet.minutes = (byte)time.Minute;
                packet.seconds = (byte)time.Second;
                packet.frames = (byte)(time.Millisecond * 0.001 * Frames);

                Marshal.StructureToPtr(packet, ptr, true);
                Marshal.Copy(ptr, sendBuffer, 0, size);

                udpClient.Send(sendBuffer, size, endPoint);

                Thread.Sleep((int)(1000/Frames));
            }
            Console.WriteLine("\nExitting Sender");
            Marshal.FreeHGlobal(ptr);
        }
    }
}
