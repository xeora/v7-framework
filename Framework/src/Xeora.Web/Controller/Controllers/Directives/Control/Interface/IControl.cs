using System.Collections.Generic;

namespace Xeora.Web.Controller.Directive.Control
{
    public delegate void ControlResolveHandler(string controlID, ref Basics.IDomain workingInstance, out ControlSettings settings);
    public interface IControl : IController, INamable, IBoundable, ILevelable
    {
        ControlSettings Settings { get; }

        ControlTypes Type { get; }
        SecurityInfo Security { get; }
        Basics.Execution.BindInfo Bind { get; }
        AttributeInfoCollection Attributes { get; }

        IControl Clone();
    }
}