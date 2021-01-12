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
    public class SyncHost
    {
        private TcpClient Conn;
        private WebSocket Socket;
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
        private TaskCompletionSource<bool> Allow = new TaskCompletionSource<bool>();
        private CancellationTokenSource CancellationTokenSource;

        public bool EnableProxyEvents = true;

        public event DownloadIntentRecievedArgs DownloadIntentRecieved;

        public delegate void DownloadIntentRecievedArgs(List<string> Downloads);

        public delegate void TransferCompleteArgs(string File, int completed, int intentMax);

        public event TransferCompleteArgs TransferComplete;

        public delegate void TransferStartedArgs(string File, int Completed, int intentMax);

        public event TransferCompleteArgs TransferStarted;

        public delegate void SyncCompleteArgs(int FilesTransferred, int Errors);

        public event SyncCompleteArgs SyncComplete;

        public delegate void SyncAbortedArgs(SyncAbortReason Reason, string Message);

        public event SyncAbortedArgs SyncAborted;

        public delegate void SyncAbortedProxyArgs(SyncAbortReason Reason, string Message, SyncHost host);

        public event SyncAbortedProxyArgs SyncAbortedProxy;

        public IFileProvider FileProvider = new SimpleFileProvider("Source");

        public IPAddress Address { get; internal set; }

        public bool IsAborted { get; internal set; } = false;

        public SyncHost(TcpClient tcpClient)
        {
            Conn = tcpClient;
            SyncAborted += SyncHost_SyncAborted;
            Address = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address;
            new Thread(async () => await Host()).Start();
        }

        private string CachedHostname;

        public string HostName
        {
            get
            {
                if (CachedHostname == null)
                {
                    if (TryGetHostEntry(Address, out IPHostEntry entry))
                    {
                        CachedHostname = entry.HostName;

                    } else
                    {
                        CachedHostname = "Unknown Device";
                    }
                }
                return CachedHostname;
            }
        }

        public bool TryGetHostEntry(IPAddress address, out IPHostEntry entry)
        {
            try
            {
                entry = Dns.GetHostEntry(address);
                return true;
            }
            catch (Exception)
            {
                entry = null;
            }
            return false;
        }

        private void SyncHost_SyncAborted(SyncAbortReason Reason, string Message)
        {
            IsAborted = true;
            if (EnableProxyEvents)
            {
                SyncAbortedProxy?.Invoke(Reason, Message, this);
            }
        }

        //public void Init()
        //{
        //    new Thread(async () => await Host()).Start();
        //}

        public void ProceedSync(bool Sync)
        {
            Console.WriteLine($"[Host] Proceed raise sync: {Sync} ");
            Allow?.TrySetResult(Sync);
        }

        public void CancelSync()
        {
            CancellationTokenSource?.Cancel();
        }

        public async Task Host()
        {
            CancellationTokenSource = new CancellationTokenSource();
            try
            {
                await HostSync(CancellationTokenSource.Token);
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                SyncAborted?.Invoke(SyncAbortReason.PartnerAbortedSync, "Partner aborted the sync");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine($"Error of type {ex.GetType().FullName}");
                SyncAborted?.Invoke(SyncAbortReason.Error, ex.Message);
            }
            Console.WriteLine("[Host] finished");
        }

        private async Task HostSync(CancellationToken CancellationToken)
        {
            Console.WriteLine("[Sync Host] Creating Socket...");

            Socket = WebSocketProtocol.CreateFromStream(Conn.GetStream(), true, "TCP", TimeSpan.FromSeconds(20));
            Console.WriteLine("[Sync Host] Waiting for sync confirmation...");

            bool Sync = await Allow.Task;
            Console.WriteLine($"[Sync Host] Syncing: {Sync}");
            if (!Sync)
            {
                ArraySegment<byte> SyncDenied = new ArraySegment<byte>(new byte[] { SKEY, SKEY_DENIED });
                await Socket.SendAsync(SyncDenied, WebSocketMessageType.Binary, true, CancellationToken);
                if (CancellationTokenSource.IsCancellationRequested)
                {
                    SyncAborted?.Invoke(SyncAbortReason.SyncWasCanceled, "Sync cancelled");
                    Socket.Dispose();
                    return;
                }
                Socket.Dispose();
                SyncAborted?.Invoke(SyncAbortReason.SyncDenied, "Sync was denied");
                return;
            }
            ArraySegment<byte> ListingStartSequence = new ArraySegment<byte>(new byte[] { SKEY, SKEY_ACCEPTED });
            await Socket.SendAsync(ListingStartSequence, WebSocketMessageType.Binary, true, CancellationToken);
            if (CancellationTokenSource.IsCancellationRequested)
            {
                SyncAborted?.Invoke(SyncAbortReason.SyncWasCanceled, "Sync cancelled");
                Socket.Dispose();
                return;
            }
            List<string> Files = FileProvider.GetLocalFiles();
            for (int i = 0; i < Files.Count; i++)
            {
                byte[] FilesBuffer = Encoding.UTF8.GetBytes(WebUtility.UrlEncode(Files[i]) + "&");
                ArraySegment<byte> Segment = new ArraySegment<byte>(FilesBuffer);
                await Socket.SendAsync(Segment, WebSocketMessageType.Text, (i == Files.Count - 1), CancellationToken);
            }
            StringBuilder RequestedFilesBuilder = new StringBuilder();
            using (StringWriter writer = new StringWriter(RequestedFilesBuilder))
            {
                byte[] FilesReadBuffer = new byte[RH_BUFFERSIZE];
                ArraySegment<byte> ReceivingFilesSegment = new ArraySegment<byte>(FilesReadBuffer);
                while (true)
                {
                    var read = await Socket.ReceiveAsync(ReceivingFilesSegment, CancellationToken);
                    writer.Write(Encoding.UTF8.GetString(FilesReadBuffer, 0, read.Count));
                    if (CancellationTokenSource.IsCancellationRequested)
                    {
                        SyncAborted?.Invoke(SyncAbortReason.SyncWasCanceled, "Sync cancelled");
                        Socket.Dispose();
                        return;
                    }
                    if (read.EndOfMessage)
                    {
                        break;
                    }
                }
            }

            string RawRequestedFiles = RequestedFilesBuilder.ToString();
            if (RawRequestedFiles.Length <= 2)
            {
                SyncAborted?.Invoke(SyncAbortReason.NoNeededFiles, "The remote client didn't request any files.");
                Socket.Dispose();
                return;
            }
            List<string> RequestedFiles = new List<string>();
            foreach (string prt in RawRequestedFiles.Split('&'))
            {
                RequestedFiles.Add(WebUtility.UrlDecode(prt));
            }
            RequestedFiles.RemoveAll(x => string.IsNullOrEmpty(x));
            DownloadIntentRecieved?.Invoke(RequestedFiles);
            List<string> DownloadQueue = RequestedFiles.ToList();
            int FilesTransfered = 0;
            byte[] FileRequestBuffer = new byte[RH_BUFFERSIZE];
            if (CancellationTokenSource.IsCancellationRequested)
            {
                SyncAborted?.Invoke(SyncAbortReason.SyncWasCanceled, "Sync cancelled");
                Socket.Dispose();
                return;
            }
            ArraySegment<byte> RequestFilesSegment = new ArraySegment<byte>(FileRequestBuffer);
            while (true)
            {
                using (MemoryStream RequestContent = new MemoryStream())
                {
                    while (true)
                    {
                        var read = await Socket.ReceiveAsync(RequestFilesSegment, CancellationToken);
                        if (CancellationTokenSource.IsCancellationRequested)
                        {
                            SyncAborted?.Invoke(SyncAbortReason.SyncWasCanceled, "Sync cancelled");
                            Socket.Dispose();
                            return;
                        }
                        RequestContent.Write(FileRequestBuffer, 0, read.Count);
                        if (read.EndOfMessage)
                        {
                            break;
                        }
                    }

                    byte[] MessageContentBytes = RequestContent.ToArray();
                    byte[] HeaderBuffer = new byte[2];
                    RequestContent.Position = 0;
                    RequestContent.Read(HeaderBuffer, 0, 2);
                    bool HasHeaders = HeaderBuffer[0] == SKEY;
                    byte[] Content;
                    if (HasHeaders)
                    {
                        int NewLen = (int)MessageContentBytes.Length - 2;
                        Content = new byte[NewLen];
                        Buffer.BlockCopy(MessageContentBytes, 2, Content, 0, MessageContentBytes.Length - 2);
                    }
                    else
                    {
                        SyncAborted?.Invoke(SyncAbortReason.Error, "Message has no SKEY Header.");
                        Socket.Dispose();
                        return;
                    }
                    if (CancellationTokenSource.IsCancellationRequested)
                    {
                        SyncAborted?.Invoke(SyncAbortReason.SyncWasCanceled, "Sync cancelled");
                        Socket.Dispose();
                        return;
                    }
                    HeaderCode header = (HeaderCode)HeaderBuffer[1];
                    if (header == HeaderCode.RequestFile)
                    {
                        string RequestedFile = Encoding.UTF8.GetString(Content);
                        if (FileProvider.FileExists(RequestedFile))
                        {
                            TransferStarted?.Invoke(RequestedFile, RequestedFiles.Count - DownloadQueue.Count, RequestedFiles.Count);
                            ArraySegment<byte> StartTransferSegment = new ArraySegment<byte>(new byte[] { SKEY, SKEY_EXISTS_STARTSEND });
                            await Socket.SendAsync(StartTransferSegment, WebSocketMessageType.Binary, true, CancellationToken);
                            using (FileStream TransferSource = FileProvider.OpenFile(RequestedFile))
                            {
                                byte[] TransferBuffer = new byte[FILETRANSFER_BUFFERSIZE];
                                while (true)
                                {
                                    long remaining = TransferSource.Length - TransferSource.Position;
                                    int blocksize = FILETRANSFER_BUFFERSIZE;
                                    if (FILETRANSFER_BUFFERSIZE > remaining)
                                    {
                                        blocksize = (int)remaining;
                                    }
                                    int Read = TransferSource.Read(TransferBuffer, 0, blocksize);
                                    ArraySegment<byte> TransferSegment = new ArraySegment<byte>(TransferBuffer, 0, Read);
                                    bool EndOfFile = TransferSource.Position >= TransferSource.Length;
                                    await Socket.SendAsync(TransferSegment, WebSocketMessageType.Binary, EndOfFile, CancellationToken);
                                    if (CancellationTokenSource.IsCancellationRequested)
                                    {
                                        SyncAborted?.Invoke(SyncAbortReason.SyncWasCanceled, "Sync cancelled");
                                        Socket.Dispose();
                                        return;
                                    }
                                    if (EndOfFile)
                                    {
                                        break;
                                    }
                                }
                                FilesTransfered++;
                            }
                            TransferStarted?.Invoke(RequestedFile, RequestedFiles.Count - DownloadQueue.Count, RequestedFiles.Count);
                        }
                    }
                    else if (header == HeaderCode.TranserComplete)
                    {
                        if (CancellationTokenSource.IsCancellationRequested)
                        {
                            SyncAborted?.Invoke(SyncAbortReason.SyncWasCanceled, "Sync cancelled");
                            Socket.Dispose();
                            return;
                        }
                        string DownloadedFile = Encoding.UTF8.GetString(Content);
                        if (DownloadQueue.Contains(DownloadedFile))
                        {
                            DownloadQueue.Remove(DownloadedFile);
                            TransferComplete?.Invoke(DownloadedFile, RequestedFiles.Count - DownloadQueue.Count, RequestedFiles.Count);
                        }
                    }
                    else if (header == HeaderCode.SyncComplete)
                    {
                        SyncComplete?.Invoke(FilesTransfered, 0);
                        Socket.Dispose();
                        return;
                    }
                }
            }
        }
    }
}