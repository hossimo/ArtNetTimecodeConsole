using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace ArtNetTimecode
{
    class Tools
    {
        public static List<string> GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            List<string> result = new List<string>();
            foreach (var item in host.AddressList)
            {
                result.Add(item.ToString());
            }
            return result;
        }

        public static T ByteArrayToStructure<T>(byte[] bytes) where T: struct
        {
            T s;
            GCHandle h = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                s = (T)Marshal.PtrToStructure<T>(h.AddrOfPinnedObject());
            }
            finally
            {
                h.Free();
            }
            return s;
        }


        // C Externals
        [DllImport("kernel32.dll", EntryPoint = "SetConsoleMode")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
        [DllImport("kernel32.dll", EntryPoint = "GetConsoleMode")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);
        [DllImport("kernel32.dll", EntryPoint = "GetStdHandle")]
        static extern IntPtr GetStdHandle(int nStdHandle);
        [Flags]
        private enum ConsoleInputModes : uint
        {
            ENABLE_PROCESSED_INPUT = 0x0001,
            ENABLE_LINE_INPUT = 0x0002,
            ENABLE_ECHO_INPUT = 0x0004,
            ENABLE_WINDOW_INPUT = 0x0008,
            ENABLE_MOUSE_INPUT = 0x0010,
            ENABLE_INSERT_MODE = 0x0020,
            ENABLE_QUICK_EDIT_MODE = 0x0040,
            ENABLE_EXTENDED_FLAGS = 0x0080,
            ENABLE_AUTO_POSITION = 0x0100
        }

        [Flags]
        private enum ConsoleOutputModes : uint
        {
            ENABLE_PROCESSED_OUTPUT = 0x0001,
            ENABLE_WRAP_AT_EOL_OUTPUT = 0x0002,
            ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004,
            DISABLE_NEWLINE_AUTO_RETURN = 0x0008,
            ENABLE_LVB_GRID_WORLDWIDE = 0x0010
        }

        const int STD_INPUT_HANDLE = -10;
        const int STD_OUTPUT_HANDLE = -11;
        const int STD_ERROR_HANDLE = -12;

        static uint originalMode = 0xFFFF;
        static public void DisableQuickSelect()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return;

            IntPtr stdin = GetStdHandle(STD_INPUT_HANDLE);
            GetConsoleMode(stdin, out originalMode);
            ConsoleInputModes iam = (ConsoleInputModes)originalMode;
            if ((originalMode & (uint) ConsoleInputModes.ENABLE_QUICK_EDIT_MODE) != 0)
            {
                uint mode = originalMode ^ (uint)ConsoleInputModes.ENABLE_QUICK_EDIT_MODE;
                iam = (ConsoleInputModes)mode;
                SetConsoleMode(stdin, mode);
            }
        }

        static public void RestoreConsoleMode()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return;

            if (originalMode != 0xFFFF)
            {
                IntPtr stdin = GetStdHandle(STD_INPUT_HANDLE);
                SetConsoleMode(stdin, originalMode);
            }
        }

    }
}
