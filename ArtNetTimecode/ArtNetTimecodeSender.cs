using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace ArtNetTimecode
{
    public static class ArtNetTimecodeSender
    {
        static bool running = true;
        const int ARTNET_PORT = 6454;
    }
}
