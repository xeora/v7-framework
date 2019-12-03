using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Xeora.Web.Basics;
using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class Property : Directive
    {
        private readonly string _RawValue;

        public Property(string rawValue, ArgumentCollection arguments) :
            base(DirectiveTypes.Property, arguments)
        {
            this._RawValue = rawValue;
            this.CanAsync = true;

            if (string.IsNullOrEmpty(this._RawValue))
                return;
            
            switch (this._RawValue)
            {
                case "DomainContents":
                case "PageRenderDuration":
                    break;
                default:
                    switch (this._RawValue[0])
                    {
                        case '^':
                        case '~':
                        case '-':
                        case '&':
                        case '+':
                        case '=':
                        case '#':
                            break;
                        case '@':
                            switch (this._RawValue[1])
                            {
                                case '-':
                                case '&':
                                case '#':
                                    break;
                                default:
                                    this.CanAsync = false;

                                    break;
                            }

                            break;
                        default:
                            this.CanAsync = false;

                            break;
                    }
                    break;
            }
        }

        public override bool Searchable => false;
        public override bool CanAsync { get; }

        public override void Parse() =>
            this.Children = new DirectiveCollection(this.Mother, this);

        public override bool PreRender()
        {
            if (this.Status != RenderStatus.None)
                return false;
            this.Status = RenderStatus.Rendering;

            this.Parse();
            
            if (string.IsNullOrEmpty(this._RawValue))
            {
                this.Deliver(RenderStatus.Rendered, string.Empty);
                return false;
            }

            Tuple<bool, object> result = 
                Directives.Property.Render(this, this._RawValue);

            if (!result.Item1) return false;
            
            this.Children.Add(
                new Static(result.Item2 == null ? string.Empty : result.Item2.ToString()));

            return true;
        }

        public override void PostRender() =>
            this.Deliver(RenderStatus.Rendered, this.Result);
    }
}