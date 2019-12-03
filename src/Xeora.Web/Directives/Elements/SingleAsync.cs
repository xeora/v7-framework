using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class SingleAsync : Single
    {
        public SingleAsync(string rawValue, ArgumentCollection arguments) :
            base(rawValue, arguments)
        { }
        
        public override bool CanAsync => true;
    }
}