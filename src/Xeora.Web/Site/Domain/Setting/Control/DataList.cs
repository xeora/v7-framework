using Xeora.Web.Basics.Domain.Control;
using Xeora.Web.Basics.Domain.Control.Definitions;
using Xeora.Web.Basics.Execution;

namespace Xeora.Web.Site.Setting.Control
{
    public class DataList : Base, IDataList
    {
        public DataList(Bind bind, SecurityDefinition security) :
            base(ControlTypes.DataList, bind, security)
        { }

        public override IBase Clone() =>
            new DataList(base.Bind, base.Security);
    }
}