using System;
using Xeora.Web.Basics.Domain.Control.Definitions;

namespace Xeora.Web.Basics.Domain
{
    public interface IControls : IDisposable
    {
        IBase Select(string controlId);
    }
}
