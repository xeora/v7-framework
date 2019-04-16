using Xeora.Web.Basics.Domain;

namespace Xeora.Web.Site
{
    public class LanguageHolder : ILanguageDefinition, ILanguage
    {
        private readonly IDomain _Owner;
        private readonly ILanguage _Language;

        public LanguageHolder(IDomain owner, ILanguage language)
        {
            this._Owner = owner;
            this._Language = language;
        }

        public bool Default => this._Language.Default;
        public Basics.Domain.Info.Language Info => this._Language.Info;

        public string Get(string translationID)
        {
            try
            {
                Basics.TranslationResult translationResult =
                    this.Translate(translationID);

                if (translationResult.Translated)
                    return translationResult.Translation;

                return this._Language.Get(translationID);
            }
            catch (Exception.TranslationNotFoundException)
            {
                if (this._Owner.Parent != null)
                {
                    this._Owner.Parent.Languages.Use(this.Info.ID);

                    if (this._Owner.Parent.Languages.Current != null)
                        return this._Owner.Parent.Languages.Current.Get(translationID);
                }
            }

            return string.Empty;
        }

        private Basics.TranslationResult Translate(string translationID)
        {
            if (string.IsNullOrEmpty(this._Owner.Settings.Configurations.LanguageExecutable))
                return new Basics.TranslationResult(false, string.Empty);

            Basics.Execution.Bind translatorBind =
                Basics.Execution.Bind.Make(string.Format("{0}?Translate,p1|p2", this._Owner.Settings.Configurations.LanguageExecutable));
            translatorBind.Parameters.Prepare(
                 parameter => 
                 {
                     switch(parameter.Key)
                     {
                         case "p1":
                             return this._Language.Info.ID;
                         case "p2":
                             return translationID;
                         default:
                             return string.Empty;
                     }
                 }
             );
            translatorBind.InstanceExecution = true;

            Basics.Execution.InvokeResult<Basics.TranslationResult> translatorInvokeResult =
                Manager.AssemblyCore.InvokeBind<Basics.TranslationResult>(Basics.Helpers.Context.Request.Header.Method, translatorBind, Manager.ExecuterTypes.Undefined);

            if (translatorInvokeResult.Result == null || translatorInvokeResult.Exception != null)
                return new Basics.TranslationResult(false, string.Empty);

            return translatorInvokeResult.Result;
        }

        public void Dispose() { /* Dispose will be handled by instance factory */ }
    }
}