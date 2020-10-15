using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MusixSync.Networking.Enums;
using MusixSync.Networking.Interfaces;
using MusixSync.Networking.Models;

namespace MusixSync.Networking
{
    public class SyncClient
    {
        public const byte SKEY = 0xfd;
        public const byte SKEY_ACCEPTED = 0xfc;
        public const byte SKEY_DENIED = 0xfb;
        public const byte SKEY_EXISTS_STARTSEND = 0xfc;
        public const byte SKEY_DOES_NOT_EXIST = 0xfb;
        public const byte SKEY_START = 0xfc;
        public const byte SKEY_END = 0xfb;
        public const byte SKEY_ACK = 0xfd;
        public const ushort RH_BUFFERSIZE = 12;
        public const ushort FILETRANSFER_BUFFERSIZE = 1024 * 5;

        private TcpClient Conn;
        private WebSocket Socket;

        public bool ProxyEventsEnabled = true;
        private CancellationTokenSource CancellationTokenSource;

        public IFileWriter FileWriter = new BasicFileWriter("Out");

        public event DownloadsDecidedArgs DownloadsDecided;

        public delegate void DownloadsDecidedArgs(List<string> Downloads);

        public delegate void TransferCompleteArgs(string File, int completed, int intentMax);

        public event TransferCompleteArgs TransferComplete;

        public delegate void TransferStartedArgs(string File, int Completed, int intentMax);

        public event TransferCompleteArgs TransferStarted;

        public delegate void SyncCompleteArgs(int FilesTransferred, int Errors);

        public event SyncCompleteArgs SyncComplete;

        public delegate void SyncAbortedArgs(SyncAbortReason Reason, string Message);

        public event SyncAbortedArgs SyncAborted;


        public delegate void SyncAbortedProxyArgs(SyncAbortReason Reason, string Message, SyncClient client);

        public event SyncAbortedProxyArgs SyncAbortedProxy;

        public IPAddress Address { get; internal set; }

        private string CachedHostname;

        public string HostName
        {
            get
            {
                if (CachedHostname == null)
                {
                    IPHostEntry h = Dns.GetHostEntry(Address);
                    if (h != null)
                    {
                        CachedHostname = h.HostName;
                    }
                    else
                    {
                        CachedHostname = "Unknown Device";
                    }
                }
                return CachedHostname;
            }
        }



        public delegate void SyncStatusArgs(bool Syncing);

        public event SyncStatusArgs OnSyncStatus;

        public bool IsAborted { get; internal set; } = false;

        public SyncClient(TcpClient tcpClient)
        {
            Conn = tcpClient;
            SyncAborted += SyncClient_SyncAborted;
            Address = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address;
            new Thread(async () => await Sync()).Start();
        }


        public void Abort()
        {
            CancellationTokenSource?.Cancel();
        }

        private void SyncClient_SyncAborted(SyncAbortReason Reason, string Message)
        {
            IsAborted = true;
            if (ProxyEventsEnabled)
            {
                SyncAbortedProxy?.Invoke(Reason, Message, this);
            }
        }

        public async Task Sync()
        {
            CancellationTokenSource = new CancellationTokenSource();
            try
            {
                await SyncTask(CancellationTokenSource.Token);
            }
            catch (WebSocketException)
            {
                SyncAborted?.Invoke(SyncAbortReason.PartnerAbortedSync, "Partner aborted the sync");
            }
            catch (Exception ex)
            {
                SyncAborted?.Invoke(SyncAbortReason.Error, ex.Message);
            }
        }

        private async Task SyncTask(CancellationToken CancellationToken)
        {
            Console.WriteLine("[Sync Client] Creating Socket...");
            Socket = WebSocketProtocol.CreateFromStream(Conn.GetStream(), false, "TCP", TimeSpan.FromSeconds(20));

            byte[] StatusBuffer = new byte[2];
            ArraySegment<byte> StatusSegment = new ArraySegment<byte>(StatusBuffer);
            Console.WriteLine("[Sync Client] Waiting for status...");

            var StatusRead = await Socket.ReceiveAsync(StatusSegment, CancellationToken);
            if (CancellationTokenSource.IsCancellationRequested)
            {
                SyncAborted?.Invoke(SyncAbortReason.SyncWasCanceled, "Sync cancelled");
                Socket.Dispose();
                return;
            }
            if (StatusBuffer[0] != SKEY)
            {
                SyncAborted?.Invoke(SyncAbortReason.Error, "Invalid Init Key");
                Socket.Dispose();
                return;
            }
            byte InitCode = StatusBuffer[1];
            if (InitCode == SKEY_DENIED)
            {
                OnSyncStatus?.Invoke(false);
                Socket.Dispose();
                return;
            }
            else if (InitCode != SKEY_ACCEPTED)
            {
                SyncAborted?.Invoke(SyncAbortReason.Error, "Invalid init code.");
                Socket.Dispose();
                return;
            }
            OnSyncStatus?.Invoke(true);
            StringBuilder RequestedFilesBuilder = new StringBuilder();
            using (StringWriter writer = new StringWriter(RequestedFilesBuilder))
            {
                byte[] FilesReadBuffer = new byte[RH_BUFFERSIZE];
                ArraySegment<byte> ReceivingFilesSegment = new ArraySegment<byte>(FilesReadBuffer);
                while (true)
                {
                    var read = await Socket.ReceiveAsync(ReceivingFilesSegment, CancellationToken);
                    if (CancellationTokenSource.IsCancellationRequested)
                    {
                        SyncAborted?.Invoke(SyncAbortReason.SyncWasCanceled, "Sync cancelled");
                        Socket.Dispose();
                        return;
                    }
                    writer.Write(Encoding.UTF8.GetString(FilesReadBuffer, 0, read.Count));
                    if (read.EndOfMessage)
                    {
                        break;
                    }
                }
            }
            string RawServerFiles = RequestedFilesBuilder.ToString();
            List<string> ServerFiles = new List<string>();
            if (RawServerFiles.Length <= 1)
            {
                ArraySegment<byte> AbortSegment = new ArraySegment<byte>(new byte[] { SKEY, SKEY_END });
                await Socket.SendAsync(AbortSegment, WebSocketMessageType.Binary, true, CancellationToken);
                SyncAborted?.Invoke(SyncAbortReason.NoFilesOnServer, "There are no files to sync");
                Socket.Dispose();
                return;
            }
            else
            {
                foreach (string serverfile in RawServerFiles.Split('&'))
                {
                    ServerFiles.Add(WebUtility.UrlDecode(serverfile));
                }
            }
            ServerFiles.RemoveAll(x => string.IsNullOrEmpty(x));
            List<string> RequestFiles = new List<string>();
            foreach (string file in ServerFiles)
            {
                if (!FileWriter.FileExists(file))
                {
                    RequestFiles.Add(file);
                }
            }
            DownloadsDecided?.Invoke(RequestFiles);
            List<string> DownloadQueue = RequestFiles.ToList();
            for (int i = 0; i < RequestFiles.Count; i++)
            {
                byte[] FilesBuffer = Encoding.UTF8.GetBytes(WebUtility.UrlEncode(RequestFiles[i]) + "&");
                ArraySegment<byte> Segment = new ArraySegment<byte>(FilesBuffer);
                await Socket.SendAsync(Segment, WebSocketMessageType.Text, (i == RequestFiles.Count - 1), CancellationToken);
                if (CancellationTokenSource.IsCancellationRequested)
                {
                    SyncAborted?.Invoke(SyncAbortReason.SyncWasCanceled, "Sync cancelled");
                    Socket.Dispose();
                    return;
                }
            }
            int FileTransfers = 0;
            ArraySegment<byte> RequestFilePrefix = new ArraySegment<byte>(new byte[] { SKEY, (byte)HeaderCode.RequestFile });
            foreach (string tRequestfile in RequestFiles)
            {
                byte[] FileNameBytes = Encoding.UTF8.GetBytes(tRequestfile);
                ArraySegment<byte> FileNameSegment = new ArraySegment<byte>(FileNameBytes);
                await Socket.SendAsync(RequestFilePrefix, WebSocketMessageType.Binary, false, CancellationToken);
                await Socket.SendAsync(FileNameSegment, WebSocketMessageType.Binary, true, CancellationToken);
                byte[] FileStatusBuffer = new byte[2];
                ArraySegment<byte> FileStatusSegment = new ArraySegment<byte>(FileStatusBuffer);
                await Socket.ReceiveAsync(FileStatusSegment, CancellationToken);
                if (CancellationTokenSource.IsCancellationRequested)
                {
                    SyncAborted?.Invoke(SyncAbortReason.SyncWasCanceled, "Sync cancelled");
                    Socket.Dispose();
                    return;
                }
                if (CancellationTokenSource.IsCancellationRequested)
                {
                    SyncAborted?.Invoke(SyncAbortReason.SyncWasCanceled, "Sync cancelled");
                    Socket.Dispose();
                    return;
                }
                if (FileStatusBuffer[0] == SKEY)
                {
                    if (FileStatusBuffer[1] != SKEY_EXISTS_STARTSEND)
                    {
                        continue;
                    }
                }
                else
                {
                    SyncAborted?.Invoke(SyncAbortReason.Error, "Invalid SKey on Recieve Files Exists!");
                    Socket.Dispose();
                    return;
                }
                byte[] ReadBuffer = new byte[FILETRANSFER_BUFFERSIZE];
                ArraySegment<byte> ReadBufferSegment = new ArraySegment<byte>(ReadBuffer);
                using (FileStream WriteStream = FileWriter.OpenWrite(tRequestfile))
                {
                    FileTransfers++;
                    TransferStarted?.Invoke(tRequestfile, RequestFiles.Count - DownloadQueue.Count, RequestFiles.Count);
                    while (true)
                    {
                        var read = await Socket.ReceiveAsync(ReadBufferSegment, CancellationToken);
                        if (CancellationTokenSource.IsCancellationRequested)
                        {
                            SyncAborted?.Invoke(SyncAbortReason.SyncWasCanceled, "Sync cancelled");
                            Socket.Dispose();
                            return;
                        }
                        WriteStream.Write(ReadBuffer, 0, read.Count);
                        if (read.EndOfMessage)
                        {
                            break;
                        }
                    }
                    await WriteStream.FlushAsync();
                    DownloadQueue.Remove(tRequestfile);
                    TransferComplete?.Invoke(tRequestfile, RequestFiles.Count - DownloadQueue.Count, RequestFiles.Count);
                }
                ArraySegment<byte> ConfirmFilePrefix = new ArraySegment<byte>(new byte[] { SKEY, (byte)HeaderCode.TranserComplete });
                await Socket.SendAsync(ConfirmFilePrefix, WebSocketMessageType.Binary, false, CancellationToken);
                await Socket.SendAsync(FileNameSegment, WebSocketMessageType.Binary, true, CancellationToken);
            }
            ArraySegment<byte> SyncCompleteSegment = new ArraySegment<byte>(new byte[] { SKEY, (byte)HeaderCode.SyncComplete });
            await Socket.SendAsync(SyncCompleteSegment, WebSocketMessageType.Binary, true, CancellationToken);
            SyncComplete?.Invoke(FileTransfers, 0);
            Socket.Dispose();
            return;
        }
    }
}