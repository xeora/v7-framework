using Xeora.Web.Basics.Domain;

namespace Xeora.Web.Application
{
    public class LanguageHolder : ILanguage
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
        
        public string Get(string translationId)
        {
            try
            {
                Basics.TranslationResult translationResult =
                    this.Translate(translationId);

                return translationResult.Translated ? translationResult.Translation : this._Language.Get(translationId);
            }
            catch (Exceptions.TranslationNotFoundException)
            {
                if (this._Owner.Parent == null) return string.Empty;

                this._Owner.Parent.Languages.Use(this.Info.Id);

                if (this._Owner.Parent.Languages.Current != null)
                    return this._Owner.Parent.Languages.Current.Get(translationId);
            }

            return string.Empty;
        }

        private Basics.TranslationResult Translate(string translationId)
        {
            if (string.IsNullOrEmpty(this._Owner.Settings.Configurations.LanguageExecutable))
                return new Basics.TranslationResult(false, string.Empty);

            Basics.Execution.Bind translatorBind =
                Basics.Execution.Bind.Make($"{this._Owner.Settings.Configurations.LanguageExecutable}?Translate,p1|p2");
            translatorBind.Parameters.Prepare(
                 parameter =>
                 {
                     return parameter.Key switch
                     {
                         "p1" => this._Language.Info.Id,
                         "p2" => translationId,
                         _ => string.Empty
                     };
                 }
             );
            translatorBind.InstanceExecution = true;

            Basics.Execution.InvokeResult<Basics.TranslationResult> translatorInvokeResult =
                Manager.Executer.InvokeBind<Basics.TranslationResult>(Basics.Helpers.Context?.Request.Header.Method ?? Basics.Context.Request.HttpMethod.GET, translatorBind, Manager.ExecuterTypes.Undefined);

            if (translatorInvokeResult.Result == null || translatorInvokeResult.Exception != null)
                return new Basics.TranslationResult(false, string.Empty);

            return translatorInvokeResult.Result;
        }

        public void Dispose() { /* Dispose will be handled by instance factory */ }
    }
}