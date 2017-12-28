using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace Xeora.Web.Controller.Directive
{
    public class Renderless : Controller
    {
        public Renderless(int rawStartIndex, string rawValue, Global.ArgumentInfoCollection contentArguments) :
            base(rawStartIndex, rawValue, ControllerTypes.Renderless, contentArguments)
        { }

        private static Regex _RootPathRegEx =
            new Regex("[\"']+(~|¨)/", RegexOptions.Compiled | RegexOptions.Multiline);
        public override void Render(string requesterUniqueID)
        {
            if (this.IsRendered)
                return;

            // Change ~/ values with the exact application root path
            MatchCollection rootPathMatches = Renderless._RootPathRegEx.Matches(this.RawValue);
            string applicationRoot =
                Basics.Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation;
            string virtualRoot = Basics.Configurations.Xeora.Application.Main.VirtualRoot;

            StringBuilder workingValue = new StringBuilder();
            int lastIndex = 0;

            IEnumerator enumerator = rootPathMatches.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Match matchItem = (Match)enumerator.Current;

                workingValue.Append(this.RawValue.Substring(lastIndex, matchItem.Index - lastIndex));

                if (matchItem.Value.IndexOf("~") > -1)
                    workingValue.AppendFormat("{0}{1}", matchItem.Value.Substring(0, 1), applicationRoot);
                else
                    workingValue.AppendFormat("{0}{1}", matchItem.Value.Substring(0, 1), virtualRoot);

                lastIndex = matchItem.Index + matchItem.Length;
            }
            workingValue.Append(this.RawValue.Substring(lastIndex));

            if (workingValue.Length > 0)
            {
                this.RenderedValue = workingValue.ToString();

                return;
            }

            this.RenderedValue = this.RawValue;
        }
    }
}