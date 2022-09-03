using System;
using System.IO;

namespace Xeora.Web.Service.Context.Response
{
    public class HttpResponseStream : Xeora.Web.Basics.Context.Response.IHttpResponseStream
    {
        private static readonly byte[] NewlineBytes = {13, 10};
        
        private readonly Net.NetworkStream _StreamEnclosure;
        private readonly bool _Chunked;
        private bool _Disposed;

        internal HttpResponseStream(Net.NetworkStream streamEnclosure, bool chunked)
        {
            this._StreamEnclosure = streamEnclosure;
            this._Chunked = chunked;
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            if (this._Disposed)
                throw new IOException("It is not allowed to write to the disposed stream!");

            if (this._Chunked)
            { this.WriteChunk(buffer, offset, count); return; }
            
            this._StreamEnclosure.Write(buffer, offset,count);
        }

        private void WriteChunk(byte[] buffer, int offset, int count)
        {
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

            if (!this._Chunked) return;
            
            byte[] lastChunkBytes = 
                System.Text.Encoding.ASCII.GetBytes("0\r\n\r\n");
            this._StreamEnclosure.Write(lastChunkBytes, 0, lastChunkBytes.Length);
        }
    }
}
