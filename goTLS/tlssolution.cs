using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace golang
{
    class tlsSolution
    {
        public class methodChain
        {
            public List<string> headers = new List<string>();
            public IDictionary<string, string> cookies = new Dictionary<string, string>();
            public methodChain collectCookies(string response)
            {
                dynamic json = JsonConvert.DeserializeObject(response);
                if (json.cookies != null)
                {
                    foreach (string cookie in json.cookies)
                    {
                        var keyVal = cookie.Split(new[] { '=' }, 2);
                        cookies[keyVal[0]] = Uri.UnescapeDataString(keyVal[1]);
                    }
                }
                return this;
            }
            public static string headerOrder = "";

            public methodChain AddHeader(string headerKey, string headerValue)
            {
                headers.Add(headerKey + ",," + headerValue);
                return this;
            }
        }

        private static byte[] getBytes(string value)
            => System.Text.Encoding.UTF8.GetBytes(value);

        static class goTLS
        {
            [DllImport("tlsSolution.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr getRequest(byte[] urlRaw, byte[] headersRaw, byte[] ipAddress);

            [DllImport("tlsSolution.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr headRequest(byte[] urlRaw, byte[] headersRaw, byte[] ipAddress);

            [DllImport("tlsSolution.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr postRequest(byte[] urlRaw, byte[] headersRaw, byte[] body, byte[] ipAddress);

            [DllImport("tlsSolution.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr putRequest(byte[] urlRaw, byte[] headersRaw, byte[] ipAddress);
        }

        public string getRequest(string urlRaw, List<string> headers, string ipAddress = null)
            => Marshal.PtrToStringAnsi(goTLS.getRequest(getBytes(urlRaw), getBytes(string.Join("=|=|=", headers)), getBytes(proxyHandler(ipAddress))));

        public string putRequest(string urlRaw, List<string> headers, string ipAddress = null)
            => Marshal.PtrToStringAnsi(goTLS.putRequest(getBytes(urlRaw), getBytes(string.Join("=|=|=", headers)), getBytes(proxyHandler(ipAddress))));

        public string headRequest(string urlRaw, List<string> headers, string ipAddress = null)
            => Marshal.PtrToStringAnsi(goTLS.headRequest(getBytes(urlRaw), getBytes(string.Join("=|=|=", headers)), getBytes(proxyHandler(ipAddress))));


        public static string proxyHandler(string ipAddress)
        {
            string ip = "";
            string[] splitIp = ipAddress.Split(':');

            if (splitIp.Length == 2)
                ip = $"http://{splitIp[0]}:{splitIp[1]}";
            else
                ip = $"http://{splitIp[2]}:{splitIp[3]}@{splitIp[0]}:{splitIp[1]}";

            return ip;
        }

        public string postRequest(string urlRaw, List<string> headers, string body, string ipAddress = null)
            => Marshal.PtrToStringAnsi(goTLS.postRequest(getBytes(urlRaw), getBytes(string.Join("=|=|=", headers)), getBytes(body), getBytes(proxyHandler(ipAddress))));
    }
}
