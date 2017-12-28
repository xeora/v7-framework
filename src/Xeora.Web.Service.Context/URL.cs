using Xeora.Web.Basics;

namespace Xeora.Web.Service.Context
{
    public class URL : Basics.Context.IURL
    {
        public URL(string rawURL)
        {
            string ApplicationRootPath = Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation;
            // Fix false path request
            if (string.Compare(ApplicationRootPath, "/") != 0 &&
                string.Compare(string.Format("{0}/", rawURL).Substring((rawURL.Length + 1) - ApplicationRootPath.Length), ApplicationRootPath) == 0)
                rawURL = string.Format("{0}/", rawURL);

            this.Raw = rawURL;

            int DoubleDashIndex = rawURL.IndexOf("//");
            if (DoubleDashIndex > -1)
                rawURL = rawURL.Remove(0, DoubleDashIndex + 2);

            int FirstSingleDashIndex = rawURL.IndexOf('/');
            if (FirstSingleDashIndex > -1)
                rawURL = rawURL.Remove(0, FirstSingleDashIndex);

            this.Relative = rawURL;

            int FirstQuestionMarkIndex = rawURL.IndexOf('?');
            if (FirstQuestionMarkIndex > -1)
            {
                this.RelativePath = rawURL.Substring(0, FirstQuestionMarkIndex);
                this.QueryString = rawURL.Substring(FirstQuestionMarkIndex + 1);

                return;
            }

            this.RelativePath = rawURL;
            this.QueryString = string.Empty;
        }

        public string Raw { get; private set; }
        public string Relative { get; private set; }
        public string RelativePath { get; private set; }
        public string QueryString { get; private set; }
    }
}