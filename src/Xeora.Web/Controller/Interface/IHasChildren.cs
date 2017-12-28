namespace Xeora.Web.Controller
{
    public interface IHasChildren
    {
        ControllerCollection Children { get; }
        IController Find(string controlID);
        void Build();
    }
}
