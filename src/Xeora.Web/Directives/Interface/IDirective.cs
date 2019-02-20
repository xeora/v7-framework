namespace Xeora.Web.Directives
{
    public interface IDirective
    {
        string UniqueID { get; }

        IMother Mother { get; set; }
        IDirective Parent { get; set; }

        DirectiveTypes Type { get; }
        Global.ArgumentCollection Arguments { get; }

        bool Searchable { get; }
        bool HasInlineError { get; set; }

        string Result { get; set; }
        bool Rendered { get; }

        void Parse();
        void Render(string requesterUniqueID);
    }
}
