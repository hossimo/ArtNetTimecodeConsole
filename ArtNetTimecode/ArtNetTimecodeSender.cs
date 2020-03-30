using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;

namespace ArtNetTimecode
{
    public class ArtNetTimecodeSender : ArtnetIO
    {
        public IPAddress sendToAddress;
        public ConcurrentQueue<DateTime> queue = new ConcurrentQueue<DateTime>();

        private Types type;
        float frames;
         public float Frames{
            get
            {
                return frames;
            }

            private set
            {
                frames = value;
            }
        }

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
        static int size = Marshal.SizeOf(typeof(ArtNetTimecodePacket));
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
            this.type = type;
            frames = type switch
            {
                Types.FPS24 => 24,
                Types.FPS25 => 25,
                Types.FPS2997 => 29.97f,
                Types.FPS30 => 30,
                _ => 30,
            };
        }

        public void ThreadProc()
        {
            UdpClient udpClient = new UdpClient()
            {
                EnableBroadcast = true
            };

            IPEndPoint endPoint = new IPEndPoint(sendToAddress, ARTNET_PORT);
            Console.WriteLine($"Sender Thread Started to {endPoint} as {type}");


            while (running)
            {
                DateTime time = DateTime.Now;
                queue.Enqueue(time);
                packet.hours = (byte)time.Hour;
                packet.minutes = (byte)time.Minute;
                packet.seconds = (byte)time.Second;
                packet.frames = (byte)(time.Millisecond * 0.001 * frames);

                Marshal.StructureToPtr(packet, ptr, true);
                Marshal.Copy(ptr, sendBuffer, 0, size);

                udpClient.Send(sendBuffer, size, endPoint);

                Thread.Sleep((int)(1000/frames));
            }
            Console.WriteLine("\nExitting Sender");
            Marshal.FreeHGlobal(ptr);
        }
    }
}
