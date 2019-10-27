using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace Xeora.Web.Manager.Statement
{
    public class Factory
    {
        private readonly ConcurrentDictionary<string, string> _StatementExecutables;
        private readonly Regex _ParamRegEx;
        private readonly object _CacheLock;

        private Factory()
        {
            this._StatementExecutables = new ConcurrentDictionary<string, string>();
            this._ParamRegEx = new Regex("\\$(?<Id>\\d+)", RegexOptions.Multiline | RegexOptions.Compiled);
            this._CacheLock = new object();
        }

        private static readonly object Lock = new object();
        private static Factory _Current;
        private static Factory Current
        {
            get
            {
                Monitor.Enter(Factory.Lock);
                try
                {
                    return Factory._Current ?? (Factory._Current = new Factory());
                }
                finally
                {
                    Monitor.Exit(Factory.Lock);
                }
            }
        }

        public static Executable CreateExecutable(string[] domainIdAccessTree, string statementBlockId, string statement, bool parametric, bool cache)
        {
            try
            {
                string blockKey =
                    string.Format(
                        "BLOCKCALL_{0}_{1}",
                        string.Join<string>("_", domainIdAccessTree),
                        statementBlockId.Replace('.', '_')
                    );

                string executableName =
                    Factory.Current.Get(blockKey, statement, parametric, cache);

                if (string.IsNullOrEmpty(executableName))
                    throw new Exceptions.GrammarException();

                return new Executable(executableName, blockKey, null);
            }
            catch (Exception ex)
            {
                return new Executable(string.Empty, string.Empty, ex);
            }
        }

        private string Get(string blockKey, string statement, bool parametric, bool cache)
        {
            if (!cache)
                return this.Create(blockKey, statement, parametric);

            if (this._StatementExecutables.TryGetValue(blockKey, out string executableName)) return executableName;
            
            lock (this._CacheLock)
            {
                executableName = 
                    this.Create(blockKey, statement, parametric);
                this._StatementExecutables.TryAdd(blockKey, executableName);

                return executableName;
            }
        }

        private string Create(string blockKey, string statement, bool parametric)
        {
            string executableName =
                $"X{Guid.NewGuid().ToString().Replace("-", string.Empty)}";

            statement =
                this.Prepare(executableName, blockKey, statement, parametric);

            this.Compile(executableName, statement);

            return executableName;
        }

        private string Prepare(string executableName, string blockKey, string statement, bool parametric)
        {
            if (statement == null)
                return null;

            System.Text.StringBuilder codeBlock =
                new System.Text.StringBuilder();

            codeBlock.AppendLine("using System;");
            codeBlock.AppendLine("using System.Data;");
            codeBlock.AppendLine("using System.Xml;");
            codeBlock.AppendLine("using System.Reflection;");
            codeBlock.AppendLine("using Xeora.Web.Basics;");
            codeBlock.AppendLine("using Xeora.Web.Basics.Mapping;");
            codeBlock.AppendLine("namespace Xeora.Domain {");

            codeBlock.AppendFormat("public class {0} : DomainExecutable {{", executableName);
            codeBlock.AppendFormat("public {0}(INegotiator negotiator) : base(negotiator) {{}}", executableName);
            codeBlock.AppendLine("public override void PreExecute(string executionId, MethodInfo mI) {}");
            codeBlock.AppendLine("public override void PostExecute(string executionId, ref object result) {}");
            codeBlock.AppendLine("public override void Terminate() {}");
            codeBlock.AppendLine("public override ResolutionResult ResolveUrl(string requestFilePath) => null;");
            codeBlock.AppendLine("public override PermissionResult EnsurePermission(string permissionKey) => null;");
            codeBlock.AppendLine("public override TranslationResult Translate(string languageCode, string translationId) => null;");
            codeBlock.AppendLine("} /* class */");

            codeBlock.AppendFormat("public class {0} {{", blockKey);
            codeBlock.AppendFormat("public static object Execute({0}) {{", parametric ? "params object[] p" : string.Empty);
            int lastIndex = 0;
            if (parametric)
            {
                MatchCollection paramMatches = this._ParamRegEx.Matches(statement);
                IEnumerator remEnum = paramMatches.GetEnumerator();

                while (remEnum.MoveNext())
                {
                    Match paramMatch = (Match)remEnum.Current;

                    if (paramMatch.Index > lastIndex)
                    {
                        codeBlock.Append(statement.Substring(lastIndex, paramMatch.Index - lastIndex));
                        lastIndex = paramMatch.Index;

                        break;
                    }

                    codeBlock.AppendFormat("p[{0}]", paramMatch.Result("${Id}"));

                    lastIndex = paramMatch.Index + paramMatch.Length;
                }
            }
            codeBlock.Append(statement.Substring(lastIndex));
            codeBlock.AppendLine("} /* method */");
            codeBlock.AppendLine("} /* class */");

            codeBlock.AppendLine("} /* namespace */");

            return codeBlock.ToString();
        }

        private void Compile(string executableName, string codeBlock)
        {
            SyntaxTree syntaxTree =
                CSharpSyntaxTree.ParseText(codeBlock);
            CSharpCompilationOptions compilerOptions =
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            List<MetadataReference> references = new List<MetadataReference>();

            Assembly[] currentDomainAssemblies =
                AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in currentDomainAssemblies)
            {
                if (assembly.IsDynamic || string.IsNullOrEmpty(assembly.Location) || !File.Exists(assembly.Location))
                    continue;

                if (assembly.Location.IndexOf(Loader.Current.Path, StringComparison.Ordinal) > -1)
                    continue;

                references.Add(
                    MetadataReference.CreateFromFile(assembly.Location));
            }

            CSharpCompilation compiler =
                CSharpCompilation.Create(
                    executableName,
                    options: compilerOptions,
                    syntaxTrees: new List<SyntaxTree> { syntaxTree },
                    references: references
                );

            Stream assemblyStream = null;
            try
            {
                assemblyStream = 
                    new FileStream(
                        Path.Combine(Loader.Current.Path, $"{executableName}.dll"), 
                        FileMode.Create, 
                        FileAccess.ReadWrite
                    );

                EmitResult eR = compiler.Emit(assemblyStream);
                if (eR.Success) return;
                
                System.Text.StringBuilder sB =
                    new System.Text.StringBuilder();

                foreach (Diagnostic diag in eR.Diagnostics)
                    sB.AppendLine(diag.ToString());

                throw new Exception(sB.ToString());
            }
            finally
            {
                assemblyStream?.Close();
            }
        }

        private void Reset()
        {
            lock (this._CacheLock)
            {
                this._StatementExecutables.Clear();
            }
        }

        public static void Dispose() =>
            Factory.Current.Reset();
    }
}
