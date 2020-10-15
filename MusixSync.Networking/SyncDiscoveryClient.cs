using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MusixSync.Networking.Models;

namespace MusixSync.Networking
{
    public class SyncDiscoveryClient
    {
        public static byte[] InitSequence = { 0xff, 0x23, 0xd3, 0x21, 0x46, 0xdf, 0xfd, 0xff };
        public const int SERVER_PORT = 2198;

        public delegate void HostDiscoveredArgs(SyncDevice host);

        public event HostDiscoveredArgs HostDiscovered;

        public UdpClient DiscoveryClient;
        public List<SyncDevice> Hosts = new List<SyncDevice>();
        public bool IsSearching { get; protected set; }

        public void StartSearch()
        {
            if (!IsSearching)
            {
                IsSearching = true;
                Hosts = new List<SyncDevice>();
                DiscoveryClient = new UdpClient()
                {
                    EnableBroadcast = true
                };
                DiscoveryClient.Client.Bind(new IPEndPoint(IPAddress.Any, SERVER_PORT));
                new Thread(Discovery).Start();
            }
        }

        public void StopSearch()
        {
            if (IsSearching)
            {
                IsSearching = false;
                DiscoveryClient.Dispose();
                DiscoveryClient = null;
            }
        }

        private void Discovery()
        {
            while (IsSearching)
            {
                try
                {
                    IPEndPoint HostAddress = new IPEndPoint(0, 0);
                    byte[] Buffer = DiscoveryClient.Receive(ref HostAddress);

                    bool IsInitSequence = true;

                    if (Buffer.Length != InitSequence.Length)
                    {
                        IsInitSequence = false;
                    }
                    else
                    {
                        for (int i = 0; i < Buffer.Length; i++)
                        {
                            if (Buffer[i] != InitSequence[i])
                            {
                                IsInitSequence = false;
                                break;
                            }
                        }
                    }
                    if (IsInitSequence)
                    {
                        if (Hosts.Where(x => x.IP == HostAddress.ToString()).Count() == 0)
                        {
                            SyncDevice newHost = new SyncDevice()
                            {
                                IP = HostAddress.ToString(),
                                Host = Dns.GetHostEntry(HostAddress.Address).HostName,
                                Address = HostAddress.Address
                            };
                            Hosts.Add(newHost);
                            HostDiscovered?.Invoke(newHost);
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        public bool AskSyncWithHost(SyncDevice host, out SyncClient client)
        {
            client = null;
            Console.WriteLine("Asking to sync");
            if (IsSearching)
            {
                Console.WriteLine("Search Stopped");
                StopSearch();
            }
            try
            {
                Console.WriteLine("create client");
                TcpClient tcpClient = new TcpClient();
                Console.WriteLine($"try connect . {host.Address}:{SERVER_PORT}");
                tcpClient.Connect(new IPEndPoint(host.Address, SERVER_PORT));
                Console.WriteLine("pass");
                if (tcpClient.Connected)
                {
                    client = new SyncClient(tcpClient);
                    Console.WriteLine("connected");
                    return true;
                }
                else
                {
                    Console.WriteLine("pass fail");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("failed to connect");
                return false;
            }
        }
    }
}