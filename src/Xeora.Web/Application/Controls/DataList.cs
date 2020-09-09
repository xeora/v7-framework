using Xeora.Web.Basics.Domain.Control;
using Xeora.Web.Basics.Domain.Control.Definitions;
using Xeora.Web.Basics.Execution;

namespace Xeora.Web.Application.Controls
{
    public class DataList : Base, IDataList
    {
        public DataList(Bind bind, SecurityDefinition security) :
            base(ControlTypes.DataList, bind, security)
        { }

        public override IBase Clone()
        {
            Bind bind = null;
            Bind?.Clone(out bind);

            SecurityDefinition security = null;
            Security?.Clone(out security);

            return new DataList(bind, security);
        }
    }
}