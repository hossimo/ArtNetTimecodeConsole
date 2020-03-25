using System;
using System.Collections.Generic;
using System.Net;
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
    }
}
