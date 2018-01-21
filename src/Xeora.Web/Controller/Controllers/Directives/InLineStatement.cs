using System.Collections.Generic;
using System.IO;
using Xeora.Web.Basics;

namespace Xeora.Web.Controller.Directive
{
    public class InLineStatement : DirectiveWithChildren, IInstanceRequires, INamable, IBoundable
    {
        private bool _Cache;
        private string _ParametersDefinition;
        public event InstanceHandler InstanceRequested;

        public InLineStatement(int rawStartIndex, string rawValue, Global.ArgumentInfoCollection contentArguments) :
            base(rawStartIndex, rawValue, DirectiveTypes.InLineStatement, contentArguments)
        {
            this.ControlID = DirectiveHelper.CaptureControlID(this.Value);
            this.BoundControlID = DirectiveHelper.CaptureBoundControlID(this.Value);

            this._Cache = true;
            this._ParametersDefinition = null;
        }

        public string ControlID { get; private set; }
        public string BoundControlID { get; private set; }
        public bool HasBound => !string.IsNullOrEmpty(this.BoundControlID);

        public override void Render(string requesterUniqueID)
        {
            if (!this.HasBound)
            {
                this.RenderInternal(requesterUniqueID);

                return;
            }

            if (string.IsNullOrEmpty(requesterUniqueID))
                return;

            IController controller = null;
            this.Mother.Pool.GetInto(requesterUniqueID, out controller);

            if (controller != null &&
                controller is INamable &&
                string.Compare(((INamable)controller).ControlID, this.BoundControlID) == 0)
            {
                this.RenderInternal(requesterUniqueID);

                return;
            }

            this.Mother.Scheduler.Register(this.BoundControlID, this.UniqueID);
        }

        private void RenderInternal(string requesterUniqueID)
        {
            Global.ContentDescription contentDescription =
                new Global.ContentDescription(this.Value);

            string blockContent = contentDescription.Parts[0];

            // InLineStatement does not have any ContentArguments, That's why it copies it's parent Arguments
            if (this.Parent != null)
                this.ContentArguments.Replace(this.Parent.ContentArguments);

            this.Parse(blockContent);
            base.Render(requesterUniqueID);
        }

        private void ExtractSubDirectives(ref string blockContent)
        {
            Dictionary<string, System.Func<string, string>> subDirectives =
                new Dictionary<string, System.Func<string, string>>() {
                    {
                        "!NOCACHE",
                        new System.Func<string, string>(
                            (d) =>
                            {
                                this._Cache = false;
                                return d.Replace("!NOCACHE", string.Empty);
                            }
                        )
                    },
                    {
                        "!PARAMS",
                        new System.Func<string, string>(
                            (d) =>
                            {
                                this._ParametersDefinition = this.ParseParameters(ref d);
                                return d;
                            }
                        )
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
                catch (Exception.GrammerException ex)
                {
                    throw ex;
                }
                catch (System.Exception)
                {
                    // Just Handle Exceptions
                }
                finally
                {
                    if (sR != null)
                        sR.Close();
                }

                foreach (string key in subDirectives.Keys)
                {
                    int dIdx = directives.IndexOf(key);

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

            int openBracketIdx = directives.IndexOf(paramMarker);
            if (openBracketIdx == -1)
                return null;

            int closeBracketIdx = directives.LastIndexOf(")");
            if (closeBracketIdx == -1)
                throw new Exception.GrammerException();
            closeBracketIdx++;

            string paramDefinition =
                directives.Substring(openBracketIdx, closeBracketIdx - openBracketIdx);

            directives = directives.Replace(paramDefinition, string.Empty);

            return paramDefinition.Substring(8, paramDefinition.Length - 9);
        }

        public object[] RenderParameters()
        {
            if (string.IsNullOrEmpty(this._ParametersDefinition))
                return null;

            List<object> parameters = new List<object>();

            string[] paramDefs = this._ParametersDefinition.Split('|');

            foreach (string paramDef in paramDefs)
            {
                Property property =
                    new Property(0, paramDef, this.Parent.ContentArguments);
                property.Mother = this.Mother;
                property.Parent = this.Parent;
                property.InstanceRequested += (ref IDomain instance) => InstanceRequested(ref instance);
                property.Setup();

                property.Render(null);

                parameters.Add(property.ObjectResult);
            }

            return parameters.ToArray();
        }

        public override void Build()
        {
            base.Build();

            IDomain instance = null;
            InstanceRequested(ref instance);

            string renderedValue = this.RenderedValue;

            this.ExtractSubDirectives(ref renderedValue);

            if (!this._Cache && string.IsNullOrEmpty(renderedValue))
                throw new Exception.EmptyBlockException();

            object methodResultInfo =
                Manager.AssemblyCore.ExecuteStatement(instance.IDAccessTree, this.ControlID, renderedValue, this.RenderParameters(), this._Cache);

            if (methodResultInfo != null && methodResultInfo is System.Exception)
                throw new Exception.ExecutionException(((System.Exception)methodResultInfo).Message, ((System.Exception)methodResultInfo).InnerException);

            if (methodResultInfo != null)
            {
                string renderResult = string.Empty;

                if (methodResultInfo is Basics.ControlResult.RedirectOrder)
                    Helpers.Context.AddOrUpdate("RedirectLocation", ((Basics.ControlResult.RedirectOrder)methodResultInfo).Location);
                else
                    renderResult = Basics.Execution.GetPrimitiveValue(methodResultInfo);

                this.RenderedValue = renderResult;

                return;
            }

            this.RenderedValue = string.Empty;
        }
    }
}