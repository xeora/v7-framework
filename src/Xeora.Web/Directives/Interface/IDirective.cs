namespace Xeora.Web.Directives
{
    public interface IDirective
    {
        string UniqueID { get; }

        IMother Mother { get; set; }
        IDirective Parent { get; set; }

        DirectiveTypes Type { get; }
        Global.ArgumentCollection Arguments { get; }

        DirectiveScheduler Scheduler { get; }

        bool Searchable { get; }
        bool CanAsync { get; }
        bool HasInlineError { get; set; }
        RenderStatus Status { get; }

        void Parse();
        void Render(string requesterUniqueID);

        void Deliver(RenderStatus status, string result);
        string Result { get; }
    }
}
