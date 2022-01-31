using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;

namespace Xeora.Web.Service.Dss
{
    public class SyncStreamHandler : IDisposable
    {
        private readonly Stream _ResponseStream;
            
        public SyncStreamHandler([NotNull] Stream remoteStream) =>
            this._ResponseStream = remoteStream;

        public bool Lock(Func<Stream, bool> lockedStream)
        {
            if (lockedStream == null) return false;
            
            Monitor.Enter(this._ResponseStream);
            try
            {
                return lockedStream.Invoke(this._ResponseStream);
            }
            finally
            {
                Monitor.Exit(this._ResponseStream);
            }
        }

        public int ReadUnsafe(byte[] buffer, int offset, int count) =>
            this._ResponseStream.Read(buffer, offset, count);
        
        public void Dispose()
        {
            this._ResponseStream.Close();
            this._ResponseStream.Dispose();
            
            GC.SuppressFinalize(this);
        }
    }
}