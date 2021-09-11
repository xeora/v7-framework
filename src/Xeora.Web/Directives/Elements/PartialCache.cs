using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class PartialCache : Directive
    {
        private readonly int _PositionId;
        private readonly string[] _Parameters;
        private readonly string[] _CacheIdExtensions;
        private readonly ContentDescription _Contents;
        private bool _Parsed;

        public PartialCache(string rawValue, int positionId, ArgumentCollection arguments) :
            base(DirectiveTypes.PartialCache, arguments)
        {
            this._PositionId = positionId;
            this._Parameters =
                DirectiveHelper.CaptureDirectiveParameters(rawValue, true);
            this._CacheIdExtensions = new string[this._Parameters.Length];
            this._Contents = new ContentDescription(rawValue);
        }

        public override bool Searchable => false;
        public override bool CanAsync => false;
        public override bool CanHoldVariable => false;

        public override void Parse()
        {
            if (this._Parsed)
                return;
            this._Parsed = true;

            // PartialCache needs to link ContentArguments of its parent.
            if (this.Parent != null)
                this.Arguments.Replace(this.Parent.Arguments);

            for (int i = 0; i < this._Parameters.Length; i++)
                this._CacheIdExtensions[i] =
                    Xeora.Web.Directives.Property.Render(this.Parent, this._Parameters[i]).Item2?.ToString();

            this.Children = new DirectiveCollection(this.Mother, this);
            this.Mother.RequestParsing(this._Contents.Parts[0], this.Children, this.Arguments);
        }

        private string[] _InstanceIdAccessTree;
        private string _CacheId;
        public override bool PreRender()
        {
            if (this.Status != RenderStatus.None)
                return false;
            this.Status = RenderStatus.Rendering;
            
            this.Parse();
            
            this.Mother.RequestInstance(out Basics.Domain.IDomain instance);
            this._InstanceIdAccessTree = instance.IdAccessTree;
            this._CacheId =
                PartialCacheObject.CreateUniqueCacheId(this._PositionId, string.Join('_', this._CacheIdExtensions), this, ref instance);
            PartialCachePool.Current.Get(this._InstanceIdAccessTree, this._CacheId, out PartialCacheObject cacheObject);

            if (cacheObject == null) return true;
            
            this.Deliver(RenderStatus.Rendered, cacheObject.Content);
            return false;
        }

        public override void PostRender()
        {
            this.Deliver(RenderStatus.Rendered, this.Result);

            PartialCachePool.Current.AddOrUpdate(
                this._InstanceIdAccessTree,
                new PartialCacheObject(this._CacheId, this.Result)
            );
        }
    }
}