namespace Xeora.Web.Directives.Controls
{
    public interface IControl
    {
        bool LinkArguments { get; }

        void Parse();
    }
}