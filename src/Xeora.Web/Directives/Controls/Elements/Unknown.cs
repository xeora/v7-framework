using System;
using Xeora.Web.Directives.Elements;

namespace Xeora.Web.Directives.Controls.Elements
{
    public class Unknown : IControl
    {
        private readonly Control _Parent;

        public Unknown(Control parent) =>
            this._Parent = parent;

        public bool LinkArguments => false;

        public void Parse() =>
            throw new NotSupportedException($"Unknown Custom Control Type! ControlId: {this._Parent.DirectiveId}");
    }
}