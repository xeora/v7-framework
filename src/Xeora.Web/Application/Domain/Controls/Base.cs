using Xeora.Web.Basics.Domain.Control;
using Xeora.Web.Basics.Domain.Control.Definitions;
using Xeora.Web.Basics.Execution;

namespace Xeora.Web.Application.Domain.Controls
{
    public abstract class Base : IBase
    {
        protected Base(ControlTypes type, Bind bind, SecurityDefinition security)
        {
            this.Type = type;
            this.Bind = bind;
            this.Security = security;
        }

        public ControlTypes Type { get; }
        public Bind Bind { get; }
        public SecurityDefinition Security { get; }

        public abstract IBase Clone();
    }
}