using System;
using System.IO;
using System.Text;

namespace Xeora.Web.Service.Context.Request
{
    public class HttpRequestBody : Basics.Context.Request.IHttpRequestBody
    {
        private readonly string _ContextId;

        private readonly Basics.Context.Request.IHttpRequestHeader _Header;
        private readonly Net.NetworkStream _StreamEnclosure;

        private Stream _ContentStream;

        public HttpRequestBody(string contextId, Basics.Context.Request.IHttpRequestHeader header, Net.NetworkStream streamEnclosure)
        {
            this._ContextId = contextId;

            this._Header = header;
            this._StreamEnclosure = streamEnclosure;

            this.Form = new HttpRequestForm();
            this.File = new HttpRequestFile();

            this.Parse();
        }

        private void AddToResidual(ref Stream contentStream, SeekOrigin origin, int offset, int count)
        {
            byte[] contentBytes = new byte[count];
            contentStream.Seek(offset, origin);
            contentStream.Read(contentBytes, 0, contentBytes.Length);

            this._StreamEnclosure.Return(contentBytes, 0, contentBytes.Length);
        }

        private void ReadToEndInto(ref Stream contentStream)
        {
            contentStream ??= new MemoryStream();

            if (this._Header.ContentLength > 0)
                this.ReadWithLength(this._Header.ContentLength, ref contentStream);
            else
                this.ReadBlind(ref contentStream);
        }

        private void ReadWithLength(int length, ref Stream contentStream)
        {
            Stream contentStreamReference = contentStream;

            this._StreamEnclosure.Listen((buffer, size) =>
            {
                int read = size;
                if (length < read)
                    read = length;

                contentStreamReference.Write(buffer, 0, read);
                length -= read;

                if (read - size < 0)
                    this._StreamEnclosure.Return(buffer, read, size - read);

                return length > 0;
            });

            contentStream = contentStreamReference;
        }

        private void ReadBlind(ref Stream contentStream)
        {
            byte[] buffer = new byte[1024];
            int waitCount = 5;

            do
            {
                int bR = this._StreamEnclosure.Read(buffer, 0, buffer.Length);

                if (bR == 0)
                {
                    if (waitCount == 0)
                        return;

                    waitCount--;
                    System.Threading.Thread.Sleep(1);

                    continue;
                }

                waitCount = 5;
                contentStream.Write(buffer, 0, bR);
            } while (true);
        }

        private void Parse()
        {
            switch (this._Header.ContentType)
            {
                case "multipart/form-data":
                    this.ParseMultipartFormData();

                    break;
                case "application/x-www-form-urlencoded":
                    this.ParseApplicationXwwwFormUrlEncoded();

                    break;
                default:
                    this.CreateInputStream();

                    break;
            }
        }

        private void ParseMultipartFormData()
        {
            while (this.MoveToNextContent())
            {
                ContentHeader cH = this.ReadContentHeader();

                this.HandleContent(cH);
            }
        }

        private bool MoveToNextContent()
        {
            string searchBoundary = $"--{this._Header.Boundary}";
            string endBoundary = $"--{this._Header.Boundary}--";

            Stream contentStream = null;
            try
            {
                bool foundNextContent = false;

                contentStream = new MemoryStream();
                string content = string.Empty;

                this._StreamEnclosure.Listen((buffer, size) =>
                {
                    contentStream.Write(buffer, 0, size);
                    content += Encoding.ASCII.GetString(buffer, 0, size);

                    int eofIndex = 
                        content.IndexOf(searchBoundary, StringComparison.Ordinal);
                    if (eofIndex == -1)
                        return true;

                    if (eofIndex + endBoundary.Length > content.Length)
                        return true;

                    if (eofIndex == content.IndexOf(endBoundary, StringComparison.Ordinal))
                        return false;

                    this.AddToResidual(ref contentStream, SeekOrigin.Begin, eofIndex, content.Length - eofIndex);
                    foundNextContent = true;

                    return false;
                });

                return foundNextContent;
            }
            finally
            {
                contentStream?.Close();
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
                contentStream = new MemoryStream();

                string content = string.Empty;
                int eofIndex = 0;

                bool completed = this._StreamEnclosure.Listen((buffer, size) =>
                {
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
                        return true;

                    eofIndex += nl;

                    this.AddToResidual(ref contentStream, SeekOrigin.Begin, eofIndex, content.Length - eofIndex);

                    return false;
                });

                return !completed ? default : this.ParseContentHeader(content.Substring(0, eofIndex));
            }
            finally
            {
                contentStream?.Close();
            }
        }

        private struct ContentHeader
        {
            public string ContentDisposition;
            public string Name;
            public string FileName;
            public string ContentType;
            public Encoding ContentEncoding;
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

        private void HandleContent(ContentHeader contentHeader)
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

                        ((HttpRequestForm)this.Form).AddOrUpdate(contentHeader.Name, sR.ReadToEnd());
                    }
                }
                catch (Exception)
                {
                    // Just Handle Exceptions
                }
                finally
                {
                    sR?.Close();
                    contentStream?.Close();
                }

                return;
            }

            // if the request size smaller than 3,5 MB, use the memory.
            HttpRequestFileInfo requestFI =
                new HttpRequestFileInfo(
                    this._ContextId, 
                    contentHeader.ContentType, 
                    contentHeader.ContentEncoding, 
                    contentHeader.FileName, 
                    this._Header.ContentLength < 3670016
                );
            if (this.ReadContentBody(requestFI.ContentStream))
                ((HttpRequestFile)this.File).AddOrUpdate(contentHeader.Name, requestFI);
        }

        private bool ReadContentBody(Stream containerStream)
        {
            string searchBoundary = $"--{this._Header.Boundary}";
            const string rn = "\r\n";
            const string n = "\n";

            int totalRead = 0;

            return this._StreamEnclosure.Listen((buffer, size) =>
            {
                totalRead += size;
                containerStream.Write(buffer, 0, size);

                string content = Encoding.ASCII.GetString(buffer, 0, size);

                int eofIndex = 
                    content.IndexOf(searchBoundary, StringComparison.Ordinal);
                if (eofIndex == -1)
                {
                    totalRead -= searchBoundary.Length;

                    this.AddToResidual(ref containerStream, SeekOrigin.Current, searchBoundary.Length * -1, searchBoundary.Length);

                    containerStream.Seek(searchBoundary.Length * -1, SeekOrigin.Current);

                    return true;
                }

                this.AddToResidual(ref containerStream, SeekOrigin.Current, (size - (eofIndex - 2)) * -1, size - (eofIndex - 2));

                byte[] newLineTest = new byte[2];
                this._StreamEnclosure.Read(newLineTest, 0, newLineTest.Length);

                if (string.CompareOrdinal(Encoding.ASCII.GetString(newLineTest), rn) == 0)
                    eofIndex -= 2;
                else if (string.CompareOrdinal(Encoding.ASCII.GetString(newLineTest, 1, 1), n) == 0)
                    eofIndex -= 1;

                containerStream.Seek(0, SeekOrigin.Begin);
                containerStream.SetLength(totalRead - size + eofIndex);

                return false;
            });
        }

        private void ParseApplicationXwwwFormUrlEncoded()
        {
            Stream contentStream = null;
            try
            {
                this.ReadToEndInto(ref contentStream);
                contentStream.Seek(0, SeekOrigin.Begin);

                StreamReader sR = this._Header.ContentEncoding != null 
                    ? new StreamReader(contentStream, this._Header.ContentEncoding, true) 
                    : new StreamReader(contentStream, true);

                string formContent = sR.ReadToEnd();
                formContent = formContent.Trim();

                if (string.IsNullOrEmpty(formContent))
                    return;

                string[] keyValues = formContent.Split('&');

                foreach (string keyValue in keyValues)
                {
                    int equalsIndex = keyValue.IndexOf('=');
                    string key, value = string.Empty;

                    if (equalsIndex == -1)
                        key = keyValue;
                    else
                    {
                        key = keyValue.Substring(0, equalsIndex);
                        value = keyValue.Substring(equalsIndex + 1);

                        value = System.Web.HttpUtility.UrlDecode(value);
                    }

                    if (((HttpRequestForm)this.Form).ContainsKey(key))
                        value = $"{this.Form[key]},{value}";

                    ((HttpRequestForm)this.Form).AddOrUpdate(key, value);
                }
            }
            finally
            {
                contentStream?.Close();
            }
        }

        private void CreateInputStream()
        {
            this._ContentStream = new MemoryStream();

            this.ReadToEndInto(ref this._ContentStream);
            this._ContentStream.Seek(0, SeekOrigin.Begin);
        }

        public Basics.Context.Request.IHttpRequestForm Form { get; }
        public Basics.Context.Request.IHttpRequestFile File { get; }
        public Stream ContentStream => this._ContentStream;

        internal void Dispose()
        {
            ((HttpRequestFile)this.File).Dispose();

            _ContentStream?.Close();
        }
    }
}
