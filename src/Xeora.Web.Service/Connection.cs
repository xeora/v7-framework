using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using Xeora.Web.Configuration;

namespace Xeora.Web.Service
{
    public class Connection
    {
        private readonly TcpClient _RemoteClient;
        private readonly X509Certificate2 _Certificate;

        private const short READ_TIMEOUT = 5; // 5 seconds

        public Connection(ref TcpClient remoteClient, X509Certificate2 certificate)
        {
            this._RemoteClient = remoteClient;
            this._Certificate = certificate;
        }

        public void Process()
        {
            IPEndPoint remoteIpEndPoint =
                (IPEndPoint)this._RemoteClient.Client.RemoteEndPoint;
            Stream remoteStream = this._RemoteClient.GetStream();

            this.Handle(remoteIpEndPoint, ref remoteStream);

            remoteStream.Close();
            remoteStream.Dispose();

            this._RemoteClient.Close();
            this._RemoteClient.Dispose();
            
            Basics.Console.Flush();
        }

        private void Handle(IPEndPoint remoteIpEndPoint, ref Stream remoteStream)
        {
            if (Configuration.Manager.Current.Configuration.Service.Ssl &&
                !this.MakeSecure(ref remoteStream, remoteIpEndPoint)) return;

            // If reads create problems and put the loop to infinite. drop the connection.
            // that's why, 5 seconds timeout should be set to remoteStream
            // No need to put timeout to write operation because xeora will handle connection state
            remoteStream.ReadTimeout = READ_TIMEOUT * 1000;

            Net.NetworkStream streamEnclosure = 
                new Net.NetworkStream(ref remoteStream);

            Basics.Console.Push("Connection is accepted from", string.Format("{0} ({1})", remoteIpEndPoint, Configuration.Manager.Current.Configuration.Service.Ssl ? "Secure" : "Basic"), string.Empty, true);

            ClientState.Handle(remoteIpEndPoint.Address, streamEnclosure);
        }

        private bool MakeSecure(ref Stream remoteStream, IPEndPoint remoteIpEndPoint)
        {
            remoteStream = new SslStream(remoteStream, false);

            try
            {
                ((SslStream)remoteStream).AuthenticateAsServer(this._Certificate, false, System.Security.Authentication.SslProtocols.Tls12, true);

                return true;
            }
            catch (IOException ex)
            {
                Basics.Console.Push("Connection is rejected from", $"{remoteIpEndPoint} ({ex.Message})", string.Empty, true, type: Basics.Console.Type.Warn);

                return false;
            }
            catch (System.Exception ex)
            {
                Basics.Console.Push("Ssl Connection FAILED!", ex.Message, ex.ToString(), false, true, type: Basics.Console.Type.Error);

                return false;
            }
        }
    }
}