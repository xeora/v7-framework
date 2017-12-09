using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Xeora.Web.External.Service.Session
{
    public class ConnectionHandler
    {
        private TcpClient _RemoteClient;
        private Stream _RemoteStream;
        private MessageHandler _MessageHandler;

        public ConnectionHandler(ref TcpClient remoteClient)
        {
            this._RemoteClient = remoteClient;
            this._RemoteStream = this._RemoteClient.GetStream();
            this._MessageHandler = new MessageHandler(ref this._RemoteStream);
        }

        public async void HandleAsync()
        {
            await Task.Run(() => this.ReadSocket());

            this._RemoteClient.Close();
        }

        private void ReadSocket()
        {
            byte[] buffer = new byte[1024];
            int bR = 0;

            Stream contentStream = null;
            try
            {
                this.MakeContentStream(ref contentStream);

                do
                {
                    bR = this._RemoteStream.Read(buffer, 0, buffer.Length);
                    if (bR == 0)
                    {
                        // give time to fill buffer
                        System.Threading.Thread.Sleep(1);

                        continue;
                    }

                    if (bR == 1 && contentStream.Length == 0 && buffer[0] == 0)
                        return;

                    contentStream.Seek(0, SeekOrigin.End);
                    contentStream.Write(buffer, 0, bR);

                    if (buffer[bR - 1] == 0)
                    {
                        // Process Message
                        this._MessageHandler.Process(ref contentStream);

                        this.MakeContentStream(ref contentStream);
                    }
                } while (true);
            }
            catch (System.Exception ex)
            {
                // Skip SocketExceptions
                if (ex is IOException && ex.InnerException is SocketException)
                    return;

                Basics.Console.Push("SYSTEM ERROR", ex.Message, false);
            }
            finally
            {
                this.KillContentStream(ref contentStream);
            }
        }

        private void MakeContentStream(ref Stream contentStream)
        {
            this.KillContentStream(ref contentStream);

            contentStream = new MemoryStream();
        }

        private void KillContentStream(ref Stream contentStream)
        {
            if (contentStream != null)
            {
                contentStream.Close();
                GC.SuppressFinalize(contentStream);
            }
        }
    }
}
