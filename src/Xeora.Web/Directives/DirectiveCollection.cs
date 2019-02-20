using System;
using System.Collections.Generic;
using System.Text;
using Xeora.Web.Basics;
using Xeora.Web.Directives.Elements;

namespace Xeora.Web.Directives
{
    public class DirectiveCollection : List<IDirective>
    {
        private readonly IMother _Mother;
        private IDirective _Parent;

        public DirectiveCollection(IMother mother, IDirective parent)
        {
            this._Mother = mother;
            this._Parent = parent;
        }

        public void OverrideParent(IDirective parent) =>
            this._Parent = parent;

        public new void Add(IDirective item)
        {
            item.Mother = this._Mother;
            item.Parent = this._Parent;

            if (item is Control)
                ((Control)item).Load();

            base.Add(item);
        }

        public new void AddRange(IEnumerable<IDirective> collection)
        {
            foreach (IDirective item in collection)
            {
                item.Mother = this._Mother;
                item.Parent = this._Parent;

                if (item is Control)
                    ((Control)item).Load();
            }

            base.AddRange(collection);
        }

        public IDirective Find(DirectiveCollection directives, string directiveID)
        {
            foreach (IDirective directive in directives)
            {
                if (!directive.Searchable)
                    continue;

                if (!(directive is INamable))
                    continue;

                if (string.Compare(((INamable)directive).DirectiveID, directiveID) == 0)
                    return directive;

                if (directive is Control control)
                {
                    switch (control.Type)
                    {
                        case Basics.Domain.Control.ControlTypes.ConditionalStatement:
                        case Basics.Domain.Control.ControlTypes.VariableBlock:
                            directive.Render(this._Parent?.UniqueID);

                            break;
                    }
                }
                else
                    directive.Parse();

                DirectiveCollection children =
                    ((IHasChildren)directive).Children;

                IDirective result =
                    this.Find(children, directiveID);

                if (result != null)
                    return result;
            }

            return null;
        }

        public void Render(string requesterUniqueID)
        {
            if (this._Parent == null && 
                this._Mother.UpdateBlockIDStack.Count > 0 && 
                string.IsNullOrEmpty(requesterUniqueID))
            {
                if (this.Count == 1 && this[0] is Single)
                {
                    Single single = 
                        (Single)this[0];
                    single.Parse();

                    IDirective result =
                        this.Find(single.Children, this._Mother.UpdateBlockIDStack.Peek());

                    if (result != null)
                    {
                        single.Children.Clear();
                        single.Children.Add(result);
                    }
                }
            }

            StringBuilder resultContainer = new StringBuilder();

            foreach (IDirective directive in this)
            {
                try
                {
                    // Analytics Calculator
                    DateTime renderBegins = DateTime.Now;

                    directive.Render(requesterUniqueID);
                    resultContainer.Append(directive.Result);

                    if (this._Parent != null)
                        this._Parent.HasInlineError |= directive.HasInlineError;

                    if (Configurations.Xeora.Application.Main.PrintAnalytics)
                    {
                        string analyticOutput = directive.UniqueID;
                        if (directive is INamable)
                            analyticOutput = string.Format("{0} - {1}", analyticOutput, ((INamable)directive).DirectiveID);
                        Basics.Console.Push(
                            string.Format("analytic - {0}", directive.GetType().Name),
                            string.Format("{0}ms {{{1}}}", DateTime.Now.Subtract(renderBegins).TotalMilliseconds, analyticOutput),
                            string.Empty, false);
                    }
                }
                catch (System.Exception ex)
                {
                    Helper.EventLogger.Log(ex);

                    if (Configurations.Xeora.Application.Main.Debugging)
                    {
                        string exceptionString = null;
                        do
                        {
                            exceptionString =
                                string.Format(
                                    "<div align='left' style='border: solid 1px #660000; background-color: #FFFFFF'><div align='left' style='font-weight: bolder; color:#FFFFFF; background-color:#CC0000; padding: 4px;'>{0}</div><br><div align='left' style='padding: 4px'>{1}{2}</div></div>",
                                    ex.Message,
                                    ex.Source,
                                    (!string.IsNullOrEmpty(exceptionString) ? string.Concat("<hr size='1px' />", exceptionString) : null)
                                );

                            ex = ex.InnerException;
                        } while (ex != null);

                        resultContainer.Append(exceptionString);
                    }

                    if (this._Parent != null)
                        this._Parent.HasInlineError = true;
                }
            }

            if (this._Parent != null)
                this._Parent.Result = resultContainer.ToString();
        }
    }
}