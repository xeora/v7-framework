namespace Xeora.Web.Controller.Directive.Control
{
    public class SecurityDefinition
    {
        private bool _Set;
        private string _FriendlyName;

        public SecurityDefinition()
        {
            this._Set = false;
            this.RegisteredGroup = string.Empty;
            this._FriendlyName = string.Empty;
            this.Bind = null;
            this.Disabled = new DisabledDefinition();
        }

        public bool Set
        {
            get { return this._Set; }
            set
            {
                this._Set = value;

                if (this._Set)
                {
                    if (string.IsNullOrEmpty(this._FriendlyName))
                        this._FriendlyName = "Unknown";
                }
            }
        }

        public string RegisteredGroup { get; set; }

        public string FriendlyName
        {
            get { return this._FriendlyName; }
            set
            {
                this._FriendlyName = value;

                if (string.IsNullOrEmpty(this._FriendlyName))
                    this._FriendlyName = "Unknown";

                this._Set = true;
            }
        }

        public Basics.Execution.Bind Bind { get; set; }
        public DisabledDefinition Disabled { get; private set; }

        public void Clone(out SecurityDefinition security)
        {
            security = new SecurityDefinition
            {
                _Set = this._Set,
                RegisteredGroup = this.RegisteredGroup,
                _FriendlyName = this._FriendlyName
            };

            if (this.Bind != null)
            {
                this.Bind.Clone(out Basics.Execution.Bind bind);
                security.Bind = bind;
            }

            security.Disabled = this.Disabled;
        }

        public class DisabledDefinition
        {
            public enum Types
            {
                Inherited,
                Dynamic
            }

            public DisabledDefinition()
            {
                this.Set = false;
                this.Type = Types.Inherited;
                this.Value = string.Empty;
            }

            public bool Set { get; set; }
            public Types Type { get; set; }
            public string Value { get; set; }
        }
    }
}