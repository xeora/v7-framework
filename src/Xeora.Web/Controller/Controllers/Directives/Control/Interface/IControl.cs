namespace Xeora.Web.Controller.Directive.Control
{
    public delegate void ControlResolveHandler(string controlID, ref Basics.Domain.IDomain workingInstance, out ControlSettings settings);
    public interface IControl : IController, INamable, IBoundable, ILevelable
    {
        ControlSettings Settings { get; }

        ControlTypes Type { get; }
        SecurityInfo Security { get; }
        Basics.Execution.Bind Bind { get; }
        AttributeInfoCollection Attributes { get; }

        IControl Clone();
    }
}