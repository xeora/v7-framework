using System.Text.RegularExpressions;
using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class PartialCache : Directive
    {
        private static readonly Regex _PositionRegEx =
            new Regex("PC~(?<PositionID>\\d+)\\:\\{", RegexOptions.Compiled);

        private readonly int _PositionID;
        private readonly ContentDescription _Contents;
        private DirectiveCollection _Children;
        private bool _Parsed;

        public PartialCache(string rawValue, ArgumentCollection arguments) :
            base(DirectiveTypes.PartialCache, arguments)
        {
            this._PositionID = -1;
            Match matchMI =
                PartialCache._PositionRegEx.Match(rawValue);

            if (matchMI.Success)
                int.TryParse(matchMI.Result("${PositionID}"), out this._PositionID);

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

            this.Mother.RequestParsing(this._Contents.Parts[0], ref this._Children, this.Arguments);
        }

        public override void Render(string requesterUniqueID)
        {
            this.Parse();

            if (this.Status != RenderStatus.None)
                return;
            this.Status = RenderStatus.Rendering;

            Basics.Domain.IDomain instance = null;
            this.Mother.RequestInstance(ref instance);

            string cacheID =
                PartialCacheObject.CreateUniqueCacheID(this._PositionID, this, ref instance);

            PartialCachePool.Current.Get(instance.IDAccessTree, cacheID, out PartialCacheObject cacheObject);

            if (cacheObject != null)
            {
                this.Deliver(RenderStatus.Rendered, cacheObject.Content);

                return;
            }

            this.Children.Render(this.UniqueID);
            this.Deliver(RenderStatus.Rendered, this.Result);

            PartialCachePool.Current.AddOrUpdate(
                instance.IDAccessTree,
                new PartialCacheObject(cacheID, this.Result)
            );
        }
    }
}