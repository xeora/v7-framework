using System.Text.RegularExpressions;
using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class PartialCache : Directive
    {
        private static readonly Regex PositionRegEx =
            new Regex("PC~(?<PositionId>\\d+)\\:\\{", RegexOptions.Compiled);

        private readonly int _PositionId;
        private readonly ContentDescription _Contents;
        private DirectiveCollection _Children;
        private bool _Parsed;

        public PartialCache(string rawValue, ArgumentCollection arguments) :
            base(DirectiveTypes.PartialCache, arguments)
        {
            this._PositionId = -1;
            Match matchMI =
                PartialCache.PositionRegEx.Match(rawValue);

            if (matchMI.Success)
                int.TryParse(matchMI.Result("${PositionId}"), out this._PositionId);

            this._Contents = new ContentDescription(rawValue);
        }

        public override bool Searchable => false;
        public override bool CanAsync => false;

        public DirectiveCollection Children => this._Children;

        public override void Parse()
        {
            if (this._Parsed)
                return;
            this._Parsed = true;

            this._Children = new DirectiveCollection(this.Mother, this);

            // PartialCache needs to link ContentArguments of its parent.
            if (this.Parent != null)
                this.Arguments.Replace(this.Parent.Arguments);

            this.Mother.RequestParsing(this._Contents.Parts[0], ref this._Children, this.Arguments);
        }

        public override void Render(string requesterUniqueId)
        {
            this.Parse();

            if (this.Status != RenderStatus.None)
                return;
            this.Status = RenderStatus.Rendering;

            Basics.Domain.IDomain instance = null;
            this.Mother.RequestInstance(ref instance);

            string cacheId =
                PartialCacheObject.CreateUniqueCacheId(this._PositionId, this, ref instance);

            PartialCachePool.Current.Get(instance.IdAccessTree, cacheId, out PartialCacheObject cacheObject);

            if (cacheObject != null)
            {
                this.Deliver(RenderStatus.Rendered, cacheObject.Content);

                return;
            }

            this.Children.Render(this.UniqueId);
            this.Deliver(RenderStatus.Rendered, this.Result);

            PartialCachePool.Current.AddOrUpdate(
                instance.IdAccessTree,
                new PartialCacheObject(cacheId, this.Result)
            );
        }
    }
}