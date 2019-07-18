﻿using Xeora.Web.Basics.Domain.Control;
using Xeora.Web.Basics.Domain.Control.Definitions;
using Xeora.Web.Basics.Execution;

namespace Xeora.Web.Site.Setting.Control
{
    public class LinkButton : Base, ILinkButton
    {
        public LinkButton(Bind bind, SecurityDefinition security, string text, string url, Updates updates, AttributeCollection attributes) :
            base(ControlTypes.LinkButton, bind, security)
        {
            this.Text = text;
            this.URL = url;
            this.Updates = updates;
            this.Attributes = attributes;
        }

        public string Text { get; }
        public string URL { get; }
        public Updates Updates { get; }
        public AttributeCollection Attributes { get; }

        public override IBase Clone()
        {
            Bind bind = null;

            if (base.Bind != null)
                base.Bind.Clone(out bind);

            SecurityDefinition security = null;

            if (base.Security != null)
                base.Security.Clone(out security);

            return new LinkButton(bind, security, this.Text, this.URL, this.Updates.Clone(), this.Attributes.Clone());
        }
    }
}