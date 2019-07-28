using System;
using System.Collections.Generic;
using System.IO;
using Xeora.Web.Basics;
using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class InLineStatement : Directive, INamable, IBoundable
    {
        private readonly ContentDescription _Contents;
        private DirectiveCollection _Children;
        private bool _Parsed;

        private bool _Cache;
        private string _ParametersDefinition;

        public InLineStatement(string rawValue, ArgumentCollection arguments) :
            base(DirectiveTypes.InLineStatement, arguments)
        {
            this.DirectiveId = DirectiveHelper.CaptureDirectiveId(rawValue);
            this.BoundDirectiveId = DirectiveHelper.CaptureBoundDirectiveId(rawValue);

            this._Contents = new ContentDescription(rawValue);
            this._Cache = true;
            this._ParametersDefinition = null;
        }

        public string DirectiveId { get; }
        public string BoundDirectiveId { get; }
        public bool HasBound => !string.IsNullOrEmpty(this.BoundDirectiveId);

        public override bool Searchable => false;
        public override bool CanAsync => false;

        public DirectiveCollection Children => this._Children;

        public override void Parse()
        {
            if (this._Parsed)
                return;
            this._Parsed = true;

            this._Children = new DirectiveCollection(this.Mother, this);

            // InLineStatement needs to link ContentArguments of its parent.
            if (this.Parent != null)
                this.Arguments.Replace(this.Parent.Arguments);

            string statementContent = this._Contents.Parts[0];
            if (string.IsNullOrEmpty(statementContent))
                throw new Exception.EmptyBlockException();

            this.Mother.RequestParsing(statementContent, ref this._Children, this.Arguments);
        }

        public override void Render(string requesterUniqueId)
        {
            this.Parse();

            string uniqueId =
                string.IsNullOrEmpty(requesterUniqueId) ? this.UniqueId : requesterUniqueId;

            if (this.HasBound)
            {
                if (string.IsNullOrEmpty(requesterUniqueId))
                    return;

                this.Mother.Pool.GetByDirectiveId(this.BoundDirectiveId, out IEnumerable<IDirective> directives);

                if (directives == null) return;

                foreach (IDirective directive in directives)
                {
                    if (!(directive is INamable)) return;

                    string directiveId = ((INamable)directive).DirectiveId;
                    if (string.CompareOrdinal(directiveId, this.BoundDirectiveId) != 0) return;

                    if (directive.Status == RenderStatus.Rendered) continue;
                    
                    directive.Scheduler.Register(this.UniqueId);
                    return;
                }
            }

            if (this.Status != RenderStatus.None)
                return;
            this.Status = RenderStatus.Rendering;

            this.Children.Render(this.UniqueId);
            this.ExecuteStatement(uniqueId);

            this.Scheduler.Fire();
        }

        private void ExecuteStatement(string requesterUniqueId)
        {
            Basics.Domain.IDomain instance = null;
            this.Mother.RequestInstance(ref instance);

            string result = this.Result;

            this.ExtractSubDirectives(ref result);

            if (!this._Cache && string.IsNullOrEmpty(result))
                throw new Exception.EmptyBlockException();

            object methodResultInfo =
                Manager.AssemblyCore.ExecuteStatement(instance.IdAccessTree, this.DirectiveId, result, this.RenderParameters(requesterUniqueId), this._Cache);

            if (methodResultInfo != null && methodResultInfo is System.Exception)
                throw new Exception.ExecutionException(((System.Exception)methodResultInfo).Message, ((System.Exception)methodResultInfo).InnerException);

            if (methodResultInfo != null)
            {
                string renderResult = string.Empty;

                if (methodResultInfo is Basics.ControlResult.RedirectOrder)
                    Helpers.Context.AddOrUpdate("RedirectLocation", ((Basics.ControlResult.RedirectOrder)methodResultInfo).Location);
                else
                    renderResult = Manager.AssemblyCore.GetPrimitiveValue(methodResultInfo);

                this.Deliver(RenderStatus.Rendered, renderResult);

                return;
            }

            this.Deliver(RenderStatus.Rendered, string.Empty);
        }

        private void ExtractSubDirectives(ref string blockContent)
        {
            Dictionary<string, Func<string, string>> subDirectives =
                new Dictionary<string, Func<string, string>>() {
                    {
                        "!NOCACHE",
                        d =>
                        {
                            this._Cache = false;
                            return d.Replace("!NOCACHE", string.Empty);
                        }
                    },
                    {
                        "!PARAMS",
                        d =>
                        {
                            this._ParametersDefinition = 
                                this.ParseParameters(ref d);
                            return d;
                        }
                    }
                };

            // Sub Directive Test
            if (blockContent.IndexOf('!') == 0)
            {
                string directives;
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
                    directives = string.Empty;
                }
                finally
                {
                    sR?.Close();
                }

                foreach (string key in subDirectives.Keys)
                {
                    int dIdx = 
                        directives.IndexOf(key, StringComparison.InvariantCulture);

                    if (dIdx == -1)
                        continue;

                    directives = subDirectives[key].Invoke(directives);
                }
            }
            blockContent = blockContent.Trim();
        }

        private string ParseParameters(ref string directives)
        {
            string paramMarker = "!PARAMS(";

            int openBracketIdx = directives.IndexOf(paramMarker, StringComparison.InvariantCulture);
            if (openBracketIdx == -1)
                return null;

            int closeBracketIdx = directives.LastIndexOf(")", System.StringComparison.InvariantCulture);
            if (closeBracketIdx == -1)
                throw new Exception.GrammerException();
            closeBracketIdx++;

            string paramDefinition =
                directives.Substring(openBracketIdx, closeBracketIdx - openBracketIdx);

            directives = directives.Replace(paramDefinition, string.Empty);

            return paramDefinition.Substring(8, paramDefinition.Length - 9);
        }

        public object[] RenderParameters(string requesterUniqueId)
        {
            if (string.IsNullOrEmpty(this._ParametersDefinition))
                return null;

            List<object> parameters = new List<object>();

            string[] paramDefs = this._ParametersDefinition.Split('|');

            foreach (string paramDef in paramDefs)
                parameters.Add(
                    DirectiveHelper.RenderProperty(this, paramDef, this.Arguments, requesterUniqueId));

            return parameters.ToArray();
        }
    }
}