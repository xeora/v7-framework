using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.XPath;
using Xeora.Web.Basics.Domain;
using Xeora.Web.Basics.Domain.Control;
using Xeora.Web.Basics.Domain.Control.Definitions;
using Xeora.Web.Basics.Execution;
using Xeora.Web.Site.Setting.Control;

namespace Xeora.Web.Site.Setting
{
    public class Controls : IControls
    {
        private readonly StringReader _XPathStream;
        private readonly XPathNavigator _XPathNavigator;

        internal Controls(string xmlContent)
        {
            if (xmlContent == null || xmlContent.Trim().Length == 0)
                throw new System.Exception(Global.SystemMessages.CONTROLSCONTENT + "!");

            try
            {
                // Performance Optimization
                this._XPathStream = new StringReader(xmlContent);
                XPathDocument xPathDoc = new XPathDocument(this._XPathStream);

                this._XPathNavigator = xPathDoc.CreateNavigator();
                // !--
            }
            catch (System.Exception)
            {
                this.Dispose();

                throw;
            }
        }

        public IBase Select(string controlID)
        {
            if (string.IsNullOrEmpty(controlID))
                return new Unknown();

            XPathNavigator xPathControlNav =
                this._XPathNavigator.SelectSingleNode(string.Format("/Controls/Control[@id='{0}']", controlID));

            if (xPathControlNav == null)
                return new Unknown();

            ControlTypes type = 
                this.ReadType(xPathControlNav.Clone());
            if (type == ControlTypes.Unknown)
                return new Unknown();

            Bind bind = this.ReadBind(xPathControlNav.Clone());
            SecurityDefinition security = 
                this.ReadSecurityDefinition(xPathControlNav.Clone());

            switch (type)
            {
                case ControlTypes.ConditionalStatement:
                    return new ConditionalStatement(bind, security);
                case ControlTypes.DataList:
                    return new DataList(bind, security);
                case ControlTypes.VariableBlock:
                    return new VariableBlock(bind, security);
                case ControlTypes.Button:
                    return new Button(bind, security,
                        this.ReadValue(xPathControlNav.Clone(), "text"),
                        this.ReadUpdates(xPathControlNav.Clone()),
                        this.ReadAttributes(xPathControlNav.Clone()));
                case ControlTypes.Checkbox:
                    return new Checkbox(bind, security,
                        this.ReadValue(xPathControlNav.Clone(), "text"),
                        this.ReadUpdates(xPathControlNav.Clone()),
                        this.ReadAttributes(xPathControlNav.Clone()));
                case ControlTypes.RadioButton:
                    return new RadioButton(bind, security,
                        this.ReadValue(xPathControlNav.Clone(), "text"),
                        this.ReadUpdates(xPathControlNav.Clone()),
                        this.ReadAttributes(xPathControlNav.Clone()));
                case ControlTypes.ImageButton:
                    return new ImageButton(bind, security,
                        this.ReadValue(xPathControlNav.Clone(), "source"),
                        this.ReadUpdates(xPathControlNav.Clone()),
                        this.ReadAttributes(xPathControlNav.Clone()));
                case ControlTypes.LinkButton:
                    return new LinkButton(bind, security,
                        this.ReadValue(xPathControlNav.Clone(), "text"),
                        this.ReadValue(xPathControlNav.Clone(), "url"),
                        this.ReadUpdates(xPathControlNav.Clone()),
                        this.ReadAttributes(xPathControlNav.Clone()));
                case ControlTypes.Textbox:
                    return new Textbox(bind, security,
                        this.ReadValue(xPathControlNav.Clone(), "text"),
                        this.ReadValue(xPathControlNav.Clone(), "defaultbuttonid"),
                        this.ReadUpdates(xPathControlNav.Clone()),
                        this.ReadAttributes(xPathControlNav.Clone()));
                case ControlTypes.Password:
                    return new Password(bind, security,
                        this.ReadValue(xPathControlNav.Clone(), "text"),
                        this.ReadValue(xPathControlNav.Clone(), "defaultbuttonid"),
                        this.ReadUpdates(xPathControlNav.Clone()),
                        this.ReadAttributes(xPathControlNav.Clone()));
                case ControlTypes.Textarea:
                    return new Textarea(bind, security,
                        this.ReadValue(xPathControlNav.Clone(), "content"),
                        this.ReadAttributes(xPathControlNav.Clone()));
            }

            return new Unknown();
        }

        private ControlTypes ReadType(XPathNavigator xPathControlNav)
        {
            XPathNavigator target =
                xPathControlNav.SelectSingleNode("*[translate(local-name(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = 'type']");

            if (target == null || !target.MoveToFirstChild())
                return ControlTypes.Unknown;

            if (Enum.TryParse(target.Value, true, out ControlTypes controlType))
                return controlType;

            return ControlTypes.Unknown;
        }

        private Bind ReadBind(XPathNavigator xPathControlNav)
        {
            XPathNavigator target =
                xPathControlNav.SelectSingleNode("*[translate(local-name(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = 'bind']");

            if (target == null || !target.MoveToFirstChild())
                return null;

            return Bind.Make(target.Value);
        }

        private AttributeCollection ReadAttributes(XPathNavigator xPathControlNav)
        {
            AttributeCollection attributes = 
                new AttributeCollection();

            XPathNavigator target =
                xPathControlNav.SelectSingleNode("*[translate(local-name(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = 'attributes']");

            if (target == null || !target.MoveToFirstChild())
                return attributes;

            XPathNavigator readerAttrs = xPathControlNav.Clone();

            if (!readerAttrs.MoveToFirstChild())
                return attributes;

            do
            {
                attributes.Add(
                    readerAttrs.GetAttribute("key", readerAttrs.BaseURI).ToLower(),
                    readerAttrs.Value
                );
            } while (readerAttrs.MoveToNext());

            return attributes;
        }

        private SecurityDefinition ReadSecurityDefinition(XPathNavigator xPathControlNav)
        {
            SecurityDefinition security =
                new SecurityDefinition();

            XPathNavigator target =
                xPathControlNav.SelectSingleNode("*[translate(local-name(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = 'security']");

            if (target == null || !target.MoveToFirstChild())
                return security;

            XPathNavigator readerSecurity = xPathControlNav.Clone();

            if (!readerSecurity.MoveToFirstChild())
                return security;

            security = new SecurityDefinition();
            CultureInfo compareCulture =
                new CultureInfo("en-US");

            do
            {
                switch (readerSecurity.Name.ToLower(compareCulture))
                {
                    case "registeredgroup":
                        security.RegisteredGroup = readerSecurity.Value;

                        break;
                    case "friendlyname":
                        security.FriendlyName = readerSecurity.Value;

                        break;
                    case "bind":
                        security.Bind = Bind.Make(readerSecurity.Value);

                        break;
                    case "disabled":
                        security.Disabled.Set = true;

                        if (!Enum.TryParse<SecurityDefinition.DisabledDefinition.Types>(
                                readerSecurity.GetAttribute("type", readerSecurity.NamespaceURI), 
                                out SecurityDefinition.DisabledDefinition.Types secType)
                            )
                            secType = SecurityDefinition.DisabledDefinition.Types.Inherited;

                        security.Disabled.Type = secType;
                        security.Disabled.Value = readerSecurity.Value;

                        break;
                }
            } while (readerSecurity.MoveToNext());

            return security;
        }

        private Updates ReadUpdates(XPathNavigator xPathControlNav)
        {
            XPathNavigator target =
                xPathControlNav.SelectSingleNode("*[translate(local-name(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = 'updates']");

            if (target == null || !target.MoveToFirstChild())
                return null;

            XPathNavigator readerUpdates = xPathControlNav.Clone();

            if (!readerUpdates.MoveToFirstChild())
                return null;

            if (!bool.TryParse(xPathControlNav.GetAttribute("local", xPathControlNav.BaseURI), out bool local))
                local = true;

            XPathNavigator readerUpdates_blck = xPathControlNav.Clone();
            List<string> blocks = new List<string>();

            if (readerUpdates_blck.MoveToFirstChild())
            {
                do
                {
                    blocks.Add(readerUpdates_blck.Value);
                } while (readerUpdates_blck.MoveToNext());
            }

            return new Updates(local, blocks.ToArray());
        }

        private string ReadValue(XPathNavigator xPathControlNav, string tag)
        {
            XPathNavigator target =
                xPathControlNav.SelectSingleNode(string.Format("*[translate(local-name(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = '{0}']", tag));

            if (target == null || !target.MoveToFirstChild())
                return string.Empty;

            return target.Value;
        }

        public void Dispose()
        {
            if (this._XPathStream != null)
            {
                this._XPathStream.Close();
                GC.SuppressFinalize(this._XPathStream);
            }
            GC.SuppressFinalize(this);
        }
    }
}