using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Xeora.Web.Service.Context
{
    public class HttpRequestBody : Basics.Context.IHttpRequestBody
    {
        private string _ContextID;

        private Basics.Context.IHttpRequestHeader _Header;
        private byte[] _Residual;
        private NetworkStream _RemoteStream;

        private Basics.Context.IHttpRequestForm _Form;
        private Basics.Context.IHttpRequestFile _File;

        private Stream _InputStream = null;

        public HttpRequestBody(string contextID, Basics.Context.IHttpRequestHeader header, byte[] residual, ref NetworkStream remoteStream)
        {
            this._ContextID = contextID;

            this._Header = header;
            this._Residual = residual;
            this._RemoteStream = remoteStream;

            this._Form = new HttpRequestForm();
            this._File = new HttpRequestFile();

            this.Parse();
        }

        private void AddToResidual(byte[] buffer, int offset, int count)
        {
            if (this._Residual == null)
            {
                this._Residual = new byte[count];
                Array.Copy(buffer, offset, this._Residual, 0, this._Residual.Length);

                return;
            }

            byte[] newResidual = new byte[this._Residual.Length + count];
            Array.Copy(buffer, offset, newResidual, 0, count);
            Array.Copy(this._Residual, 0, newResidual, count, this._Residual.Length);

            this._Residual = newResidual;
        }

        private int Read(byte[] buffer, int offset, int count)
        {
            int totalRead = 0;

            if (this._Residual != null)
            {
                totalRead = this._Residual.Length;
                if (totalRead > count)
                    totalRead = count;

                Array.Copy(this._Residual, 0, buffer, offset, totalRead);

                count -= totalRead;
                offset += totalRead;

                // Truncate Residual
                int newResidualLength = this._Residual.Length - totalRead;
                if (newResidualLength == 0)
                    this._Residual = null;
                else
                {
                    byte[] newResidual = new byte[newResidualLength];
                    Array.Copy(this._Residual, totalRead, newResidual, 0, newResidual.Length);
                    this._Residual = newResidual;
                }

                if (count == 0)
                    return totalRead;
            }

            if (this._RemoteStream.DataAvailable)
                totalRead += this._RemoteStream.Read(buffer, offset, count);

            return totalRead;
        }

        private void ReadToEndInto(ref Stream contentStream)
        {
            byte[] buffer = new byte[1024];
            int bR = 0;

            if (contentStream == null)
                contentStream = new MemoryStream();

            do
            {
                bR = this.Read(buffer, 0, buffer.Length);

                if (bR > 0)
                    contentStream.Write(buffer, 0, bR);
            } while (bR > 0);
        }

        private void Parse()
        {
            switch (this._Header.ContentType)
            {
                case "multipart/form-data":
                    this.ParseMultipartFormData();

                    break;
                case "application/x-www-form-urlencoded":
                    this.ParseApplicationXWWWFormURLEncoded();

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
            string SearchBoundary = string.Format("--{0}", this._Header.Boundary);

            byte[] buffer = new byte[SearchBoundary.Length];
            string content = string.Empty;

            Stream contentStream = null;
            try
            {
                contentStream = new MemoryStream();
                do
                {
                    int bR = this.Read(buffer, 0, buffer.Length);

                    if (bR == 0)
                        break;

                    contentStream.Write(buffer, 0, bR);
                    content += Encoding.ASCII.GetString(buffer, 0, bR);

                    int EOFIndex = content.IndexOf(SearchBoundary);
                    if (EOFIndex == -1)
                        continue;

                    byte[] contentBytes = new byte[content.Length - EOFIndex];
                    contentStream.Seek(EOFIndex, SeekOrigin.Begin);
                    contentStream.Read(contentBytes, 0, contentBytes.Length);
                    this.AddToResidual(contentBytes, 0, contentBytes.Length);

                    return true;
                } while (true);
            }
            finally
            {
                if (contentStream != null)
                {
                    contentStream.Close();
                    GC.SuppressFinalize(contentStream);
                }
            }

            return false;
        }

        private ContentHeader ReadContentHeader()
        {
            string RNRN = "\r\n\r\n";
            string NN = "\n\n";
            int NL;

            byte[] buffer = new byte[1024];
            string content = string.Empty;

            Stream contentStream = null;
            try
            {
                contentStream = new MemoryStream();
                do
                {
                    int bR = this.Read(buffer, 0, buffer.Length);

                    if (bR == 0)
                        break;

                    contentStream.Write(buffer, 0, bR);
                    content += Encoding.ASCII.GetString(buffer, 0, bR);

                    NL = 4;
                    int EOFIndex = content.IndexOf(RNRN);
                    if (EOFIndex == -1)
                    {
                        NL = 2;
                        EOFIndex = content.IndexOf(NN);
                    }
                    if (EOFIndex == -1)
                        continue;

                    EOFIndex += NL;

                    byte[] contentBytes = new byte[content.Length - EOFIndex];
                    contentStream.Seek(EOFIndex, SeekOrigin.Begin);
                    contentStream.Read(contentBytes, 0, contentBytes.Length);
                    this.AddToResidual(contentBytes, 0, contentBytes.Length);

                    return this.ParseContentHeader(content.Substring(0, EOFIndex));
                } while (true);
            }
            finally
            {
                if (contentStream != null)
                {
                    contentStream.Close();
                    GC.SuppressFinalize(contentStream);
                }
            }

            return default(ContentHeader);
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

                if (line.IndexOf(this._Header.Boundary) > -1)
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

                    if (value.StartsWith("\""))
                        value = value.Substring(1);

                    if (value.EndsWith("\""))
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

                    if (this.ReadContentBody(ref contentStream))
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
                    if (sR != null)
                    {
                        sR.Close();
                        GC.SuppressFinalize(sR);
                    }

                    if (contentStream != null)
                    {
                        contentStream.Close();
                        GC.SuppressFinalize(contentStream);
                    }
                }

                return;
            }

            // if the request size smaller than 3,5 MB, use the memory.
            HttpRequestFileInfo requestFI =
                new HttpRequestFileInfo(this._ContextID, contentHeader.ContentType, contentHeader.ContentEncoding, contentHeader.FileName, this._Header.ContentLength < 3670016);
            if (this.ReadContentBody(ref requestFI._ContentStream))
                ((HttpRequestFile)this._File).AddOrUpdate(contentHeader.Name, requestFI);
        }

        private bool ReadContentBody(ref Stream containerStream)
        {
            string SearchBoundary = string.Format("--{0}", this._Header.Boundary);
            string RN = "\r\n";
            string N = "\n";

            byte[] buffer = new byte[131072];
            byte[] contentBytes;

            int totalRead = 0;

            do
            {
                int bR = this.Read(buffer, 0, buffer.Length);

                if (bR == 0)
                    break;

                totalRead += bR;
                containerStream.Write(buffer, 0, bR);

                string content = Encoding.ASCII.GetString(buffer, 0, bR);

                int EOFIndex = content.IndexOf(SearchBoundary);
                if (EOFIndex == -1)
                {
                    totalRead -= SearchBoundary.Length;

                    containerStream.Seek(SearchBoundary.Length * -1, SeekOrigin.Current);

                    contentBytes = new byte[SearchBoundary.Length];
                    containerStream.Read(contentBytes, 0, contentBytes.Length);
                    this.AddToResidual(contentBytes, 0, contentBytes.Length);

                    containerStream.Seek(SearchBoundary.Length * -1, SeekOrigin.Current);

                    continue;
                }

                containerStream.Seek((bR - (EOFIndex - 2)) * -1, SeekOrigin.Current);

                contentBytes = new byte[(bR - (EOFIndex - 2))];
                containerStream.Read(contentBytes, 0, contentBytes.Length);
                this.AddToResidual(contentBytes, 0, contentBytes.Length);

                byte[] newLineTest = new byte[2];
                this.Read(newLineTest, 0, newLineTest.Length);

                if (string.Compare(Encoding.ASCII.GetString(newLineTest), RN) == 0)
                    EOFIndex -= 2;
                else if (string.Compare(Encoding.ASCII.GetString(newLineTest, 1, 1), N) == 0)
                    EOFIndex -= 1;

                containerStream.Seek(0, SeekOrigin.Begin);
                containerStream.SetLength((totalRead - bR) + EOFIndex);

                return true;
            } while (true);

            return false;
        }

        private void ParseApplicationXWWWFormURLEncoded()
        {
            Stream contentStream = null;
            StreamReader sR = null;

            try
            {
                this.ReadToEndInto(ref contentStream);
                contentStream.Seek(0, SeekOrigin.Begin);

                if (this._Header.ContentEncoding != null)
                    sR = new StreamReader(contentStream, this._Header.ContentEncoding, true);
                else
                    sR = new StreamReader(contentStream, true);

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

                    if (((HttpRequestForm)this._Form).ContainsKey(key))
                        value = string.Format("{0},{1}", this._Form[key], value);

                    ((HttpRequestForm)this._Form).AddOrUpdate(key, value);
                }
            }
            finally
            {
                if (contentStream != null)
                {
                    contentStream.Close();
                    GC.SuppressFinalize(contentStream);
                }
            }
        }

        private void CreateInputStream()
        {
            this._InputStream = new MemoryStream();

            this.ReadToEndInto(ref this._InputStream);
            this._InputStream.Seek(0, SeekOrigin.Begin);
        }

        public Basics.Context.IHttpRequestForm Form => this._Form;
        public Basics.Context.IHttpRequestFile File => this._File;

        internal void Dispose()
        {
            ((HttpRequestFile)this._File).Dispose();

            if (this._InputStream != null)
            {
                this._InputStream.Close();
                GC.SuppressFinalize(this._InputStream);
            }
        }
    }
}
