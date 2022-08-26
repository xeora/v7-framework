using System;
using System.IO;

namespace Xeora.Web.Service.Context.Response
{
    public class HttpResponseStream : Xeora.Web.Basics.Context.Response.IHttpResponseStream
    {
        private static byte[] NewlineBytes = {13, 10};
        
        private readonly Net.NetworkStream _StreamEnclosure;
        private bool _Disposed;

        internal HttpResponseStream(Net.NetworkStream streamEnclosure)
        {
            this._StreamEnclosure = streamEnclosure;
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            if (this._Disposed)
                throw new IOException("It is not allowed to write to the disposed stream!");
            
            byte[] countBytes = 
                BitConverter.GetBytes(count - offset);
            Array.Reverse(countBytes);
            int nonZeroIndex = 0;
            for (int i = 0; i < countBytes.Length; i++)
            {
                if (countBytes[i] == 0) continue;
                
                nonZeroIndex = i;
                break;
            }

            string chunkSizeHex = 
                $"{Convert.ToHexString(countBytes, nonZeroIndex, countBytes.Length - nonZeroIndex)}";
            byte[] chunkSizeBytes = System.Text.Encoding.ASCII.GetBytes(chunkSizeHex);
            
            this._StreamEnclosure.Write(chunkSizeBytes, 0, chunkSizeBytes.Length);
            this._StreamEnclosure.Write(NewlineBytes, 0, NewlineBytes.Length);
            this._StreamEnclosure.Write(buffer, offset, count);
            this._StreamEnclosure.Write(NewlineBytes, 0, NewlineBytes.Length);
        }
        
        public void Dispose()
        {
            if (this._Disposed) return;
            this._Disposed = true;
            
            byte[] lastChunkBytes = 
                System.Text.Encoding.ASCII.GetBytes("0\r\n\r\n");
            this._StreamEnclosure.Write(lastChunkBytes, 0, lastChunkBytes.Length);
        }
    }
}
