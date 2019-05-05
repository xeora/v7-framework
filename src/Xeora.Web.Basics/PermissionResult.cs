using System;

namespace Xeora.Web.Basics
{
    [Serializable]
    public class PermissionResult
    {
        public enum Results
        {
            Allowed,
            Forbidden
        }

        public PermissionResult(Results result) =>
            this.Result = result;

        public PermissionResult(bool allowed) =>
            this.Result = allowed ? Results.Allowed : Results.Forbidden;

        public Results Result { get; private set; }
    }
}
