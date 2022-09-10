using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Xeora.Web.Service.Context.WebSocket
{
    public class Payload
    {
        private readonly bool _Compression;
        private readonly byte[] _MaskBytes;
        private readonly ulong _Length;
        private readonly Func<ulong, byte[]> _ReadRequestHandler;

        private readonly List<byte[]> _Fragments;

        public Payload() =>
            this._Fragments = new List<byte[]>();
        
        public Payload(bool compression, byte[] maskBytes, ulong length, Func<ulong, byte[]> readRequestHandler) :
            this()
        {
            this._Compression = compression;
            this._MaskBytes = maskBytes;
            this._Length = length;
            this._ReadRequestHandler = readRequestHandler;
        }

        public void ImportToFragments()
        {
            if (this._Length == 0) return;
            
            byte[] dataBytes =
                this._ReadRequestHandler(this._Length);
            
            this.Unmask(dataBytes);
            
            this._Fragments.Add(dataBytes);
        }
        
        public void ImportToFragments(byte[] data)
        {
            if (data.Length == 0) return;
            this._Fragments.Add(data);
        }
        
        public byte[] ExportAsFragment()
        {
            if (this._Length == 0) return Array.Empty<byte>();
            
            byte[] dataBytes =
                this._ReadRequestHandler(this._Length);
            
            this.Unmask(dataBytes);

            return dataBytes;
        }

        public void ProcessInto(Stream contentStream)
        {
            byte[] dataBytes = Array.Empty<byte>();

            foreach (byte[] data in this._Fragments)
            {
                Array.Resize(ref dataBytes, dataBytes.Length + data.Length);
                Array.Copy(data, 0, dataBytes, dataBytes.Length - data.Length, data.Length);
            }

            if (this._Compression)
            {
                this.Decompress(dataBytes, contentStream);
                return;
            }
            
            contentStream.Write(dataBytes, 0, dataBytes.Length);
        }
        
        public byte[] PrepareFrom(bool compress, byte[] buffer, int offset, int count)
        {
            if (compress)
                return this.Compress(buffer, offset, count);
            
            Array.Copy(buffer, 0, buffer, offset, count);
            Array.Resize(ref buffer, count);

            return buffer;
        }

        private void Unmask(byte[] dataBytes)
        {
            // Shouldn't be zero length on server side and there will be no client implementation
            // So it is just precaution implementation
            if (this._MaskBytes.Length == 0) return;
            
            for (int i = 0; i < dataBytes.Length; i++)
                dataBytes[i] ^= this._MaskBytes[i % 4];
        }

        private void Decompress(byte[] input, Stream outputStream)
        {
            DeflateStream deflatedStream = null;
            Stream inputStream = null;
            try
            {
                inputStream = new MemoryStream(input);
                inputStream.Seek(0, SeekOrigin.Begin);

                deflatedStream = 
                    new DeflateStream(inputStream, CompressionMode.Decompress, true);
                deflatedStream.CopyTo(outputStream, 1024);
            }
            finally
            {
                deflatedStream?.Dispose();
                inputStream?.Dispose();
            }
        }
        
        private byte[] Compress(byte[] input, int offset, int count)
        {
            MemoryStream outputStream = null;
            Stream inputStream = null;
            try
            {
                outputStream = new MemoryStream();
                
                inputStream = new MemoryStream();
                inputStream.Write(input, offset, count);
                inputStream.Seek(0, SeekOrigin.Begin);

                DeflateStream deflatedStream = null;
                try
                {
                    deflatedStream = 
                        new DeflateStream(outputStream, CompressionMode.Compress, true);
                    inputStream.CopyTo(deflatedStream, 1024);
                }
                finally
                {
                    deflatedStream?.Dispose();
                }
                
                // Write last terminator (BFINAL)
                outputStream.Write(new byte[] {0x00}, 0, 1);
                outputStream.Close();
                
                outputStream.SetLength(outputStream.Position);
                return outputStream.GetBuffer();
            }
            finally
            {
                inputStream?.Dispose();
                outputStream?.Dispose();
            }
        }
    }
}
