using System;

namespace Xeora.Web.Basics.ControlResult
{
    [Serializable]
    public class Conditional
    {
        public enum Conditions
        {
            False,
            True,
            Unknown
        }

        public Conditional(Conditions result) =>
            this.Result = result;

        public Conditional(bool result) =>
            this.Result = result ? Conditions.True : Conditions.False;

        public Conditions Result { get; private set; }
    }
}