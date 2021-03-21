using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Xeora.Web.Exceptions;

namespace Xeora.Web.Service.Dss
{
    public class Handler
    {
        private readonly Stream _ResponseStream;
        private readonly IManager _Manager;

        public Handler(ref Stream responseStream, IManager manager)
        {
            this._ResponseStream = responseStream;
            this._Manager = manager;
        }

        public async void ProcessAsync(long requestId, byte[] requestBytes) =>
            await Task.Run(() => this.Process(requestId, requestBytes));

        private void Process(long requestId, byte[] requestBytes)
        {
            Stream responseStream = null;
            Stream requestStream = null;

            try
            {
                responseStream = new MemoryStream();
                requestStream = 
                    new MemoryStream(requestBytes, 0, requestBytes.Length, false);
                requestStream.Seek(0, SeekOrigin.Begin);

                BinaryReader binaryReader =
                     new BinaryReader(requestStream);

                char[] command = 
                    binaryReader.ReadChars(3);

                switch (new string(command))
                {
                    case "ACQ":
                        this.HandleAcq(requestId, ref binaryReader, ref responseStream);

                        break;
                    case "GET":
                        this.HandleGet(requestId, ref binaryReader, ref responseStream);

                        break;
                    case "SET":
                        this.HandleSet(requestId, ref binaryReader, ref responseStream);

                        break;
                    case "LCK":
                        this.HandleLck(requestId, ref binaryReader, ref responseStream);

                        break;
                    case "RLS":
                        this.HandleRls(requestId, ref binaryReader, ref responseStream);

                        break;
                    case "KYS":
                        this.HandleKys(requestId, ref binaryReader, ref responseStream);
                        
                        break;
                    case "EXT":
                        this.HandleExt(requestId, ref binaryReader, ref responseStream);

                        break;
                    case "ECH":
                        this.HandleEch(requestId, ref binaryReader, ref responseStream);

                        break;
                }

                Monitor.Enter(this._ResponseStream);
                try
                {
                    responseStream.Seek(0, SeekOrigin.Begin);
                    responseStream.CopyTo(this._ResponseStream);
                }
                finally
                {
                    Monitor.Exit(this._ResponseStream);
                }
            }
            catch (Exception ex)
            {
                // Skip SocketExceptions
                if (ex is IOException && ex.InnerException is SocketException)
                    return;

                Basics.Console.Push("SYSTEM ERROR", ex.Message, ex.ToString(), false, true, type: Basics.Console.Type.Error);
            }
            finally
            {
                requestStream?.Close();
                responseStream?.Close();
            }
        }

        private void PutHeader(long requestId, ref Stream contentStream)
        {
            long contentLength = contentStream.Position;
            contentLength -= 8; // Remove long length;

            long head = requestId << 24;
            head |= contentLength;

            byte[] headBytes = 
                BitConverter.GetBytes(head);

            contentStream.Seek(0, SeekOrigin.Begin);
            contentStream.Write(headBytes, 0, headBytes.Length);

            contentStream.Seek(0, SeekOrigin.End);
        }

        private void HandleAcq(long requestId, ref BinaryReader responseReader, ref Stream responseStream)
        {
            /*
             * -> \LONG\ACQ\SHORT
             * -> \LONG\ACQ\SHORT\BYTE\CHARS{BYTEVALUELENGTH}
             * <- \LONG\BYTE\BYTE\CHARS{BYTEVALUELENGTH}\BYTE\LONG
             */

            BinaryWriter binaryWriter =
                new BinaryWriter(responseStream);

            try
            {
                // Put Dummy Header
                binaryWriter.Write((long) 0);

                short reservationTimeout = 
                    responseReader.ReadInt16();
                byte uniqueIdLength = 
                    responseReader.ReadByte();
                string uniqueId = string.Empty;

                if (uniqueIdLength > 0)
                    uniqueId = new string(responseReader.ReadChars(uniqueIdLength));
                
                this._Manager.Reserve(uniqueId, reservationTimeout, out Basics.Dss.IDss reservationObject);

                binaryWriter.Write((byte) 0);
                binaryWriter.Write((byte) reservationObject.UniqueId.Length);
                binaryWriter.Write(reservationObject.UniqueId.ToCharArray());
                binaryWriter.Write(reservationObject.Reusing ? (byte) 1 : (byte) 0);
                binaryWriter.Write(reservationObject.Expires.Ticks);
            }
            catch
            {
                binaryWriter.Write((byte) 10);
            }
            finally
            {
                binaryWriter.Flush();                
            }

            this.PutHeader(requestId, ref responseStream);
        }

        private bool GetReservationObject(ref BinaryReader responseReader, out Basics.Dss.IDss reservationObject)
        {
            reservationObject = null;

            byte uniqueIdLength = responseReader.ReadByte();

            if (uniqueIdLength == 0)
                return false;

            string uniqueId = 
                new string(responseReader.ReadChars(uniqueIdLength));
            return this._Manager.Reserve(uniqueId, 0, out reservationObject);
        }

        private void HandleGet(long requestId, ref BinaryReader responseReader, ref Stream responseStream)
        {
            /*
             * -> \LONG\GET\BYTE\CHARS{BYTEVALUELENGTH}\BYTE\CHARS{BYTEVALUELENGTH}
             * <- \LONG\BYTE\BYTE\CHARS{BYTEVALUELENGTH}\INTEGER\BYTES{INTEGERVALUELENGTH}
             */
            
            BinaryWriter binaryWriter = 
                new BinaryWriter(responseStream);
            
            try
            {
                // Put Dummy Header
                binaryWriter.Write((long)0);

                if (!this.GetReservationObject(ref responseReader, out Basics.Dss.IDss reservationObject))
                {
                    binaryWriter.Write((byte)10);
                    binaryWriter.Flush();
                
                    this.PutHeader(requestId, ref responseStream);
                
                    return;
                }
            
                byte keyLength = 
                    responseReader.ReadByte();
                string key = 
                    new string(responseReader.ReadChars(keyLength));

                if (string.IsNullOrEmpty(key))
                {
                    binaryWriter.Write((byte)20);
                    binaryWriter.Flush();
                
                    this.PutHeader(requestId, ref responseStream);
                
                    return;
                }
                
                object value =
                    reservationObject.Get(key);
                binaryWriter.Write((byte) 0);

                binaryWriter.Write(keyLength);
                binaryWriter.Write(key.ToCharArray());

                if (value != null)
                {
                    byte[] valueBytes = (byte[]) value;

                    binaryWriter.Write(valueBytes.Length);
                    binaryWriter.Write(valueBytes, 0, valueBytes.Length);
                }
                else
                    binaryWriter.Write((int) 0);
            }
            catch
            {
                binaryWriter.Write((byte) 39);
            }
            finally
            {
                binaryWriter.Flush();                
            }

            this.PutHeader(requestId, ref responseStream);
        }

        private void HandleSet(long requestId, ref BinaryReader responseReader, ref Stream responseStream)
        {
            /*
             * -> \LONG\SET\BYTE\CHARS{BYTEVALUELENGTH}\BYTE\CHARS{BYTEVALUELENGTH}\BYTE\CHARS{BYTEVALUELENGTH}\INTEGER\BYTES{INTEGERVALUELENGTH}
             * <- \LONG\BYTE
             */
            
            BinaryWriter binaryWriter = 
                new BinaryWriter(responseStream);
            
            try
            {
                // Put dummy header
                binaryWriter.Write((long)0);
            
                if (!this.GetReservationObject(ref responseReader, out Basics.Dss.IDss reservationObject))
                {
                    binaryWriter.Write((byte)10);
                    binaryWriter.Flush();
                
                    this.PutHeader(requestId, ref responseStream);
                
                    return;
                }

                byte keyLength = 
                    responseReader.ReadByte();
                string key = 
                    new string(responseReader.ReadChars(keyLength));

                byte lockCodeLength = 
                    responseReader.ReadByte();
                string lockCode = 
                    new string(responseReader.ReadChars(lockCodeLength));

                int valueLength = 
                    responseReader.ReadInt32();
                byte[] valueBytes = 
                    responseReader.ReadBytes(valueLength);

                if (string.IsNullOrEmpty(key))
                {
                    binaryWriter.Write((byte)20);
                    binaryWriter.Flush();
                
                    this.PutHeader(requestId, ref responseStream);
                
                    return;
                }
                
                reservationObject.Set(key, valueBytes.Length > 0 ? valueBytes : null, lockCode);
                binaryWriter.Write((byte) 0);
            }
            catch (KeyLockedException)
            {
                binaryWriter.Write((byte) 30);
            }
            catch
            {
                binaryWriter.Write((byte) 39);
            }
            finally
            {
                binaryWriter.Flush();                
            }

            this.PutHeader(requestId, ref responseStream);
        }
        
        private void HandleLck(long requestId, ref BinaryReader responseReader, ref Stream responseStream)
        {
            /*
             * -> \LONG\LCK\BYTE\CHARS{BYTEVALUELENGTH}\BYTE\CHARS{BYTEVALUELENGTH}
             * <- \LONG\BYTE\BYTE\CHARS{BYTEVALUELENGTH}\BYTE\CHARS{BYTEVALUELENGTH}
             */
            
            BinaryWriter binaryWriter = 
                new BinaryWriter(responseStream);

            try
            {
                // Put dummy header
                binaryWriter.Write((long)0);
            
                if (!this.GetReservationObject(ref responseReader, out Basics.Dss.IDss reservationObject))
                {
                    binaryWriter.Write((byte)10);
                    binaryWriter.Flush();
                
                    this.PutHeader(requestId, ref responseStream);
                
                    return;
                }

                byte keyLength = 
                    responseReader.ReadByte();
                string key = 
                    new string(responseReader.ReadChars(keyLength));

                if (string.IsNullOrEmpty(key))
                {
                    binaryWriter.Write((byte)20);
                    binaryWriter.Flush();
                
                    this.PutHeader(requestId, ref responseStream);
                
                    return;
                }
                
                string lockCode =
                    reservationObject.Lock(key);
                binaryWriter.Write((byte) 0);

                binaryWriter.Write(keyLength);
                binaryWriter.Write(key.ToCharArray());

                binaryWriter.Write((byte) lockCode.Length);
                binaryWriter.Write(lockCode.ToCharArray());
            }
            catch
            {
                binaryWriter.Write((byte) 39);
            }
            finally
            {
                binaryWriter.Flush();    
            }

            this.PutHeader(requestId, ref responseStream);
        }
        
        private void HandleRls(long requestId, ref BinaryReader responseReader, ref Stream responseStream)
        {
            /*
             * -> \LONG\RLS\BYTE\CHARS{BYTEVALUELENGTH}\BYTE\CHARS{BYTEVALUELENGTH}\BYTE\CHARS{BYTEVALUELENGTH}
             * <- \LONG\BYTE
             */
            
            BinaryWriter binaryWriter = 
                new BinaryWriter(responseStream);

            try
            {
                // Put dummy header
                binaryWriter.Write((long)0);
                
                if (!this.GetReservationObject(ref responseReader, out Basics.Dss.IDss reservationObject))
                {
                    binaryWriter.Write((byte)10);
                    binaryWriter.Flush();
                    
                    this.PutHeader(requestId, ref responseStream);
                    
                    return;
                }

                byte keyLength = 
                    responseReader.ReadByte();
                string key = 
                    new string(responseReader.ReadChars(keyLength));

                byte lockCodeLength = 
                    responseReader.ReadByte();
                string lockCode = 
                    new string(responseReader.ReadChars(lockCodeLength));
                
                if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(lockCode))
                {
                    binaryWriter.Write((byte)20);
                    binaryWriter.Flush();
                    
                    this.PutHeader(requestId, ref responseStream);
                    
                    return;
                }
                
                reservationObject.Release(key, lockCode);
                
                binaryWriter.Write((byte)0);
            }
            catch
            {
                binaryWriter.Write((byte) 39);
            }
            finally
            {
                binaryWriter.Flush();    
            }
            
            this.PutHeader(requestId, ref responseStream);
        }

        private void HandleKys(long requestId, ref BinaryReader responseReader, ref Stream responseStream)
        {
            /*
             * -> \LONG\KYS\BYTE\CHARS{BYTEVALUELENGTH}
             * <- \LONG\BYTE\BYTE\CHARS{BYTEVALUELENGTH}\BYTE\CHARS{BYTEVALUELENGTH}\BYTE\CHARS{BYTEVALUELENGTH}...\BYTE
             */
            
            BinaryWriter binaryWriter = 
                new BinaryWriter(responseStream);
            
            try
            {
                // Put dummy header
                binaryWriter.Write((long)0);
            
                if (!this.GetReservationObject(ref responseReader, out Basics.Dss.IDss reservationObject))
                {
                    binaryWriter.Write((byte)10);
                    binaryWriter.Flush();
                
                    this.PutHeader(requestId, ref responseStream);
                
                    return;
                }

                binaryWriter.Write((byte) 0);

                foreach (string key in reservationObject.Keys)
                {
                    binaryWriter.Write((byte) key.Length);
                    binaryWriter.Write(key.ToCharArray());
                }
                
                binaryWriter.Write((byte) 0);
            }
            catch
            {
                binaryWriter.Write((byte) 39);
            }
            finally
            {
                binaryWriter.Flush();                
            }
            
            this.PutHeader(requestId, ref responseStream);
        }
        
        private void HandleExt(long requestId, ref BinaryReader responseReader, ref Stream responseStream)
        {
            /*
             * -> \LONG\EXT\BYTE\CHARS{BYTEVALUELENGTH}
             * <- \LONG\BYTE
             */

            BinaryWriter binaryWriter =
                new BinaryWriter(responseStream);

            try
            {
                // Put Dummy Header
                binaryWriter.Write((long) 0);
            
                byte uniqueIdLength = 
                    responseReader.ReadByte();
                string uniqueId = string.Empty;

                if (uniqueIdLength > 0)
                    uniqueId = new string(responseReader.ReadChars(uniqueIdLength));
                
                if (uniqueIdLength == 0)
                    throw new ArgumentNullException();

                if (!this._Manager.Reserve(uniqueId, 0, out Basics.Dss.IDss reservationObject))
                    throw new ObjectDisposedException(nameof(reservationObject));

                ((IService) reservationObject).Extend();

                binaryWriter.Write((byte) 0);
            }
            catch
            {
                binaryWriter.Write((byte) 39);
            }
            finally
            {
                binaryWriter.Flush();                
            }

            this.PutHeader(requestId, ref responseStream);
        }
        
        private void HandleEch(long requestId, ref BinaryReader responseReader, ref Stream responseStream)
        {
            /*
             * -> \LONG\ECH\BYTE\CHARS{BYTEVALUELENGTH}
             * <- \LONG\BYTE\CHARS{BYTEVALUELENGTH}
             */

            BinaryWriter binaryWriter =
                new BinaryWriter(responseStream);
            
            try
            {
                // Put Dummy Header
                binaryWriter.Write((long) 0);
            
                byte echoMessageLength = 
                    responseReader.ReadByte();
                string echoMessage = string.Empty;

                if (echoMessageLength > 0)
                    echoMessage = new string(responseReader.ReadChars(echoMessageLength));

                binaryWriter.Write((byte) 0);

                binaryWriter.Write(echoMessageLength);
                binaryWriter.Write(echoMessage.ToCharArray());
            }
            catch
            {
                binaryWriter.Write((byte) 39);
            }
            finally
            {
                binaryWriter.Flush();                
            }

            this.PutHeader(requestId, ref responseStream);
        }
    }
}
