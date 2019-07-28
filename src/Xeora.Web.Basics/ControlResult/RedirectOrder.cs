using System;

namespace Xeora.Web.Basics.ControlResult
{
    [Serializable]
    public class RedirectOrder
    {
        public RedirectOrder(string location) =>
            this.Location = location;

        public string Location { get; }
    }
}
