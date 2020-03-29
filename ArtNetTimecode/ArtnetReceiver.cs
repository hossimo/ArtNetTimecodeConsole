using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace ArtNetTimecode
{
    public static class ArtnetReceiver
    {
        static bool running = true;
        const int ARTNET_PORT = 6454;


        public static void StopThread()
        {
            running = false;
        }

        public static void ThreadProc()
        {
            UdpClient udpClient = new UdpClient(ARTNET_PORT)
            {
                EnableBroadcast = true
            };
            IPEndPoint remote = new IPEndPoint(IPAddress.Any, ARTNET_PORT);

            Console.WriteLine($"Listning on port {ARTNET_PORT}");
            Console.WriteLine();

            while (running)
            {
                // Grab a list of my addresses
                List<string> myAddresses = Tools.GetLocalIPAddress();

                // grab a packet
                if (udpClient.Available == 0)
                    continue;

                byte[] receivedBytes = udpClient.Receive(ref remote);


                // check to see if I sent it
                if (myAddresses.Contains(remote.Address.ToString()))
                {
                    continue;
                }

                // test for valid packet
                ArtNetStub stubPacket = Tools.ByteArrayToStructure<ArtNetStub>(receivedBytes);
                // Art-Net
                if (!stubPacket.id.Contains("Art-Net"))
                {
                    continue;
                }

                // test the version
                if (stubPacket.version < 14)
                {
                    continue;
                }

                // test the opcode
                Console.WriteLine($" Got 0x{stubPacket.opcode:X} from{remote.Address.ToString()}");

                switch (stubPacket.opcode)
                {
                    case OpCode.OpPoll:
                        ProcessOpPoll(receivedBytes, remote);
                        break;
                    case OpCode.OpPollReply:
                        break;
                    case OpCode.OpDiagData:
                        break;
                    case OpCode.OpCommand:
                        break;
                    case OpCode.OpOutput:
                        break;
                    case OpCode.OpNzs:
                        break;
                    case OpCode.OpSync:
                        break;
                    case OpCode.OpAddress:
                        break;
                    case OpCode.OpInput:
                        break;
                    case OpCode.OpTodRequest:
                        break;
                    case OpCode.OpTodData:
                        break;
                    case OpCode.OpTodControl:
                        break;
                    case OpCode.OpRdm:
                        break;
                    case OpCode.OpRdmSub:
                        break;
                    case OpCode.OpVideoSetup:
                        break;
                    case OpCode.OpVideoPalette:
                        break;
                    case OpCode.OpVideoData:
                        break;
                    case OpCode.OpMacMaster:
                        break;
                    case OpCode.OpMacSlave:
                        break;
                    case OpCode.OpFirmwareMaster:
                        break;
                    case OpCode.OpFirmwareReply:
                        break;
                    case OpCode.OpFileTnMaster:
                        break;
                    case OpCode.OpFileFnMaster:
                        break;
                    case OpCode.OpFileFnReply:
                        break;
                    case OpCode.OpIpProg:
                        break;
                    case OpCode.OpIpProgReply:
                        break;
                    case OpCode.OpMedia:
                        break;
                    case OpCode.OpMediaPatch:
                        break;
                    case OpCode.OpMediaControl:
                        break;
                    case OpCode.OpMediaContrlReply:
                        break;
                    case OpCode.OpTimeCode:
                        break;
                    case OpCode.OpTrigger:
                        break;
                    case OpCode.OpDirectory:
                        break;
                    case OpCode.OpDirectoryReply:
                        break;
                    default:
                        break;
                }

                Thread.Sleep(0);
            }
            Console.WriteLine("Exitting Thread {0}", Thread.CurrentThread.Name);
        }

        public static void ProcessOpPoll(byte[] bytes, IPEndPoint remote)
        {
            ArtNetPollPacket packet = Tools.ByteArrayToStructure<ArtNetPollPacket>(bytes);

            // need to keep a table of remotes and their diagnostics requests
            bool send = false;
            IPAddress sendTo = IPAddress.Any;

            switch (packet.talkToMe)
            {
                case TalkToMe.AutoInform:
                    // schedule sending of diagnostics
                case TalkToMe.SendDiagnostics:
                    send = true;
                    goto case TalkToMe.UnicastDiagnostics;
                case TalkToMe.UnicastDiagnostics:
                    sendTo = remote.Address;
                    goto case TalkToMe.DisableVLC;
                case TalkToMe.DisableVLC:
                default:
                    break;
            }

            if (send == true)
            {
                // build up artpol reply.
            }
        }
    }
}
