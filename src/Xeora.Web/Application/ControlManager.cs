using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.XPath;
using Xeora.Web.Application.Controls;
using Xeora.Web.Basics.Domain;
using Xeora.Web.Basics.Domain.Control;
using Xeora.Web.Basics.Domain.Control.Definitions;
using Xeora.Web.Basics.Execution;

namespace Xeora.Web.Application
{
    public class ControlManager : IControls
    {
        private readonly StringReader _XPathStream;
        private readonly XPathNavigator _XPathNavigator;

        internal ControlManager(string xmlContent)
        {
            if (xmlContent == null || xmlContent.Trim().Length == 0)
                throw new Exception(Global.SystemMessages.CONTROLSCONTENT + "!");

            try
            {
                // Performance Optimization
                this._XPathStream = new StringReader(xmlContent);
                XPathDocument xPathDoc = new XPathDocument(this._XPathStream);

                this._XPathNavigator = xPathDoc.CreateNavigator();
                // !--
            }
            catch (Exception)
            {
                this.Dispose();

                throw;
            }
        }

        public IBase Select(string controlId)
        {
            if (string.IsNullOrEmpty(controlId))
                return new Unknown();

            XPathNavigator xPathControlNav =
                this._XPathNavigator.SelectSingleNode($"/Controls/Control[@id='{controlId}']");

            if (xPathControlNav == null)
                return new Unknown();

            ControlTypes type = 
                ControlManager.ReadType(xPathControlNav.Clone());
            if (type == ControlTypes.Unknown)
                return new Unknown();

            Bind bind = ControlManager.ReadBind(xPathControlNav.Clone());
            SecurityDefinition security = 
                ControlManager.ReadSecurityDefinition(xPathControlNav.Clone());

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
                        ControlManager.ReadValue(xPathControlNav.Clone(), "text"),
                        ControlManager.ReadUpdates(xPathControlNav.Clone()),
                        ControlManager.ReadAttributes(xPathControlNav.Clone()));
                case ControlTypes.Checkbox:
                    return new Checkbox(bind, security,
                        ControlManager.ReadValue(xPathControlNav.Clone(), "text"),
                        ControlManager.ReadUpdates(xPathControlNav.Clone()),
                        ControlManager.ReadAttributes(xPathControlNav.Clone()));
                case ControlTypes.RadioButton:
                    return new RadioButton(bind, security,
                        ControlManager.ReadValue(xPathControlNav.Clone(), "text"),
                        ControlManager.ReadUpdates(xPathControlNav.Clone()),
                        ControlManager.ReadAttributes(xPathControlNav.Clone()));
                case ControlTypes.ImageButton:
                    return new ImageButton(bind, security,
                        ControlManager.ReadValue(xPathControlNav.Clone(), "source"),
                        ControlManager.ReadUpdates(xPathControlNav.Clone()),
                        ControlManager.ReadAttributes(xPathControlNav.Clone()));
                case ControlTypes.LinkButton:
                    return new LinkButton(bind, security,
                        ControlManager.ReadValue(xPathControlNav.Clone(), "text"),
                        ControlManager.ReadValue(xPathControlNav.Clone(), "url"),
                        ControlManager.ReadUpdates(xPathControlNav.Clone()),
                        ControlManager.ReadAttributes(xPathControlNav.Clone()));
                case ControlTypes.Textbox:
                    return new Textbox(bind, security,
                        ControlManager.ReadValue(xPathControlNav.Clone(), "text"),
                        ControlManager.ReadValue(xPathControlNav.Clone(), "defaultbuttonid"),
                        ControlManager.ReadUpdates(xPathControlNav.Clone()),
                        ControlManager.ReadAttributes(xPathControlNav.Clone()));
                case ControlTypes.Password:
                    return new Password(bind, security,
                        ControlManager.ReadValue(xPathControlNav.Clone(), "text"),
                        ControlManager.ReadValue(xPathControlNav.Clone(), "defaultbuttonid"),
                        ControlManager.ReadUpdates(xPathControlNav.Clone()),
                        ControlManager.ReadAttributes(xPathControlNav.Clone()));
                case ControlTypes.Textarea:
                    return new Textarea(bind, security,
                        ControlManager.ReadValue(xPathControlNav.Clone(), "content"),
                        ControlManager.ReadAttributes(xPathControlNav.Clone()));
            }

            return new Unknown();
        }

        private static ControlTypes ReadType(XPathNavigator xPathControlNav)
        {
            XPathNavigator target =
                xPathControlNav.SelectSingleNode("*[translate(local-name(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = 'type']");

            if (target == null || !target.MoveToFirstChild())
                return ControlTypes.Unknown;

            return Enum.TryParse(target.Value, true, out ControlTypes controlType) 
                ? controlType 
                : ControlTypes.Unknown;
        }

        private static Bind ReadBind(XPathNavigator xPathControlNav)
        {
            XPathNavigator target =
                xPathControlNav.SelectSingleNode("*[translate(local-name(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = 'bind']");

            if (target == null || !target.MoveToFirstChild())
                return null;

            return Bind.Make(target.Value);
        }

        private static AttributeCollection ReadAttributes(XPathNavigator xPathControlNav)
        {
            AttributeCollection attributes = 
                new AttributeCollection();

            XPathNavigator target =
                xPathControlNav.SelectSingleNode("*[translate(local-name(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = 'attributes']");

            if (target == null || !target.MoveToFirstChild())
                return attributes;
            do
            {
                XPathNavigator readerAttrs = target.Clone();

                string key =
                    readerAttrs.GetAttribute("key", readerAttrs.BaseURI)?.ToLower();

                if (!readerAttrs.MoveToFirstChild())
                    return attributes;

                attributes.Add(key, readerAttrs.Value);
            } while (target.MoveToNext());

            return attributes;
        }

        private static SecurityDefinition ReadSecurityDefinition(XPathNavigator xPathControlNav)
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

            do
            {
                switch (readerSecurity.Name.ToLower(CultureInfo.InvariantCulture))
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

                        if (!Enum.TryParse(
                                readerSecurity.GetAttribute("type", readerSecurity.NamespaceURI), 
                                out SecurityDefinition.DisabledDefinition.Types secType)
                            ) secType = SecurityDefinition.DisabledDefinition.Types.Inherited;

                        security.Disabled.Type = secType;
                        security.Disabled.Value = readerSecurity.Value;

                        break;
                }
            } while (readerSecurity.MoveToNext());

            return security;
        }

        private static Updates ReadUpdates(XPathNavigator xPathControlNav)
        {
            XPathNavigator target =
                xPathControlNav.SelectSingleNode("*[translate(local-name(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = 'updates']");

            if (target == null || !target.MoveToFirstChild())
                return new Updates();

            XPathNavigator readerUpdates = xPathControlNav.Clone();

            if (!readerUpdates.MoveToFirstChild())
                return new Updates();

            if (!bool.TryParse(xPathControlNav.GetAttribute("local", xPathControlNav.BaseURI), out bool local))
                local = true;

            XPathNavigator readerUpdatesBlock = xPathControlNav.Clone();
            List<string> blocks = new List<string>();

            if (!readerUpdatesBlock.MoveToFirstChild()) return new Updates(local, blocks.ToArray());
            
            do
            {
                blocks.Add(readerUpdatesBlock.Value);
            } while (readerUpdatesBlock.MoveToNext());

            return new Updates(local, blocks.ToArray());
        }

        private static string ReadValue(XPathNavigator xPathControlNav, string tag)
        {
            XPathNavigator target =
                xPathControlNav.SelectSingleNode(
                    $"*[translate(local-name(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = '{tag}']");

            if (target == null || !target.MoveToFirstChild())
                return string.Empty;

            return target.Value;
        }

        public void Dispose() => _XPathStream?.Dispose();
    }
}