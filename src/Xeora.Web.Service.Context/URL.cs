using System;
using Xeora.Web.Basics;

namespace Xeora.Web.Service.Context
{
    public class Url : Basics.Context.IUrl
    {
        public Url(string rawUrl)
        {
            string applicationRootPath = 
                Configurations.Xeora.Application.Main.ApplicationRoot.BrowserImplementation;
            // Fix false path request
            if (string.CompareOrdinal(applicationRootPath, "/") != 0)
            {
                string fixedRawUrl = $"{rawUrl}/";

                if (string.CompareOrdinal(fixedRawUrl.Substring(fixedRawUrl.Length - applicationRootPath.Length), applicationRootPath) == 0)
                    rawUrl = fixedRawUrl;
            }

            this.Raw = rawUrl;

            int firstQuestionMarkIndex = this.Raw.IndexOf('?');
            if (firstQuestionMarkIndex > -1)
            {
                this.RelativePath = 
                    this.CleanUpDashRepetition(this.Raw.Substring(0, firstQuestionMarkIndex));
                this.QueryString = 
                    this.Raw.Substring(firstQuestionMarkIndex + 1);
                this.Relative = $"{this.RelativePath}?{this.QueryString}";

                return;
            }

            this.RelativePath = this.CleanUpDashRepetition(this.Raw);
            this.QueryString = string.Empty;
            this.Relative = this.RelativePath;
        }

        private string CleanUpDashRepetition(string input)
        {
            int doubleDashIndex;
            do
            {
                doubleDashIndex = 
                    input.IndexOf("//", StringComparison.Ordinal);
                if (doubleDashIndex > -1)
                    input = input.Remove(doubleDashIndex, 1);
            } while (doubleDashIndex > -1);

            return input;
        }

        public string Raw { get; }
        public string Relative { get; }
        public string RelativePath { get; }
        public string QueryString { get; }
    }
}