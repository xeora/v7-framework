using System;
using System.Collections.Generic;
using Xeora.Web.Basics;
using Xeora.Web.Controller.Directive;

namespace Xeora.Web.Controller
{
    public class ControllerCollection : List<IController>
    {
        private readonly IController _Parent;

        public ControllerCollection(IController parent) =>
            this._Parent = parent;

        public new void Add(IController item)
        {
            item.Mother = this._Parent.Mother;
            item.Parent = this._Parent;
            item.Setup();

            base.Add(item);
        }

        public void AddRange(ControllerCollection collection)
        {
            foreach (Controller item in collection)
            {
                item.Mother = this._Parent.Mother;
                item.Parent = this._Parent;
                item.Setup();
            }

            base.AddRange(collection);
        }

        public void Render(string requesterUniqueID)
        {
            for (int cC = 0; cC < this.Count; cC++)
            {
                IController controller = this[cC];

                try
                {
                    // Analytics Calculator
                    DateTime renderBegins = DateTime.Now;

                    controller.Render(requesterUniqueID);

                    if (Configurations.Xeora.Application.Main.PrintAnalytics)
                    {
                        string analyticOutput = controller.UniqueID;
                        if (controller is INamable)
                            analyticOutput = string.Format("{0} - {1}", analyticOutput, ((INamable)controller).ControlID);
                        Basics.Console.Push(
                            string.Format("analytic - {0}", controller.GetType().Name), 
                            string.Format("{0}ms {{{1}}}", DateTime.Now.Subtract(renderBegins).TotalMilliseconds, analyticOutput), 
                            string.Empty, false);
                    }
                }
                catch (System.Exception ex)
                {
                    controller.Exception = ex;
                }
            }
        }
    }
}