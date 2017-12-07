using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xeora.CLI.Tools;

namespace Xeora.CLI.Basics
{
    public class Create : ICommand
    {
        private string _XeoraProjectPath;
        private string _DomainID;
        private string _LanguageCode;
        private string _LanguageName;

        public Create()
        {
            this._DomainID = "Main";
            this._LanguageCode = "en-US";
            this._LanguageName = "English";
        }

        public void PrintUsage()
        {
            Console.WriteLine();
            Console.WriteLine("xeora create OPTIONS");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("   -h, --help                  print this screen");
            Console.WriteLine("   -x, --xeora PATH            xeora project root path (required)");
            Console.WriteLine("   -d, --domain DOMAINID       domainid for xeora project otherwise it will be 'Main'");
            Console.WriteLine("   -c, --langcode CODE         code for language otherwise it will be 'en-US'");
            Console.WriteLine("   -n, --langname NAME         name for language otherwise it will be 'English'");
            Console.WriteLine();
        }

        public int SetArguments(string[] args)
        {
            for (int aC = 0; aC < args.Length; aC++)
            {
                switch (args[aC])
                {
                    case "-h":
                    case "--help":
                        this.PrintUsage();
                        return -1;
                    case "-x":
                    case "--xeora":
                        if (!this.CheckArgument(args, aC))
                        {
                            this.PrintUsage();
                            Console.WriteLine("xeora project root path is required");
                            Console.WriteLine();
                            return 2;
                        }
                        this._XeoraProjectPath = args[aC + 1];
                        aC++;

                        break;
                    case "-d":
                    case "--domain":
                        if (!this.CheckArgument(args, aC))
                        {
                            this.PrintUsage();
                            Console.WriteLine("domainid has not been specified");
                            Console.WriteLine();
                            return 2;
                        }
                        this._DomainID = args[aC + 1];
                        aC++;

                        break;
                    case "-c":
                    case "--langcode":
                        if (!this.CheckArgument(args, aC))
                        {
                            this.PrintUsage();
                            Console.WriteLine("language code has not been specified");
                            Console.WriteLine();
                            return 2;
                        }
                        this._LanguageCode = args[aC + 1];
                        aC++;

                        break;
                    case "-n":
                    case "--langname":
                        if (!this.CheckArgument(args, aC))
                        {
                            this.PrintUsage();
                            Console.WriteLine("language name has not been specified");
                            Console.WriteLine();
                            return 2;
                        }
                        this._LanguageName = args[aC + 1];
                        aC++;

                        break;
                }
            }

            if (string.IsNullOrEmpty(this._XeoraProjectPath))
            {
                this.PrintUsage();
                Console.WriteLine("xeora project root path is required");
                Console.WriteLine();
                return 2;
            }

            return 0;
        }

        public int Execute()
        {
            try
            {
                DirectoryInfo xeoraProjectRoot =
                    new DirectoryInfo(this._XeoraProjectPath);
                if (!xeoraProjectRoot.Exists)
                    xeoraProjectRoot.Create();

                Console.WriteLine(string.Format("Creating Xeora Project Template at {0}", this._XeoraProjectPath));

                DirectoryInfo domainsRoot =
                    xeoraProjectRoot.CreateSubdirectory("Domains");

                DirectoryInfo domainRoot =
                    domainsRoot.CreateSubdirectory(this._DomainID);

                DirectoryInfo contentsRoot =
                    domainRoot.CreateSubdirectory("Contents");
                DirectoryInfo contentRoot = 
                    contentsRoot.CreateSubdirectory(this._LanguageCode);
                this.CreateStylesFile(contentRoot);

                domainRoot.CreateSubdirectory("Executables");

                DirectoryInfo languagesRoot =
                    domainRoot.CreateSubdirectory("Languages");
                this.CreateLanguageFile(languagesRoot, this._LanguageCode, this._LanguageName);

                DirectoryInfo templatesRoot =
                    domainRoot.CreateSubdirectory("Templates");
                this.CreateConfigurationFile(templatesRoot, this._LanguageCode);
                this.CreateControlsFile(templatesRoot);
                this.CreateDefaultTemplateFile(templatesRoot);

                this.CreateDefaultXeoraSettingsFile(xeoraProjectRoot, this._DomainID);

                Console.WriteLine("\t Completed");

                return 0;
            }
            catch
            {
                Console.WriteLine("creating has been FAILED!");
                return 1;
            }
        }

        private bool CheckArgument(string[] argument, int index)
        {
            if (argument.Length <= index + 1)
                return false;

            string value = argument[index + 1];
            if (value.IndexOf("-") == 0)
                return false;

            return true;
        }

        private void CreateStylesFile(DirectoryInfo target)
        {
            FileInfo targetFile =
                new FileInfo(Path.Combine(target.FullName, "styles.css"));
            if (targetFile.Exists)
                return;

            string fileContent = "/* Default CSS Stylesheet for a New Xeora Web Application project */";

            Stream stylesFileStream = null;
            try
            {
                byte[] fileBytes = Encoding.UTF8.GetBytes(fileContent);

                stylesFileStream = targetFile.OpenWrite();
                stylesFileStream.Write(fileBytes, 0, fileBytes.Length);
            }
            finally
            {
                if (stylesFileStream != null)
                    stylesFileStream.Close();
            }
        }

        private void CreateLanguageFile(DirectoryInfo target, string languageCode, string languageName)
        {
            FileInfo targetFile = 
                new FileInfo(Path.Combine(target.FullName, string.Format("{0}.xml", languageCode)));
            if (targetFile.Exists)
                return;
            
            string fileContent = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<language name=""{0}"" code=""{1}"">
{2}
</language>";

            Dictionary<string, string> translations = this.GetTranslation(languageCode);
            Dictionary<string, string>.Enumerator enumerator = 
                translations.GetEnumerator();

            StringBuilder sB = new StringBuilder();
            while (enumerator.MoveNext())
                sB.AppendFormat("\t<translation id=\"{0}\">{1}</translation>\n", enumerator.Current.Key, enumerator.Current.Value);

            fileContent = string.Format(fileContent, languageName, languageCode, sB.ToString());

            Stream langFileStream = null;
            try
            {
                byte[] fileBytes = Encoding.UTF8.GetBytes(fileContent);

                langFileStream = targetFile.OpenWrite();
                langFileStream.Write(fileBytes, 0, fileBytes.Length);
            }
            finally
            {
                if (langFileStream != null)
                    langFileStream.Close();
            }
        }

        private Dictionary<string, string> GetTranslation(string languageCode)
        {
            Dictionary<string, string> translations = new Dictionary<string, string>();

            // TODO: make translations are dynamic according to the language.
            translations["TEMPLATE_IDMUSTBESET"] = "TemplateID must be set";
            translations["CONTROLSMAPNOTFOUND"] = "ControlsXML file does not exists";
            translations["CONFIGURATIONNOTFOUND"] = "ConfigurationXML file does not exists";
            translations["TEMPLATE_NOFOUND"] = "{0} name Template file does not exists";
            translations["TEMPLATE_AUTH"] = "This Template requires authentication";
            translations["SITETITLE"] = "Hello! - I'm Xeora";

            return translations;
        }

        private void CreateConfigurationFile(DirectoryInfo target, string languageCode)
        {
            FileInfo targetFile =
                new FileInfo(Path.Combine(target.FullName, "Configuration.xml"));
            if (targetFile.Exists)
                return;

            string fileContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Settings>
  <Configuration>
      <Item key=""authenticationpage"" value=""main"" />
      <Item key=""defaultpage"" value=""main"" />
      <Item key=""defaultlanguage"" value=""{0}"" />
      <Item key=""defaultcaching"" value=""TextsOnly"" />
  </Configuration>
  <Services>
      <AuthenticationKeys />
      <Item type=""template"" id=""main"" />
  </Services>
</Settings>
";

            fileContent = string.Format(fileContent, languageCode);

            Stream confFileStream = null;
            try
            {
                byte[] fileBytes = Encoding.UTF8.GetBytes(fileContent);

                confFileStream = targetFile.OpenWrite();
                confFileStream.Write(fileBytes, 0, fileBytes.Length);
            }
            finally
            {
                if (confFileStream != null)
                    confFileStream.Close();
            }
        }

        private void CreateControlsFile(DirectoryInfo target)
        {
            FileInfo targetFile =
                new FileInfo(Path.Combine(target.FullName, "Controls.xml"));
            if (targetFile.Exists)
                return;

            string fileContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Controls />
";

            Stream controlsFileStream = null;
            try
            {
                byte[] fileBytes = Encoding.UTF8.GetBytes(fileContent);

                controlsFileStream = targetFile.OpenWrite();
                controlsFileStream.Write(fileBytes, 0, fileBytes.Length);
            }
            finally
            {
                if (controlsFileStream != null)
                    controlsFileStream.Close();
            }
        }

        private void CreateDefaultTemplateFile(DirectoryInfo target)
        {
            FileInfo targetFile =
                new FileInfo(Path.Combine(target.FullName, "main.xchtml"));
            if (targetFile.Exists)
                return;

            string fileContent = @"$S:HelloXeora:{!NOCACHE
  return ""Hello, Xeora Framework is ready!"";
}:HelloXeora$
";

            Stream defTempFileStream = null;
            try
            {
                byte[] fileBytes = Encoding.UTF8.GetBytes(fileContent);

                defTempFileStream = targetFile.OpenWrite();
                defTempFileStream.Write(fileBytes, 0, fileBytes.Length);
            }
            finally
            {
                if (defTempFileStream != null)
                    defTempFileStream.Close();
            }
        }

        private void CreateDefaultXeoraSettingsFile(DirectoryInfo target, string domainID)
        {
            FileInfo targetFile =
                new FileInfo(Path.Combine(target.FullName, "xeora.settings.json"));
            if (targetFile.Exists)
                return;

            string fileContent = @"{{
  ""service"": {{
    ""address"": ""0.0.0.0"",
    ""port"": 3381,
    ""print"": false
  }},
  ""application"": {{
    ""main"": {{
      ""defaultDomain"": [ ""{0}"" ],
      ""physicalRoot"": ""{1}"",
      ""debugging"": true,
      ""compression"": true,
      ""printAnalytics"": false
    }}
  }}
}}";

            fileContent = string.Format(fileContent, domainID, target.FullName);

            Stream settingsFileStream = null;
            try
            {
                byte[] fileBytes = Encoding.UTF8.GetBytes(fileContent);

                settingsFileStream = targetFile.OpenWrite();
                settingsFileStream.Write(fileBytes, 0, fileBytes.Length);
            }
            finally
            {
                if (settingsFileStream != null)
                    settingsFileStream.Close();
            }
        }
    }
}
