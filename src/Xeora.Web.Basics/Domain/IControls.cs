using System;
using System.Xml.XPath;

namespace Xeora.Web.Basics.Domain
{
    public interface IControls : IDisposable
    {
        XPathNavigator Select(string controlID);
    }
}
