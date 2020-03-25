using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace ArtNetTimecode
{
    public class ArtnetReceiver
    {
        static bool running;
        const int ARTNET_PORT = 6454;

        public static void StopThread()
        {
            running = false;
        }

        public static void ThreadProc()
        {
            UdpClient udpClient = new UdpClient(ARTNET_PORT);
            IPEndPoint remote = new IPEndPoint(IPAddress.Any, ARTNET_PORT);

            Console.WriteLine($"Listning on port {ARTNET_PORT}");

            running = true;
            while (running)
            {
                Byte[] receivedBytes = udpClient.Receive(ref remote);
                
                // ignore my addresses
                List<string> myAddresses = Tools.GetLocalIPAddress();
                if (myAddresses.Contains(remote.Address.ToString()))
                {
                    continue;
                }

                GCHandle handle = GCHandle.Alloc(receivedBytes, GCHandleType.Pinned);
                ArtNetTimecodePacket stubPacket;
                try
                {
                    stubPacket = (ArtNetTimecodePacket)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(ArtNetTimecodePacket));
                }
                finally
                {
                    handle.Free();
                }
                Console.WriteLine($"Got 0x{stubPacket.opcode:X}");

                Thread.Sleep(0);
            }
            Console.WriteLine("Exitting .");
        }

    }
}
