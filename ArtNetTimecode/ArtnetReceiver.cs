using System;
using System.Threading;
namespace ArtNetTimecode
{
    public class ArtnetReceiver
    {
        static bool running;

        public static void StopThread()
        {
            running = false;
        }

        public static void ThreadProc()
        {
            Console.WriteLine("Starting");
            running = true;
            while (running)
            {
                Thread.Sleep(0);
            }
            Console.WriteLine("Exitting .");
        }

    }
}
