using System;
using Xeora.Web.Global;

namespace Xeora.Web.Controller.Directive.Control
{
    public abstract class ControlHelper
    {
        public static ControlTypes ParseControlType(string controlTypeName)
        {
            ControlTypes controlType;
            if (Enum.TryParse(controlTypeName, true, out controlType))
                return controlType;

            return ControlTypes.Unknown;
        }

        public static IControl MakeControl(int rawStartIndex, string rawValue, ArgumentInfoCollection contentArguments, ControlResolveHandler controlResolveHandler)
        {
            Renderless dummy = new Renderless(rawStartIndex, rawValue, contentArguments);
            string controlID = DirectiveHelper.CaptureControlID(dummy.Value);

            ControlSettings controlSettings =
                ControlHelper.GetControlSettings(controlID, controlResolveHandler);

            if (controlSettings == null)
                return new Unknown(rawStartIndex, rawValue, contentArguments, new ControlSettings(null));

            switch (controlSettings.Type)
            {
                case ControlTypes.Button:
                    return new Button(rawStartIndex, rawValue, contentArguments, controlSettings);
                case ControlTypes.Checkbox:
                    return new Checkbox(rawStartIndex, rawValue, contentArguments, controlSettings);
                case ControlTypes.ConditionalStatement:
                    return new ConditionalStatement(rawStartIndex, rawValue, contentArguments, controlSettings);
                case ControlTypes.DataList:
                    return new DataList(rawStartIndex, rawValue, contentArguments, controlSettings);
                case ControlTypes.ImageButton:
                    return new ImageButton(rawStartIndex, rawValue, contentArguments, controlSettings);
                case ControlTypes.LinkButton:
                    return new LinkButton(rawStartIndex, rawValue, contentArguments, controlSettings);
                case ControlTypes.Password:
                    return new Password(rawStartIndex, rawValue, contentArguments, controlSettings);
                case ControlTypes.RadioButton:
                    return new RadioButton(rawStartIndex, rawValue, contentArguments, controlSettings);
                case ControlTypes.Textarea:
                    return new Textarea(rawStartIndex, rawValue, contentArguments, controlSettings);
                case ControlTypes.Textbox:
                    return new Textbox(rawStartIndex, rawValue, contentArguments, controlSettings);
                case ControlTypes.VariableBlock:
                    return new VariableBlock(rawStartIndex, rawValue, contentArguments, controlSettings);
                default:
                    return new Unknown(rawStartIndex, rawValue, contentArguments, controlSettings);
            }
        }

        private static ControlSettings GetControlSettings(string controlID, ControlResolveHandler controlResolveRequested)
        {
            Basics.Domain.IDomain workingInstance = null;
            do
            {
                ControlSettings controlSettings = null;
                controlResolveRequested?.Invoke(controlID, ref workingInstance, out controlSettings);

                if (controlSettings != null)
                    return controlSettings;

                if (workingInstance == null)
                    return null;

                workingInstance = workingInstance.Parent;
            } while (workingInstance != null);

            return null;
        }
    }
}