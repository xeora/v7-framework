using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace Xeora.Web.Directives.Elements
{
    public class Static : Directive
    {
        private readonly string _RawValue;

        public Static(string rawValue) :
            base(DirectiveTypes.Static, null)
        {
            this._RawValue = rawValue;
        }

        public override bool Searchable => false;
        public override bool CanAsync => true;

        public override void Parse()
        { }

        private static readonly Regex RootPathRegEx =
            new Regex("[\"']+(~|¨)/", RegexOptions.Compiled | RegexOptions.Multiline);
        public override void Render(string requesterUniqueId)
        {
            this.Parse();

            if (this.Status != RenderStatus.None)
                return;
            this.Status = RenderStatus.Rendering;

            // Change ~/ values with the exact application root path
            MatchCollection rootPathMatches = Static.RootPathRegEx.Matches(this._RawValue);
            string applicationRoot =
                Basics.Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation;
            string virtualRoot = Basics.Configurations.Xeora.Application.Main.VirtualRoot;

            StringBuilder workingValue = new StringBuilder();
            int lastIndex = 0;

            IEnumerator enumerator = rootPathMatches.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Match matchItem = (Match)enumerator.Current;

                workingValue.Append(this._RawValue.Substring(lastIndex, matchItem.Index - lastIndex));

                workingValue.AppendFormat("{0}{1}", matchItem.Value.Substring(0, 1),
                    matchItem.Value.IndexOf("~", System.StringComparison.InvariantCulture) > -1
                        ? applicationRoot
                        : virtualRoot);

                lastIndex = matchItem.Index + matchItem.Length;
            }
            workingValue.Append(this._RawValue.Substring(lastIndex));

            if (workingValue.Length > 0)
            {
                this.Deliver(RenderStatus.Rendered, workingValue.ToString());
                return;
            }

            this.Deliver(RenderStatus.Rendered, this._RawValue);
        }
    }
}