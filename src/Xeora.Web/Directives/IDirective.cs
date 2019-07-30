namespace Xeora.Web.Directives
{
    public interface IDirective
    {
        string UniqueId { get; }

        IMother Mother { get; set; }
        IDirective Parent { get; set; }
        string TemplateTree { get; set; }

        DirectiveTypes Type { get; }
        Global.ArgumentCollection Arguments { get; }

        DirectiveScheduler Scheduler { get; }

        bool Searchable { get; }
        bool CanAsync { get; }
        bool HasInlineError { get; set; }
        RenderStatus Status { get; }

        void Parse();
        void Render(string requesterUniqueId);

        void Deliver(RenderStatus status, string result);
        string Result { get; }
    }
}
