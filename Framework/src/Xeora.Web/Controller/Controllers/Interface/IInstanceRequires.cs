namespace Xeora.Web.Controller.Directive
{
    public delegate void InstanceHandler(ref Basics.IDomain instance);
    public interface IInstanceRequires
    {
        event InstanceHandler InstanceRequested;
    }
}