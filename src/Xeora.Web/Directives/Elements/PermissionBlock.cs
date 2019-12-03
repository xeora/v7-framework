using Xeora.Web.Basics;
using Xeora.Web.Basics.Domain;
using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class PermissionBlock : Directive, INameable
    {
        private enum ContentTypes
        {
            None,
            Allowed,
            Forbidden
        }

        private ContentTypes _SelectedContent = ContentTypes.None;

        private readonly ContentDescription _Contents;
        private bool _HasStatementContent;
        private bool _Parsed;

        public PermissionBlock(string rawValue, ArgumentCollection arguments) :
            base(DirectiveTypes.PermissionBlock, arguments)
        {
            this.DirectiveId = DirectiveHelper.CaptureDirectiveId(rawValue);
            this._Contents = new ContentDescription(rawValue);
        }

        public string DirectiveId { get; }

        public override bool Searchable => true;
        public override bool CanAsync => false;

        public override void Parse()
        {
            if (this._SelectedContent == ContentTypes.None)
                return;

            if (this._Parsed)
                return;
            this._Parsed = true;

            string statementContent = string.Empty;
            switch (this._SelectedContent)
            {
                case ContentTypes.Allowed:
                    statementContent = this._Contents.Parts[0];

                    break;
                case ContentTypes.Forbidden:
                    statementContent = this._Contents.MessageTemplate;

                    break;
            }
            
            if (string.IsNullOrEmpty(statementContent))
                return;
            this._HasStatementContent = true;
            
            // PermissionBlock needs to link ContentArguments of its parent.
            if (this.Parent != null)
                this.Arguments.Replace(this.Parent.Arguments);

            this.Children = new DirectiveCollection(this.Mother, this);
            this.Mother.RequestParsing(statementContent, this.Children, this.Arguments);
        }

        public override bool PreRender()
        {
            if (this.Status != RenderStatus.None)
                return false;
            this.Status = RenderStatus.Rendering;

            PermissionResult permissionResult = 
                this.EnsurePermission();

            this._SelectedContent = 
                permissionResult.Result == PermissionResult.Results.Allowed ? ContentTypes.Allowed : ContentTypes.Forbidden;
            this.Parse();
            
            return this._HasStatementContent;
        }

        public override void PostRender()
        {
            this.Deliver(RenderStatus.Rendered, this.Result);
            this.Scheduler.Fire();
        }

        private PermissionResult EnsurePermission()
        {
            this.Mother.RequestInstance(out IDomain instance);

            if (string.IsNullOrEmpty(instance.Settings.Configurations.SecurityExecutable))
                return new PermissionResult(PermissionResult.Results.Forbidden);

            Basics.Execution.Bind permissionBind =
                Basics.Execution.Bind.Make($"{instance.Settings.Configurations.SecurityExecutable}?EnsurePermission,p1");
            permissionBind.Parameters.Prepare(parameter => this.DirectiveId);
            permissionBind.InstanceExecution = true;

            Basics.Execution.InvokeResult<PermissionResult> permissionInvokeResult =
                Manager.Executer.InvokeBind<PermissionResult>(Helpers.Context.Request.Header.Method, permissionBind, Manager.ExecuterTypes.Undefined);

            if (permissionInvokeResult.Result == null || permissionInvokeResult.Exception != null)
                return new PermissionResult(PermissionResult.Results.Forbidden);

            return permissionInvokeResult.Result;
        }
    }
}