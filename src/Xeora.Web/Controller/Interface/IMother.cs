using System.Collections.Generic;

namespace Xeora.Web.Controller
{
    public delegate void ParsingHandler(string rawValue, ref ControllerCollection childrenContainer, Global.ArgumentInfoCollection contentArguments);
    public interface IMother
    {
        ControllerPool Pool { get; }
        ControllerSchedule Scheduler { get; }

        Basics.ControlResult.Message MessageResult { get; }
        Stack<string> UpdateBlockControlIDStack { get; }

        void RequestParsing(string rawValue, ref ControllerCollection childrenContainer, Global.ArgumentInfoCollection contentArguments);
    }
}
