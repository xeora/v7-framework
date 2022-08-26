using System;
using System.IO;
using System.Text;

namespace Xeora.Web.Service.Context.Request.Parser
{
    public class MultipartFormData
    {
        private struct ContentHeader
        {
            public string ContentDisposition;
            public string Name;
            public string FileName;
            public string ContentType;
            public Encoding ContentEncoding;
        }

        private const int MEMORY_THRESHOLD_SIZE = 3670016;
        private readonly string _ContextId;
        
        private readonly Basics.Context.Request.IHttpRequestHeader _Header;
        private readonly IO.BodyStream _BodyStream;

        private byte[] _LeftOver = Array.Empty<byte>();

        public MultipartFormData(string contextId, Basics.Context.Request.IHttpRequestHeader header, IO.BodyStream bodyStream)
        {
            this._ContextId = contextId;
            
            this._Header = header;
            this._BodyStream = bodyStream;
        }

        private void Return(ref Stream contentStream, SeekOrigin origin, int offset, int count)
        {
            byte[] newLeftOver = new byte[count];
            
            contentStream.Seek(offset, origin);
            int read = 
                contentStream.Read(newLeftOver, 0, newLeftOver.Length);
            
            Array.Resize(ref newLeftOver, read + this._LeftOver.Length);
            Array.Copy(this._LeftOver, 0, newLeftOver, read, this._LeftOver.Length);

            this._LeftOver = newLeftOver;
        }

        private int Read(byte[] buffer, int offset, int count)
        {
            if (this._LeftOver.Length <= 0) return this._BodyStream.Read(buffer, offset, count);
            
            if (this._LeftOver.Length > count)
            {
                Array.Copy(this._LeftOver, 0, buffer, offset, count);
                Array.Copy(this._LeftOver, count, this._LeftOver, 0, this._LeftOver.Length - count);
                Array.Resize(ref this._LeftOver, this._LeftOver.Length - count);
                return count;
            }

            int size = this._LeftOver.Length;
            Array.Copy(this._LeftOver, 0, buffer, offset, this._LeftOver.Length);
            this._LeftOver = Array.Empty<byte>();
            size += this._BodyStream.Read(buffer, offset + size, count - size);
            return size;
        }

        internal ParserResultTypes Parse(HttpRequestForm form, HttpRequestFile file)
        {
            try
            {
                while (this.MoveToNextContent())
                {
                    ContentHeader cH = 
                        this.ReadContentHeader();
                    this.HandleContent(cH, form, file);
                }

                return ParserResultTypes.Success;
            }
            catch (IOException)
            {
                return ParserResultTypes.BadRequest;
            }
        }

        private bool MoveToNextContent()
        {
            string searchBoundary = $"--{this._Header.Boundary}";
            string endBoundary = $"--{this._Header.Boundary}--";

            Stream contentStream = null;
            try
            {
                byte[] buffer = new byte[short.MaxValue];
                
                contentStream = new MemoryStream();
                string content = string.Empty;
                
                do
                {
                    int size = 
                        this.Read(buffer, 0, buffer.Length);

                    contentStream.Write(buffer, 0, size);
                    content += Encoding.ASCII.GetString(buffer, 0, size);

                    int eofIndex =
                        content.IndexOf(searchBoundary, StringComparison.Ordinal);
                    if (eofIndex == -1)
                        continue;

                    if (eofIndex + endBoundary.Length > content.Length)
                        continue;

                    if (eofIndex == content.IndexOf(endBoundary, StringComparison.Ordinal))
                        return false;

                    this.Return(ref contentStream, SeekOrigin.Begin, eofIndex, content.Length - eofIndex);
                    return true;
                } while (true);
            }
            finally
            {
                contentStream?.Dispose();
            }
        }

        private ContentHeader ReadContentHeader()
        {
            const string rnrn = "\r\n\r\n";
            const string nn = "\n\n";
            int nl;

            Stream contentStream = null;
            try
            {
                byte[] buffer = new byte[short.MaxValue];
                
                contentStream = new MemoryStream();
                string content = string.Empty;
                int eofIndex;

                do
                {
                    int size = 
                        this.Read(buffer, 0, buffer.Length);

                    contentStream.Write(buffer, 0, size);
                    content += Encoding.ASCII.GetString(buffer, 0, size);

                    nl = 4;
                    eofIndex = 
                        content.IndexOf(rnrn, StringComparison.Ordinal);
                    if (eofIndex == -1)
                    {
                        nl = 2;
                        eofIndex = content.IndexOf(nn, StringComparison.Ordinal);
                    }
                    if (eofIndex == -1)
                        continue;

                    eofIndex += nl;

                    this.Return(ref contentStream, SeekOrigin.Begin, eofIndex, content.Length - eofIndex);
                    break;
                } while (true);

                return this.ParseContentHeader(content.Substring(0, eofIndex));
            }
            finally
            {
                contentStream?.Dispose();
            }
        }

        private ContentHeader ParseContentHeader(string contentHeader)
        {
            ContentHeader rCH = new ContentHeader();
            StringReader sR = new StringReader(contentHeader);

            while (sR.Peek() > -1)
            {
                string line = sR.ReadLine();

                if (line.IndexOf(this._Header.Boundary, StringComparison.InvariantCulture) > -1)
                    continue;

                if (string.IsNullOrEmpty(line))
                    break;

                string[] keyValues = line.Split(';');

                foreach (string keyValue in keyValues)
                {
                    int sepIndex = keyValue.IndexOf(':');
                    if (sepIndex == -1)
                    {
                        sepIndex = keyValue.IndexOf('=');

                        if (sepIndex == -1)
                            continue;
                    }

                    string key = keyValue.Substring(0, sepIndex);
                    key = key.Trim();
                    string value = keyValue.Substring(sepIndex + 1);
                    value = value.Trim();

                    if (value.StartsWith("\"", StringComparison.InvariantCulture))
                        value = value.Substring(1);

                    if (value.EndsWith("\"", StringComparison.InvariantCulture))
                        value = value.Substring(0, value.Length - 1);

                    int semiIndex = value.IndexOf(';');
                    if (semiIndex > -1)
                        value = value.Substring(0, semiIndex);

                    switch (key.ToLowerInvariant())
                    {
                        case "content-disposition":
                            rCH.ContentDisposition = value;

                            break;
                        case "name":
                            rCH.Name = value;

                            break;
                        case "filename":
                            rCH.FileName = value;

                            break;
                        case "content-type":
                            rCH.ContentType = value;

                            break;
                        case "charset":
                            try
                            {
                                rCH.ContentEncoding = Encoding.GetEncoding(value);
                            }
                            catch (Exception)
                            {
                                rCH.ContentEncoding = null;
                            }

                            break;
                    }
                }
            }

            return rCH;
        }

        private void HandleContent(ContentHeader contentHeader, HttpRequestForm form, HttpRequestFile file)
        {
            if (string.IsNullOrEmpty(contentHeader.Name))
                return;

            if (string.IsNullOrEmpty(contentHeader.ContentType))
            {
                Stream contentStream = null;
                StreamReader sR = null;

                try
                {
                    contentStream = new MemoryStream();

                    if (this.ReadContentBody(contentStream))
                    {
                        if (contentHeader.ContentEncoding != null)
                            sR = new StreamReader(contentStream, contentHeader.ContentEncoding, true);
                        else if (this._Header.ContentEncoding != null)
                            sR = new StreamReader(contentStream, this._Header.ContentEncoding, true);
                        else
                            sR = new StreamReader(contentStream, true);

                        form.AddOrUpdate(contentHeader.Name, sR.ReadToEnd());
                    }
                }
                catch (Exception)
                {
                    // Just Handle Exceptions
                }
                finally
                {
                    sR?.Dispose();
                    contentStream?.Dispose();
                }

                return;
            }

            // if the request size smaller than 3,5 MB, use the memory.
            HttpRequestFileInfo requestFileInfo =
                new HttpRequestFileInfo(
                    this._ContextId, 
                    contentHeader.ContentType, 
                    contentHeader.ContentEncoding, 
                    contentHeader.FileName, 
                    this._Header.ContentLength < MEMORY_THRESHOLD_SIZE
                );
            if (this.ReadContentBody(requestFileInfo.ContentStream))
                file.AddOrUpdate(contentHeader.Name, requestFileInfo);
        }

        private bool ReadContentBody(Stream containerStream)
        {
            string searchBoundary = $"--{this._Header.Boundary}";
            const string rn = "\r\n";
            const string n = "\n";

            int totalRead = 0;

            byte[] buffer = new byte[short.MaxValue];

            do
            {
                int size = 
                    this.Read(buffer, 0, buffer.Length);
                
                totalRead += size;
                containerStream.Write(buffer, 0, size);

                string content = Encoding.ASCII.GetString(buffer, 0, size);

                int eofIndex = 
                    content.IndexOf(searchBoundary, StringComparison.Ordinal);
                if (eofIndex == -1)
                {
                    totalRead -= searchBoundary.Length;

                    this.Return(ref containerStream, SeekOrigin.Current, searchBoundary.Length * -1, searchBoundary.Length);

                    containerStream.Seek(searchBoundary.Length * -1, SeekOrigin.Current);

                    continue;
                }

                this.Return(ref containerStream, SeekOrigin.Current, (size - (eofIndex - 2)) * -1, size - (eofIndex - 2));

                byte[] newLineTest = new byte[2];
                this.Read(newLineTest, 0, newLineTest.Length);

                if (string.CompareOrdinal(Encoding.ASCII.GetString(newLineTest), rn) == 0)
                    eofIndex -= 2;
                else if (string.CompareOrdinal(Encoding.ASCII.GetString(newLineTest, 1, 1), n) == 0)
                    eofIndex -= 1;

                containerStream.Seek(0, SeekOrigin.Begin);
                containerStream.SetLength(totalRead - size + eofIndex);

                return true;
            } while (true);
        }
    }
}
