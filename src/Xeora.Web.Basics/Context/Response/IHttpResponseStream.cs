using System;

namespace Xeora.Web.Basics.Context.Response
{
    public interface IHttpResponseStream : IDisposable
    {
        void Write(byte[] buffer, int offset, int count);
    }
}
