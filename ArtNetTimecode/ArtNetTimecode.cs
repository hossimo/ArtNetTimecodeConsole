using System;
using System.Net;
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
            ArtnetReceiver receiver = new ArtnetReceiver();
            Thread receiverThread = new Thread(new ThreadStart(receiver.ThreadProc))
            {
                Name = "Art-Net Receiver"
            };

            receiverThread.Start();

            // disable quick select
            Tools.DisableQuickSelect();

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

            // Start Sender Thread
            ArtNetTimecodeSender sender = new ArtNetTimecodeSender(serverAddress, Types.FPS30);
            Thread senderThread = new Thread(new ThreadStart(sender.ThreadProc))
            {
                Name = "Aet-Net Sender"
            };
            senderThread.Start();



            while (run)
            {
                DateTime t = sender.LastSent();
                if (t != null)
                {
                    int currentLine = Console.CursorTop;
                    Console.WriteLine($"> {t.Hour:00}:{t.Minute:00}:{t.Second:00}:{t.Millisecond / sender.Frames:00} @ {sender.Frames}");
                    Console.SetCursorPosition(0, currentLine);
                    Console.CursorVisible = false;
                }
                //Thread.Sleep(100);
                //if (sender.queue != null && sender.queue.TryDequeue(out t))
                //{
                //    int currentLine = Console.CursorTop;
                //    Console.WriteLine($"> {t.Hour:00}:{t.Minute:00}:{t.Second:00}:{t.Millisecond / sender.Frames:00} @ {sender.Frames}");
                //    Console.SetCursorPosition(0, currentLine);
                //    Console.CursorVisible = false;
                //}
            }
            Console.CursorVisible = true;
            receiver.StopThread();
            receiverThread.Join();
            sender.StopThread();
            senderThread.Join();
            Tools.RestoreConsoleMode();
            return;
        }

        protected static void CancelHandler(object sender, ConsoleCancelEventArgs args)
        {
            args.Cancel = true;
            run = false;
        }

    }
}
