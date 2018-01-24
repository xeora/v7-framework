namespace Xeora.Web.Controller.Directive
{
    public delegate void InstanceHandler(ref Basics.Domain.IDomain instance);
    public interface IInstanceRequires
    {
        event InstanceHandler InstanceRequested;
    }
}