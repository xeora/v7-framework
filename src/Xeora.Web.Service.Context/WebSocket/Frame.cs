using System;
using System.IO;

namespace Xeora.Web.Service.Context.WebSocket
{
    public class Frame
    {
        private readonly Func<ulong, byte[]> _ReadRequestHandler;

        private byte[] _Header;

        public Frame() =>
            this.Data = new Payload();
        
        public Frame(Func<ulong, byte[]> readRequestHandler) =>
            this._ReadRequestHandler = readRequestHandler;
        
        public void Inject(byte[] header)
        {
            this._Header = header;
            this.ParseHeader();
        }

        public void ProcessInto(Stream contentStream)
        {
            this.Data.ImportToFragments();
            
            if (this.Fin)
            {
                this.Data.ProcessInto(contentStream);
                return;
            }

            Frame fragmentFrame;
            do
            {
                
                fragmentFrame = 
                    new Frame(this._ReadRequestHandler);
                        
                byte[] frameHeader = 
                    this._ReadRequestHandler(2);
                
                fragmentFrame.Inject(frameHeader);
                byte[] data = 
                    fragmentFrame.Data.ExportAsFragment();
                this.Data.ImportToFragments(data);
            } while (!fragmentFrame.Fin);
            
            this.Data.ProcessInto(contentStream);
        }
        
        public bool Fin { get; set; }
        public bool Rsv1 { get; set; }
        public bool Rsv2 { get; set; }
        public bool Rsv3 { get; set; }
        public OpCodes OpCode { get; set; }
        public bool Mask { get; set; }
        public byte[] MaskBytes { get; set; }
        public ulong DataLength { get; private set; }
        private Payload Data { get; set; }
        
        private void ParseHeader()
        {
            this.Fin = (this._Header[0] & 0b10000000) != 0;
            this.Rsv1 = (this._Header[0] & 0b01000000) != 0;
            this.Rsv2 = (this._Header[0] & 0b00100000) != 0;
            this.Rsv3 = (this._Header[0] & 0b00010000) != 0;
            this.OpCode = (OpCodes)(this._Header[0] & 0b00001111);
            this.Mask = (this._Header[1] & 0b10000000) != 0;
            
            this.DataLength = 
                (ulong)(this._Header[1] & 0b01111111);
            byte[] extendedDataLengthBytes;

            switch (this.DataLength)
            {
                case 126:
                    // following 2 bytes are payload length
                    extendedDataLengthBytes = this._ReadRequestHandler(2);
                    Array.Reverse(extendedDataLengthBytes);
                    this.DataLength = BitConverter.ToUInt16(extendedDataLengthBytes, 0);

                    break;
                case 127:
                    // following 8 bytes are payload length
                    extendedDataLengthBytes = this._ReadRequestHandler(8);
                    Array.Reverse(extendedDataLengthBytes);
                    this.DataLength = BitConverter.ToUInt64(extendedDataLengthBytes, 0);

                    break;
            }

            this.MaskBytes = 
                this.Mask ? this._ReadRequestHandler(4) : Array.Empty<byte>();
            this.Data = 
                new Payload(this.Rsv1, this.MaskBytes, this.DataLength, this._ReadRequestHandler);
        }

        public void BuildInto(byte[] buffer, int offset, int count, Stream targetStream)
        {
            byte[] frameHeader = new byte[2];
            
            if (this.Fin) frameHeader[0] |= 0b10000000;
            if (this.Rsv1) frameHeader[0] |= 0b01000000;
            if (this.Rsv2) frameHeader[0] |= 0b00100000;
            if (this.Rsv3) frameHeader[0] |= 0b00010000;
            
            frameHeader[0] |= (byte)this.OpCode;

            byte[] data =
                this.Data.PrepareFrom(this.Rsv1, buffer, offset, count);
            byte[] dataLengthBytes = Array.Empty<byte>();
            
            switch (data.Length)
            {
                case < 125:
                    frameHeader[1] = (byte)data.Length;
                    break;
                case > 125 and < ushort.MaxValue:
                    frameHeader[1] = 126;
                    Array.Resize(ref frameHeader, 4);
                    
                    dataLengthBytes = 
                        BitConverter.GetBytes((ushort)data.Length);

                    break;
                default:
                    frameHeader[1] = 127;
                    Array.Resize(ref frameHeader, 10);
                    
                    dataLengthBytes = 
                        BitConverter.GetBytes((ulong)data.Length);
                    
                    break;
            }

            if (dataLengthBytes.Length > 0)
            {
                if (BitConverter.IsLittleEndian) Array.Reverse(dataLengthBytes);
                Array.Copy(dataLengthBytes, 0, frameHeader, 2, dataLengthBytes.Length);
            }
            
            targetStream.Write(frameHeader, 0, frameHeader.Length);
            targetStream.Write(data, 0, data.Length);
        }
    }
}
