﻿using Xeora.Web.Basics.Domain;
using Xeora.Web.Global;

namespace Xeora.Web.Directives.Elements
{
    public class Translation : Directive
    {
        private readonly string _TranslationId;

        public Translation(string rawValue, ArgumentCollection arguments) :
            base(DirectiveTypes.Translation, arguments)
        {
            this._TranslationId = DirectiveHelper.CaptureDirectiveId(rawValue);
        }

        public override bool Searchable => false;
        public override bool CanAsync => true;
        public override bool CanHoldVariable => false;

        public override void Parse() =>
            this.Children = new DirectiveCollection(this.Mother, this);

        public override bool PreRender()
        {
            if (this.Status != RenderStatus.None)
                return false;
            this.Status = RenderStatus.Rendering;
            
            this.Parse();
            
            this.Mother.RequestInstance(out IDomain instance);

            string translation =
                instance.Languages.Current.Get(this._TranslationId);
            
            if (!string.IsNullOrEmpty(translation))
                this.Children.Add(new Static(translation));

            return true;
        }
        
        public override void PostRender() =>
            this.Deliver(RenderStatus.Rendered, this.Result);
    }
}