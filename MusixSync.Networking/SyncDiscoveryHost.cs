using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MusixSync.Networking.Models;

namespace MusixSync.Networking
{
    public class SyncDiscoveryHost
    {
        public static TcpListener Listener;
        public const int SERVER_PORT = 2198;
        private bool Active = false;
        public List<SyncHost> ClientsWaiting;
        private NetScanner Scanner = new NetScanner();
        public static readonly byte[] InitSequence = { 0xff, 0x23, 0xd3, 0x21, 0x46, 0xdf, 0xfd, 0xff };
        private UdpClient DiscoveryClient;

        public delegate void OnClientConnectArgs(SyncHost client);

        public event OnClientConnectArgs OnClientConnect;

        private bool _IsDiscoveryActive = false;

        public string MusicCollectionPath;

        private bool IsExiting = false;

        public void DisposeClient()
        {
            IsExiting = true;
        }

        ~SyncDiscoveryHost()
        {
            IsExiting = true;
        }

        public bool IsDiscoverable
        {
            get
            {
                return _IsDiscoveryActive;
            }
            set
            {
                if (value != _IsDiscoveryActive)
                {
                    if (value)
                    {
                        TriggerDiscover();
                    }
                    else
                    {
                        StopDiscover();
                    }
                }
            }
        }


        private void TriggerDiscover()
        {
            DiscoveryClient = new UdpClient()
            {
                EnableBroadcast = true
            };
            _IsDiscoveryActive = true;
            Thread disc = new Thread(MakeDiscover);
            disc.Start();
        }

        private void StopDiscover()
        {
            _IsDiscoveryActive = false;
        }

        private void MakeDiscover()
        {
            TriggerNetScan();
            while (_IsDiscoveryActive && !IsExiting)
            {
                DiscoveryClient.Send(InitSequence, InitSequence.Length, "255.255.255.255", SERVER_PORT);
                Thread.Sleep(1000);
            }
        }

        private void TriggerNetScan()
        {
            Scanner.ScanNetwork(Scanner.GetLocalIPPrefixes(), HostScanComplete, OnHostDiscovered, true);
        }

        private void OnHostDiscovered(string IP, string Hostname)
        {
            if (_IsDiscoveryActive)
            {
                DiscoveryClient.Send(InitSequence, InitSequence.Length, IP, SERVER_PORT);
            }
        }

        private void HostScanComplete(List<NetScanner.PingResult> Results)
        {
            if (_IsDiscoveryActive)
            {
                GC.Collect();
                Thread.Sleep(2000);

                for (int i = 0; i < 15; i++)
                {
                    if (!_IsDiscoveryActive)
                    {
                        return;
                    }
                    if (IsExiting) return;
                    foreach (NetScanner.PingResult host in Results)
                    {
                        DiscoveryClient.Send(InitSequence, InitSequence.Length, host.IP, SERVER_PORT);
                    }

                    Thread.Sleep(1000);
                }

                if (_IsDiscoveryActive)
                {
                    TriggerNetScan();
                }
            }
        }

        public void Start()
        {
            if (!Active)
            {
                ClientsWaiting = new List<SyncHost>();
                System.Console.WriteLine("Listener started");
                Listener = new TcpListener(new IPEndPoint(IPAddress.Any, SERVER_PORT));
                Listener.Start();
                Active = true;
                new Thread(Listen).Start();
            }
        }

        public void Stop()
        {
            if (Active)
            {
                Active = false;
                foreach (SyncHost client in ClientsWaiting)
                {
                    client.ProceedSync(false);
                }

                Listener.Stop();
            }
        }

        private void Listen()
        {
            while (Active)
            {
                try
                {
                    System.Console.WriteLine("Waiting for client...");
                    TcpClient newClient = Listener.AcceptTcpClient();
                    System.Console.WriteLine("client connected");
                    SyncHost SClient = new SyncHost(newClient);
                    lock (ClientsWaiting)
                    {
                        ClientsWaiting.Add(SClient);
                        new Thread(() => OnClientConnect?.Invoke(SClient)).Start();
                    }
                }
                catch (Exception)
                {
                    Thread.Sleep(1000);
                }
            }
        }

        public void SyncWithClient(SyncHost client)
        {
            IsDiscoverable = false;
            if (ClientsWaiting.Contains(client))
            {
                ClientsWaiting.Remove(client);
            }
            foreach (SyncHost syncHost in ClientsWaiting)
            {
                syncHost.ProceedSync(false);
            }
            Stop();
        }
    }
}