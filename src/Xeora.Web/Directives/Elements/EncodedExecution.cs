using System;
using System.Collections.Generic;
using System.IO;
using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class EncodedExecution : Directive
    {
        private readonly ContentDescription _Contents;
        private bool _Parsed;

        private bool _Clean;

        public EncodedExecution(string rawValue, ArgumentCollection arguments) :
            base(DirectiveTypes.EncodedExecution, arguments)
        {
            this._Contents = new ContentDescription(rawValue);
            this._Clean = false;
        }

        public override bool Searchable => false;
        public override bool CanAsync => false;
        public override bool CanHoldVariable => false;

        public override void Parse()
        {
            if (this._Parsed)
                return;
            this._Parsed = true;
            
            // EncodedExecution needs to link ContentArguments of its parent.
            if (this.Parent != null)
                this.Arguments.Replace(this.Parent.Arguments);

            string executionContent = this._Contents.Parts[0];
            if (string.IsNullOrEmpty(executionContent))
                throw new Exceptions.EmptyBlockException();

            this.Children = new DirectiveCollection(this.Mother, this);
            this.Mother.RequestParsing(executionContent, this.Children, this.Arguments);
        }

        public override bool PreRender()
        {
            if (this.Status != RenderStatus.None)
                return false;
            this.Status = RenderStatus.Rendering;

            this.Parse();
            
            return true;
        }

        public override void PostRender()
        {
            string result = this.Result;

            this.ExtractSubDirectives(ref result);

            Basics.ICryptography cryptography =
                CryptographyProvider.Current.Get(Basics.Helpers.Context.Session.SessionId);
            
            if (this._Clean)
            {
                this.Deliver(
                    RenderStatus.Rendered, 
                    cryptography.Encrypt(result)
                );

                return;
            }

            if (this.UpdateBlockIds.Count > 0)
            {
                this.Deliver(
                    RenderStatus.Rendered,
                    string.Format(
                        "javascript:__XeoraJS.update('{0}', '{1}');",
                        string.Join(">", this.UpdateBlockIds.ToArray()),
                        cryptography.Encrypt(this.Result)
                    )
                );

                return;
            }

            this.Deliver( 
                RenderStatus.Rendered,
                string.Format(
                    "javascript:__XeoraJS.post('{0}');",
                    cryptography.Encrypt(this.Result)
                )
            );
        }

        private void ExtractSubDirectives(ref string blockContent)
        {
            Dictionary<string, System.Func<string, string>> subDirectives =
                new Dictionary<string, System.Func<string, string>>() {
                    {
                        "!CLEAN",
                        d =>
                        {
                            this._Clean = true;
                            return d.Replace("!CLEAN", string.Empty);
                        }
                    }
                };

            // Sub Directive Test
            if (blockContent.IndexOf('!') == 0)
            {
                string directives = string.Empty;
                StringReader sR = null;
                try
                {
                    sR = new StringReader(blockContent);
                    directives = sR.ReadLine();

                    blockContent = sR.ReadToEnd();
                }
                catch (Exceptions.GrammarException)
                {
                    throw;
                }
                catch (Exception)
                {
                    // Just Handle Exceptions
                }
                finally
                {
                    sR?.Close();
                    
                    if (string.IsNullOrEmpty(directives)) directives = string.Empty;
                }

                foreach (string key in subDirectives.Keys)
                {
                    int dIdx = 
                        directives.IndexOf(key, StringComparison.Ordinal);

                    if (dIdx == -1)
                        continue;

                    directives = subDirectives[key].Invoke(directives);
                }

                blockContent = $"{directives}{blockContent}";
            }
            blockContent = blockContent.Trim();
        }
    }
}
