using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Xeora.CLI.Basics
{
    public class Create : ICommand
    {
        private string _XeoraProjectPath;
        private string _DomainId;
        private string _LanguageCode;
        private string _LanguageName;

        public Create()
        {
            this._DomainId = "Main";
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
            Console.WriteLine("   -x, --xeora PATH            xeora project path (required)");
            Console.WriteLine("   -d, --domain DOMAINID       domainid for xeora project otherwise it will be 'Main'");
            Console.WriteLine("   -c, --language-code CODE    code for language otherwise it will be 'en-US'");
            Console.WriteLine("   -n, --language-name NAME    name for language otherwise it will be 'English'");
            Console.WriteLine();
        }

        private int SetArguments(IReadOnlyList<string> args)
        {
            for (int aC = 0; aC < args.Count; aC++)
            {
                switch (args[aC])
                {
                    case "-h":
                    case "--help":
                        this.PrintUsage();
                        return -1;
                    case "-x":
                    case "--xeora":
                        if (!Common.CheckArgument(args, aC))
                        {
                            this.PrintUsage();
                            Console.WriteLine("xeora project path is required");
                            Console.WriteLine();
                            return 2;
                        }
                        this._XeoraProjectPath = args[aC + 1];
                        aC++;

                        break;
                    case "-d":
                    case "--domain":
                        if (!Common.CheckArgument(args, aC))
                        {
                            this.PrintUsage();
                            Console.WriteLine("domainid has not been specified");
                            Console.WriteLine();
                            return 2;
                        }
                        this._DomainId = args[aC + 1];
                        aC++;

                        break;
                    case "-c":
                    case "--language-code":
                        if (!Common.CheckArgument(args, aC))
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
                    case "--language-name":
                        if (!Common.CheckArgument(args, aC))
                        {
                            this.PrintUsage();
                            Console.WriteLine("language name has not been specified");
                            Console.WriteLine();
                            return 2;
                        }
                        this._LanguageName = args[aC + 1];
                        aC++;

                        break;
                    default:
                        this.PrintUsage();
                        Console.WriteLine("unrecognizable argument");
                        Console.WriteLine();
                        return 2;
                }
            }

            if (!string.IsNullOrEmpty(this._XeoraProjectPath)) return 0;
            
            this.PrintUsage();
            Console.WriteLine("xeora project path is required");
            Console.WriteLine();
            return 2;
        }

        public async Task<int> Execute(IReadOnlyList<string> args)
        {
            int argumentsResult =
                this.SetArguments(args);
            if (argumentsResult != 0) return argumentsResult;

            List<Task> tasks = new List<Task>();
            try
            {
                DirectoryInfo xeoraProjectRoot =
                    new DirectoryInfo(this._XeoraProjectPath);
                if (!xeoraProjectRoot.Exists) xeoraProjectRoot.Create();

                Console.Write($"Creating Xeora Project from Template at {this._XeoraProjectPath}... ");

                DirectoryInfo domainsRoot =
                    xeoraProjectRoot.CreateSubdirectory("Domains");

                DirectoryInfo domainRoot =
                    domainsRoot.CreateSubdirectory(this._DomainId);

                DirectoryInfo contentsRoot =
                    domainRoot.CreateSubdirectory("Contents");
                DirectoryInfo contentRoot = 
                    contentsRoot.CreateSubdirectory(this._LanguageCode);
                tasks.Add(CreateStylesFile(contentRoot));

                domainRoot.CreateSubdirectory("Executables");

                DirectoryInfo languagesRoot =
                    domainRoot.CreateSubdirectory("Languages");
                tasks.Add(CreateLanguageFile(languagesRoot, this._LanguageCode, this._LanguageName));

                DirectoryInfo templatesRoot =
                    domainRoot.CreateSubdirectory("Templates");
                tasks.Add(CreateConfigurationFile(templatesRoot, this._LanguageCode));
                tasks.Add(CreateControlsFile(templatesRoot));
                tasks.Add(CreateDefaultTemplateFile(templatesRoot));

                tasks.Add(CreateDefaultXeoraSettingsFile(xeoraProjectRoot, this._DomainId));

                await Task.WhenAll(tasks);
                
                Console.WriteLine("Completed");
                
                Console.WriteLine();
                Console.WriteLine("You can run the application using;");
                Console.WriteLine($"   xeora run {this._XeoraProjectPath}{Path.DirectorySeparatorChar}xeora.settings.json");
                
                Console.WriteLine();
                Console.WriteLine("Visit the following url on your browser;");
                Console.WriteLine("   http://localhost:3381");

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed! {e.Message}");
                return 1;
            }
        }

        private static async Task CreateStylesFile(FileSystemInfo target)
        {
            FileInfo targetFile =
                new FileInfo(Path.Combine(target.FullName, "styles.css"));
            if (targetFile.Exists) return;

            const string fileContent = @"/* Default CSS Stylesheet for a New Xeora Web Application project */

a {
    text-decoration: none;
    color: #333333;
}

a.visit {
    color: #333333;
}

ul {
    padding-left: 20px;
    list-style-type: square;
}

li {
    line-height: 22px;
}

#main {
    align: center;
}

#main > DIV {
    margin: 30px;
    align: left;
}

#logo {
    margin-bottom: 20px;
    background-image: url(http://www.xeora.org/Main_en-US/logo2.png);
    background-repeat: no-repeat;
    background-size: auto 36px;
    min-height: 36px;
}

#content {
    margin-left: 30px;
}";

            Stream stylesFileStream = null;
            try
            {
                byte[] fileBytes = 
                    Encoding.UTF8.GetBytes(fileContent);

                stylesFileStream = 
                    targetFile.OpenWrite();
                await stylesFileStream.WriteAsync(fileBytes, 0, fileBytes.Length);
            }
            finally
            {
                stylesFileStream?.Close();
            }
        }

        private static async Task CreateLanguageFile(FileSystemInfo target, string languageCode, string languageName)
        {
            FileInfo targetFile = 
                new FileInfo(Path.Combine(target.FullName, $"{languageCode}.xml"));
            if (targetFile.Exists) return;
            
            string fileContent = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<language name=""{0}"" code=""{1}"">
    <translation id=""SITETITLE"">Hello! - I'm Xeora</translation>
</language>";

            fileContent = string.Format(fileContent, languageName, languageCode);

            Stream langFileStream = null;
            try
            {
                byte[] fileBytes = 
                    Encoding.UTF8.GetBytes(fileContent);

                langFileStream = 
                    targetFile.OpenWrite();
                await langFileStream.WriteAsync(fileBytes, 0, fileBytes.Length);
            }
            finally
            {
                langFileStream?.Close();
            }
        }

        private static async Task CreateConfigurationFile(FileSystemInfo target, string languageCode)
        {
            FileInfo targetFile =
                new FileInfo(Path.Combine(target.FullName, "Configuration.xml"));
            if (targetFile.Exists) return;

            string fileContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Settings>
  <Configuration>
      <Item key=""authenticationTemplate"" value=""main"" />
      <Item key=""defaultTemplate"" value=""main"" />
      <Item key=""defaultLanguage"" value=""{0}"" />
      <Item key=""defaultCaching"" value=""TextsOnly"" />
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
                byte[] fileBytes = 
                    Encoding.UTF8.GetBytes(fileContent);

                confFileStream = 
                    targetFile.OpenWrite();
                await confFileStream.WriteAsync(fileBytes, 0, fileBytes.Length);
            }
            finally
            {
                confFileStream?.Close();
            }
        }

        private static async Task CreateControlsFile(FileSystemInfo target)
        {
            FileInfo targetFile =
                new FileInfo(Path.Combine(target.FullName, "Controls.xml"));
            if (targetFile.Exists) return;

            const string fileContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Controls />
";

            Stream controlsFileStream = null;
            try
            {
                byte[] fileBytes = 
                    Encoding.UTF8.GetBytes(fileContent);

                controlsFileStream = 
                    targetFile.OpenWrite();
                await controlsFileStream.WriteAsync(fileBytes, 0, fileBytes.Length);
            }
            finally
            {
                controlsFileStream?.Close();
            }
        }

        private static async Task CreateDefaultTemplateFile(FileSystemInfo target)
        {
            FileInfo targetFile =
                new FileInfo(Path.Combine(target.FullName, "main.xchtml"));
            if (targetFile.Exists) return;

            const string fileContent = @"<div id=""main"">
    <div>
        <div id=""logo""></div>

        <div id=""content"">
            $S:HelloXeora:{!NOCACHE
                return ""Congratulations!<br/><br/>You successfully set up <b>Xeora Framework...</b>"";
            }:HelloXeora$
            <br/>
            <h3>Now,</h3>
            <p>You can start developing your <b>Xeora Application</b>. The following links may be helpful for you.</p>
            <ul>
                <li><a href=""http://www.xeora.org/downloads"" target=""_blank"">Downloads</a></li>
                <li>
                    <a href=""http://www.xeora.org/documentation/v7"" target=""_blank"">Documentation</a>
                    <ul>
                        <li><a href=""http://www.xeora.org/documentation/v7#folder_hierarchy"" target=""_blank"">Folder Hierarchy</a></li>
                        <li><a href=""http://www.xeora.org/documentation/v7#xeoracube_configuration"" target=""_blank"">Configuration</a></li>
                        <li><a href=""http://www.xeora.org/documentation/v7#frameworks_architecture"" target=""_blank"">Language Grammar</a></li>
                        <li>
                            <a href=""http://www.xeora.org/documentation/v7#using_framework"" target=""_blank"">Using Framework</a>
                            <ul>
                                <li><a href=""http://www.xeora.org/documentation/v7/api"" target=""_blank"">Xeora.Web.Basics API</a></li>
                            </ul>
                        </li>
                    </ul>
                </li>
                <li><a href=""http://www.xeora.org/tutorials"" target=""_blank"">Tutorials</a></li>
            </ul>
        </div>
    </div>
</div>";

            Stream defTempFileStream = null;
            try
            {
                byte[] fileBytes = 
                    Encoding.UTF8.GetBytes(fileContent);

                defTempFileStream = 
                    targetFile.OpenWrite();
                await defTempFileStream.WriteAsync(fileBytes, 0, fileBytes.Length);
            }
            finally
            {
                defTempFileStream?.Close();
            }
        }

        private static async Task CreateDefaultXeoraSettingsFile(FileSystemInfo target, string domainId)
        {
            FileInfo targetFile =
                new FileInfo(Path.Combine(target.FullName, "xeora.settings.json"));
            if (targetFile.Exists) return;

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

            fileContent = string.Format(fileContent, domainId, target.FullName.Replace("\\", "\\\\"));

            Stream settingsFileStream = null;
            try
            {
                byte[] fileBytes = 
                    Encoding.UTF8.GetBytes(fileContent);

                settingsFileStream = 
                    targetFile.OpenWrite();
                await settingsFileStream.WriteAsync(fileBytes, 0, fileBytes.Length);
            }
            finally
            {
                settingsFileStream?.Close();
            }
        }
    }
}
