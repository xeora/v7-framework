namespace Xeora.Web.Controller.Directive.Control
{
    public interface IUpdateBlocks
    {
        string[] BlockIDsToUpdate { get; }
        bool UpdateLocalBlock { get; }
    }
}