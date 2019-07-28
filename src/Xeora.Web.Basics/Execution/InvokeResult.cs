using System;

namespace Xeora.Web.Basics.Execution
{
    [Serializable]
    public class InvokeResult<T>
    {
        public InvokeResult(Bind bind)
        {
            this.Bind = bind;
            this.Result = default;
            this.Exception = null;
            this.ApplicationPath = string.Empty;
        }

        public Bind Bind { get; }
        public T Result { get; set; }
        public Exception Exception { get; set; }
        public string ApplicationPath { get; set; }
    }
}
