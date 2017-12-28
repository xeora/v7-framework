using System;

namespace Xeora.Web.Basics.ControlResult
{
    [Serializable()]
    public class Protection
    {
        public enum Results
        {
            None,
            ReadOnly,
            ReadWrite
        }

        public Protection(Results result) =>
            this.Result = result;

        public Results Result { get; private set; }
    }
}
