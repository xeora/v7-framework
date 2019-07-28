using System;
using Xeora.Web.Directives.Elements;

namespace Xeora.Web.Directives.Controls.Elements
{
    public class Unknown : IControl
    {
        private readonly Control _Parent;

        public Unknown(Control parent) =>
            this._Parent = parent;

        public DirectiveCollection Children => null;
        public bool LinkArguments => true;

        public void Parse()
        { }

        public void Render(string requesterUniqueId) =>
            throw new NotSupportedException($"Unknown Custom Control Type! ControlId: {this._Parent.DirectiveId}");
    }
}