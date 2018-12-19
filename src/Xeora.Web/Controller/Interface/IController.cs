namespace Xeora.Web.Controller
{
    public interface IController
    {
        string UniqueID { get; }
        System.Exception Exception { get; set; }

        IMother Mother { get; set; }
        IController Parent { get; set; }

        string RawValue { get; }
        int RawStartIndex { get; }
        int RawEndIndex { get; }
        int RawLength { get; }

        string Value { get; }

        ControllerTypes ControllerType { get; }
        Global.ArgumentInfoCollection ContentArguments { get; }

        string UpdateBlockControlID { get; }
        bool IsUpdateBlockController { get; }

        string RenderedValue { get; }
        bool HasInlineError { get; }
        bool IsRendered { get; }

        void Setup();
        void Render(string requesterUniqueID);
    }
}
