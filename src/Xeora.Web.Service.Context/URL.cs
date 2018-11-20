using Xeora.Web.Basics;

namespace Xeora.Web.Service.Context
{
    public class URL : Basics.Context.IURL
    {
        public URL(string rawURL)
        {
            string ApplicationRootPath = 
                Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation;
            // Fix false path request
            if (string.Compare(ApplicationRootPath, "/") != 0)
            {
                string fixedRawURL = string.Format("{0}/", rawURL);

                if (string.Compare(fixedRawURL.Substring(fixedRawURL.Length - ApplicationRootPath.Length), ApplicationRootPath) == 0)
                    rawURL = fixedRawURL;
            }

            this.Raw = rawURL;

            int FirstQuestionMarkIndex = this.Raw.IndexOf('?');
            if (FirstQuestionMarkIndex > -1)
            {
                this.RelativePath = this.CleanUpDashRepetition(this.Raw.Substring(0, FirstQuestionMarkIndex));
                this.QueryString = this.Raw.Substring(FirstQuestionMarkIndex + 1);
                this.Relative = string.Format("{0}?{1}", this.RelativePath, this.QueryString);

                return;
            }

            this.RelativePath = this.CleanUpDashRepetition(this.Raw);
            this.QueryString = string.Empty;
            this.Relative = this.RelativePath;
        }

        private string CleanUpDashRepetition(string input)
        {
            int DoubleDashIndex;
            do
            {
                DoubleDashIndex = input.IndexOf("//");
                if (DoubleDashIndex > -1)
                    input = input.Remove(DoubleDashIndex, 1);
            } while (DoubleDashIndex > -1);

            return input;
        }

        public string Raw { get; private set; }
        public string Relative { get; private set; }
        public string RelativePath { get; private set; }
        public string QueryString { get; private set; }
    }
}