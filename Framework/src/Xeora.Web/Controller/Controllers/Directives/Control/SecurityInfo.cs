namespace Xeora.Web.Controller.Directive.Control
{
    public class SecurityInfo
    {
        private bool _SecuritySet;
        private string _FriendlyName;

        public SecurityInfo()
        {
            this._SecuritySet = false;
            this.RegisteredGroup = string.Empty;
            this._FriendlyName = string.Empty;
            this.Bind = null;
            this.Disabled = new DisabledClass();
        }

        public bool SecuritySet
        {
            get { return this._SecuritySet; }
            set
            {
                this._SecuritySet = value;

                if (this._SecuritySet)
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

                this._SecuritySet = true;
            }
        }

        public Basics.Execution.BindInfo Bind { get; set; }
        public DisabledClass Disabled { get; private set; }

        public void Clone(out SecurityInfo security)
        {
            security = new SecurityInfo();

            security._SecuritySet = this._SecuritySet;
            security.RegisteredGroup = this.RegisteredGroup;
            security._FriendlyName = this._FriendlyName;

            if (this.Bind != null)
            {
                Basics.Execution.BindInfo bindInfo;
                this.Bind.Clone(out bindInfo);
                security.Bind = bindInfo;
            }

            security.Disabled = this.Disabled;
        }

        public class DisabledClass
        {
            public enum DisabledTypes
            {
                Inherited,
                Dynamic
            }

            public DisabledClass()
            {
                this.IsSet = false;
                this.Type = DisabledTypes.Inherited;
                this.Value = string.Empty;
            }

            public bool IsSet { get; set; }
            public DisabledTypes Type { get; set; }
            public string Value { get; set; }
        }
    }
}