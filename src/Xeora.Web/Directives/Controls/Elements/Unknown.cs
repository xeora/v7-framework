using System;
using Xeora.Web.Directives.Elements;

namespace Xeora.Web.Directives.Controls.Elements
{
    public class Unknown : IControl
    {
        private readonly Control _Parent;

        public Unknown(Control parent) =>
            this._Parent = parent;

        public bool Searchable => false;

        public void Parse()
        { }

        public void Render(string requesterUniqueID) =>
            throw new NotSupportedException(string.Format("Unknown Custom Control Type! Raw: {0}", this._Parent.DirectiveID));
    }
}