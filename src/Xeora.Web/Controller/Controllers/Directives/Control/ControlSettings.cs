namespace Xeora.Web.Controller.Directive.Control
{
    public class ControlSettings
    {
        public ControlSettings()
        {
            this.Type = ControlTypes.Unknown;
            this.Security = new SecurityDefinition();
            this.Bind = null;
            this.Attributes = new AttributeDefinitionCollection();

            // Set default values
            this.UpdateLocalBlock = true;
            this.BlockIDsToUpdate = new string[] { };
            this.DefaultButtonID = string.Empty;
            this.Text = string.Empty;
            this.URL = string.Empty;
            this.Content = string.Empty;
            this.Source = string.Empty;
        }

        public bool UpdateLocalBlock { get; set; }
        public string[] BlockIDsToUpdate { get; set; }
        public string DefaultButtonID { get; set; }
        public string Text { get; set; }
        public string URL { get; set; }
        public string Content { get; set; }
        public string Source { get; set; }

        public ControlTypes Type { get; set; }
        public SecurityDefinition Security { get; set; }
        public Basics.Execution.Bind Bind { get; set; }
        public AttributeDefinitionCollection Attributes { get; set; }

        public ControlSettings Clone()
        {
            ControlSettings settings =
                new ControlSettings();

            settings.Type = this.Type;

            SecurityDefinition security;
            this.Security.Clone(out security);
            settings.Security = security;

            if (this.Bind != null)
            {
                Basics.Execution.Bind bind;
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