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
    }
}
