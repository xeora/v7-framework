using System.Text.RegularExpressions;
using Xeora.Web.Controller.Directive;

namespace Xeora.Web.Controller
{
    public class ControllerHelper
    {
        public static string RenderSingleContent(string rawValue, IController parent, Global.ArgumentInfoCollection contentArguments, string requesterUniqueID)
        {
            IController controller =
                ControllerHelper.CreateSingleController(rawValue, parent, contentArguments);
            controller.Render(requesterUniqueID);

            return controller.RenderedValue;
        }

        public static Basics.Execution.Bind RenderBind(Basics.Execution.Bind bind, IController parent, Global.ArgumentInfoCollection contentArguments, string requesterUniqueID)
        {
            if (bind == null)
                return null;

            if (bind.Parameters.Count == 0)
                return bind;

            string[] parameters = new string[bind.Parameters.Count];

            // Render Params One By One (this render process is mainly controls with bind which fired when a control get interaction with user)
            // The aim is rendering static values comes from dinamic ones like =$#SomeID$
            for (int pC = 0; pC < bind.Parameters.Count; pC++)
            {
                parameters[pC] = ControllerHelper.RenderSingleContent(
                    bind.Parameters[pC].Query,
                    parent,
                    contentArguments,
                    requesterUniqueID
                );
            }

            bind.Parameters.Override(parameters);

            return bind;
        }

        public static IController CreateSingleController(string rawValue, IController parent, Global.ArgumentInfoCollection contentArguments)
        {
            Single controller =
                new Single(0, rawValue, contentArguments);
            controller.Mother = parent.Mother;
            controller.Parent = parent;
            controller.Setup();

            return controller;
        }

        private static Regex _ControllerTypeRegEx =
            new Regex("\\$((\\w(\\#\\d+(\\+)?)?(\\[[\\.\\w\\-]+\\])?)|(\\w+))\\:", RegexOptions.Compiled);
        public static ControllerTypes CaptureControllerType(string rawValue)
        {
            if (!string.IsNullOrEmpty(rawValue))
            {
                Match cpIDMatch = ControllerHelper._ControllerTypeRegEx.Match(rawValue);

                if (cpIDMatch.Success)
                    return ControllerTypes.Directive;

                return ControllerTypes.Property;
            }

            return ControllerTypes.Renderless;
        }
    }
}
