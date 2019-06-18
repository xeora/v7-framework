using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class ControlAsync : Control
    {
        public ControlAsync(string rawValue, ArgumentCollection arguments) : 
            base(rawValue, arguments)
        { }

        public override bool CanAsync => true;
    }
}