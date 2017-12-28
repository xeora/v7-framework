namespace Xeora.Web.Controller
{
    public delegate void ParsingHandler(string rawValue, ref ControllerCollection childrenContainer, Global.ArgumentInfoCollection contentArguments);
    public interface IMother
    {
        ControllerPool Pool { get; }
        ControllerSchedule Scheduler { get; }

        Basics.ControlResult.Message MessageResult { get; }
        string ProcessingUpdateBlockControlID { get; }

        void RequestParsing(string rawValue, ref ControllerCollection childrenContainer, Global.ArgumentInfoCollection contentArguments);
    }
}
