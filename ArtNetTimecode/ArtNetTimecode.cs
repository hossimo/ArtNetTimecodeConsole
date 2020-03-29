using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;


namespace ArtNetTimecode
{
    class ArtNetTimecode
    {
        static bool run;
        static void Main(string[] args)
        {
            run = true;

            // Start Receiver Thread
            Thread receiverThread = new Thread(new ThreadStart(ArtnetReceiver.ThreadProc))
            {
                Name = "Art-Net Receiver"
            };
            receiverThread.Start();

            // catch control - C
            Console.CancelKeyPress += new ConsoleCancelEventHandler(CancelHandler);

            IPAddress serverAddress;
            if (args.Length == 1) {
                if (args[0] == "help" || args[0] == "-h" || args[0] == "--help")
                {
                    Console.WriteLine($"ArtnetTimecode [-h] [ip]");
                    Console.WriteLine($"-h\t help");
                    Console.WriteLine($"ip\t destination address in the form of x.x.x.x");
                    return;
                }
                if (!IPAddress.TryParse(args[0], out serverAddress))
                {
                    Console.WriteLine($"ArtnetTimecode [-h] [ip]");
                    Console.WriteLine($"-h\t help");
                    Console.WriteLine($"ip\t destination address in the form of x.x.x.x");
                    return;
                }
                Console.WriteLine($"Unicasting Artnet-TimeCode to {args[0]}, Ctrl + C to exit");
            }
            else {
                Console.WriteLine($"Broadcasting Artnet-TimeCode, Ctrl + C to exit");
                serverAddress = IPAddress.Broadcast;
            }


            ArtNetTimecodePacket packet = new ArtNetTimecodePacket
            {
                id = "Art-Net",
                opcode = (OpCode) 0x9700,
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

            byte[] sendBuffer = new byte[19];
            int size = 19;
            IntPtr ptr = Marshal.AllocHGlobal(size);

            UdpClient c = new UdpClient();
            IPEndPoint endPoint = new IPEndPoint(serverAddress, 6454);


            while (run)
            {
                int currentLine = Console.CursorTop;
                DateTime localTime = DateTime.Now;
                packet.hours = (byte)localTime.Hour;
                packet.minutes = (byte)localTime.Minute;
                packet.seconds = (byte)localTime.Second;
                packet.frames = (byte) (localTime.Millisecond * 0.001 * 30);

                Marshal.StructureToPtr(packet, ptr, true);
                Marshal.Copy(ptr, sendBuffer, 0, size);

                c.Send(sendBuffer, size, endPoint);
                Console.SetCursorPosition(0, currentLine);
                Console.CursorVisible = false;
                Console.Write($"Sent: {packet.hours:00}:{packet.minutes:00}:{packet.seconds:00}:{packet.frames:00}");
                Thread.Sleep(1000/60);
            }
            Console.WriteLine("\nExitting");
            //receiverThread.Abort
            Marshal.FreeHGlobal(ptr);
            Console.CursorVisible = true;
            ArtnetReceiver.StopThread();
            receiverThread.Join();
            return;
        }

        protected static void CancelHandler(object sender, ConsoleCancelEventArgs args)
        {
            args.Cancel = true;
            run = false;
        }

    }
}
