using System.Collections.Generic;

namespace Xeora.Web.Directives
{
    public interface IDirective
    {
        string UniqueId { get; }

        IMother Mother { get; set; }
        IDirective Parent { get; set; }
        DirectiveCollection Children { get; }
        string TemplateTree { get; set; }
        List<string> UpdateBlockIds { get; }

        DirectiveTypes Type { get; }
        Global.ArgumentCollection Arguments { get; }

        DirectiveScheduler Scheduler { get; }

        bool Searchable { get; }
        bool CanAsync { get; }
        bool HasInlineError { get; set; }
        RenderStatus Status { get; }

        void Parse();
        bool PreRender();
        void Render();
        void PostRender();

        void Deliver(RenderStatus status, string result);
        string Result { get; }
    }
}
