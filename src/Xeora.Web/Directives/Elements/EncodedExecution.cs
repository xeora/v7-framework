using System;
using System.Collections.Generic;
using System.IO;
using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class EncodedExecution : Directive, IHasChildren
    {
        private readonly ContentDescription _Contents;
        private DirectiveCollection _Children;
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

        public DirectiveCollection Children => this._Children;

        public override void Parse()
        {
            if (this._Parsed)
                return;
            this._Parsed = true;

            this._Children = new DirectiveCollection(this.Mother, this);

            // EncodedExecution needs to link ContentArguments of its parent.
            if (this.Parent != null)
                this.Arguments.Replace(this.Parent.Arguments);

            string executionContent = this._Contents.Parts[0];
            if (string.IsNullOrEmpty(executionContent))
                throw new Exception.EmptyBlockException();

            this.Mother.RequestParsing(executionContent, ref this._Children, this.Arguments);
        }

        public override void Render(string requesterUniqueId)
        {
            this.Parse();

            if (this.Status != RenderStatus.None)
                return;
            this.Status = RenderStatus.Rendering;

            this.Children.Render(this.UniqueId);

            string result = this.Result;

            this.ExtractSubDirectives(ref result);

            if (this._Clean)
            {
                this.Deliver(
                    RenderStatus.Rendered, 
                    Manager.AssemblyCore.EncodeFunction(Basics.Helpers.Context.HashCode, result)
                );

                return;
            }

            if (this.Mother.UpdateBlockIdStack.Count > 0)
            {
                this.Deliver(
                    RenderStatus.Rendered,
                    string.Format(
                        "javascript:__XeoraJS.update('{0}', '{1}');",
                        this.Mother.UpdateBlockIdStack.Peek(),
                        Manager.AssemblyCore.EncodeFunction(Basics.Helpers.Context.HashCode, this.Result)
                    )
                );

                return;
            }

            this.Deliver( 
                RenderStatus.Rendered,
                string.Format(
                    "javascript:__XeoraJS.post('{0}');",
                    Manager.AssemblyCore.EncodeFunction(Basics.Helpers.Context.HashCode, this.Result)
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
                catch (Exception.GrammerException)
                {
                    throw;
                }
                catch (System.Exception)
                {
                    // Just Handle Exceptions
                }
                finally
                {
                    sR?.Close();
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
