using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MusixSync.Networking.Models
{
    public class NetScanner
    {
        public delegate void HostDiscoveredCallback(string IP, string Hostname);

        public delegate void NetworkScanCompletedCallback(List<PingResult> Results);

        public string[] GetLocalIPs()
        {
            return Dns.GetHostEntry(Dns.GetHostName()).AddressList.Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).Select(x => x.ToString()).ToArray();
        }

        public string[] GetLocalIPPrefixes()
        {
            List<string> prefixes = new List<string>();
            foreach (string LocalIP in GetLocalIPs())
            {
                prefixes.Add(LocalIP.Remove(LocalIP.LastIndexOf('.') + 1));
            }
            return prefixes.ToArray();
        }

        public void ScanNetwork(string[] IPPrefixes, NetworkScanCompletedCallback completedCallback, HostDiscoveredCallback callback = null, bool IgnoreLocal = false)
        {
            PingCallbackProvider prov = new PingCallbackProvider(callback, completedCallback);
            prov.IgnoreLocal = IgnoreLocal;
            if (IgnoreLocal)
            {
                prov.Locals = GetLocalIPs();
            }
            foreach (string IPPrefix in IPPrefixes)
            {
                for (int i = 1; i < 255; i++)
                {
                    lock (prov.CallbackRef)
                    {
                        prov.CallbackRef.Max++;
                    }
                    string ip = IPPrefix + i.ToString();
                    Ping p = new Ping();
                    p.PingCompleted += new PingCompletedEventHandler(prov.PingCompleted);
                    p.SendAsync(ip, 100, ip);
                }
                lock (prov.CallbackRef)
                {
                    prov.CallbackRef.AllSent = true;
                }
            }
        }

        internal class PingCallbackCountRef
        {
            public int Max = 0;
            public int Completed = 0;

            public int CallbacksRequired = 0;
            public int CallbacksSent = 0;
            public bool AllSent = false;
            public bool FinalizeReady = false;
        }

        public class PingResult
        {
            public PingResult(string ip, string host)
            {
                IP = ip;
                Host = host;
            }

            public string IP;
            public string Host;
        }

        internal class PingCallbackProvider
        {
            private HostDiscoveredCallback Callback;
            private NetworkScanCompletedCallback CompletedCallback;
            public List<PingResult> hosts = new List<PingResult>();
            public PingCallbackCountRef CallbackRef = new PingCallbackCountRef();
            public bool IgnoreLocal = false;
            public string[] Locals = { };

            public PingCallbackProvider(HostDiscoveredCallback callback, NetworkScanCompletedCallback completedCallback)
            {
                Callback = callback;
                CompletedCallback = completedCallback;
            }

            public void PingCompleted(object sender, PingCompletedEventArgs e)
            {
                bool Finalize = false;
                lock (CallbackRef)
                {
                    CallbackRef.Completed++;
                    if (CallbackRef.AllSent && CallbackRef.Completed >= CallbackRef.Max)
                    {
                        if (CallbackRef.CallbacksSent >= CallbackRef.CallbacksRequired)
                        {
                            Finalize = true;
                        }
                        else
                        {
                            CallbackRef.FinalizeReady = true;
                        }
                    }
                }
                if (e.Reply != null && e.Reply.Status == IPStatus.Success)
                {
                    string ip = (string)e.UserState;
                    if (IgnoreLocal)
                    {
                        lock (Locals)
                        {
                            if (Locals.Contains(ip))
                            {
                                return;
                            }
                        }
                    }

                    lock (CallbackRef)
                    {
                        CallbackRef.CallbacksRequired++;
                    }
                    string name;
                    try
                    {
                        IPHostEntry hostEntry = Dns.GetHostEntry(ip);
                        name = hostEntry.HostName;
                    }
                    catch (SocketException)
                    {
                        name = "?";
                    }
                    hosts.Add(new PingResult(ip, name));
                    Callback?.Invoke(ip, name);
                    lock (CallbackRef)
                    {
                        CallbackRef.CallbacksSent++;
                        if (CallbackRef.FinalizeReady && CallbackRef.CallbacksSent >= CallbackRef.CallbacksRequired)
                        {
                            CompletedCallback?.Invoke(hosts);
                        }
                    }
                }
                if (Finalize)
                {
                    CompletedCallback?.Invoke(hosts);
                }
            }
        }
    }
}
