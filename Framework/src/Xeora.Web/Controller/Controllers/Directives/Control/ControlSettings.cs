using System.Collections.Generic;

namespace Xeora.Web.Controller.Directive.Control
{
    public class ControlSettings
    {
        public ControlSettings(Dictionary<string, object> settings)
        {
            this.Type = ControlTypes.Unknown;
            this.Security = new SecurityInfo();
            this.Bind = null;
            this.Attributes = new AttributeInfoCollection();

            // Set default values
            this.UpdateLocalBlock = true;
            this.BlockIDsToUpdate = new string[] { };
            this.DefaultButtonID = string.Empty;
            this.Text = string.Empty;
            this.URL = string.Empty;
            this.Content = string.Empty;
            this.Source = string.Empty;

            if (settings == null)
                return;

            foreach (string key in settings.Keys)
            {
                switch (key)
                {
                    case "type":
                        this.Type = (ControlTypes)settings[key];

                        break;
                    case "security":
                        if (settings[key] != null)
                        {
                            SecurityInfo securityInfo;
                            ((SecurityInfo)settings[key]).Clone(out securityInfo);

                            this.Security = securityInfo;
                        }

                        break;
                    case "bind":
                        if (settings[key] != null)
                        {
                            Basics.Execution.BindInfo bindInfo;
                            ((Basics.Execution.BindInfo)settings[key]).Clone(out bindInfo);

                            this.Bind = bindInfo;
                        }

                        break;
                    case "attributes":
                        if (settings[key] != null)
                            this.Attributes.AddRange(((AttributeInfoCollection)settings[key]).ToArray());

                        break;
                    case "blockidstoupdate.localupdate":
                        if (settings[key] != null)
                            this.UpdateLocalBlock = (bool)settings[key];
                        else
                            this.UpdateLocalBlock = true;

                        break;
                    case "blockidstoupdate":
                        if (settings[key] != null)
                            this.BlockIDsToUpdate = (string[])settings[key];

                        break;
                    case "defaultbuttonid":
                        if (settings[key] != null)
                            this.DefaultButtonID = (string)settings[key];
                        else
                            this.DefaultButtonID = string.Empty;

                        break;
                    case "text":
                        if (settings[key] != null)
                            this.Text = (string)settings[key];
                        else
                            this.Text = string.Empty;

                        break;
                    case "url":
                        if (settings[key] != null)
                            this.URL = (string)settings[key];
                        else
                            this.URL = string.Empty;

                        break;
                    case "content":
                        if (settings[key] != null)
                            this.Content = (string)settings[key];
                        else
                            this.Content = string.Empty;

                        break;
                    case "source":
                        if (settings[key] != null)
                            this.Source = (string)settings[key];
                        else
                            this.Source = string.Empty;

                        break;
                }
            }
        }

        public bool UpdateLocalBlock { get; private set; }
        public string[] BlockIDsToUpdate { get; private set; }
        public string DefaultButtonID { get; private set; }
        public string Text { get; private set; }
        public string URL { get; private set; }
        public string Content { get; private set; }
        public string Source { get; private set; }

        public ControlTypes Type { get; private set; }
        public SecurityInfo Security { get; private set; }
        public Basics.Execution.BindInfo Bind { get; private set; }
        public AttributeInfoCollection Attributes { get; private set; }

        public ControlSettings Clone()
        {
            ControlSettings settings =
                new ControlSettings(null);

            settings.Type = this.Type;

            SecurityInfo security;
            this.Security.Clone(out security);
            settings.Security = security;

            if (this.Bind != null)
            {
                Basics.Execution.BindInfo bind;
                this.Bind.Clone(out bind);
                settings.Bind = bind;
            }

            settings.Attributes.AddRange(this.Attributes);

            // Set default values
            settings.UpdateLocalBlock = this.UpdateLocalBlock;
            settings.BlockIDsToUpdate = this.BlockIDsToUpdate;
            settings.DefaultButtonID = this.DefaultButtonID;
            settings.Text = this.Text;
            settings.URL = this.URL;
            settings.Content = this.Content;
            settings.Source = this.Source;

            return settings;
        }
    }
}