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

namespace Xeora.Web.Manager
{
    public class StatementFactory
    {
        private ConcurrentDictionary<string, string> _StatementExecutables;
        private Regex _ParamRegEx;
        private object _GetLock;

        private StatementFactory()
        {
            this._StatementExecutables = new ConcurrentDictionary<string, string>();
            this._ParamRegEx = new Regex("\\$(?<ID>\\d+)", RegexOptions.Multiline | RegexOptions.Compiled);
            this._GetLock = new object();
        }

        private static object _Lock = new object();
        private static StatementFactory _Current = null;
        private static StatementFactory Current
        {
            get
            {
                Monitor.Enter(StatementFactory._Lock);
                try
                {
                    if (StatementFactory._Current == null)
                        StatementFactory._Current = new StatementFactory();
                }
                finally
                {
                    Monitor.Exit(StatementFactory._Lock);
                }

                return StatementFactory._Current;
            }
        }

        public static StatementExecutable CreateExecutable(string[] domainIDAccessTree, string statementBlockID, string statement, bool parametric, bool cache)
        {
            Loader.Initialize(() => {
                Application.Dispose();
                StatementFactory.Dispose();
            });

            try
            {
                string blockKey =
                    string.Format(
                        "BLOCKCALL_{0}_{1}",
                        string.Join<string>("_", domainIDAccessTree),
                        statementBlockID.Replace('.', '_')
                    );

                string executableName =
                    StatementFactory.Current.Get(blockKey, statement, parametric, cache);

                if (string.IsNullOrEmpty(executableName))
                    throw new Exception.GrammerException();

                return new StatementExecutable(executableName, blockKey, null);
            }
            catch (System.Exception ex)
            {
                return new StatementExecutable(string.Empty, string.Empty, ex);
            }
        }

        private string Get(string blockKey, string statement, bool parametric, bool cache)
        {
            if (!cache)
                return this.Create(blockKey, statement, parametric, cache);

            lock (this._GetLock)
            {
                string executableName;

                if (!this._StatementExecutables.TryGetValue(blockKey, out executableName))
                    executableName = this.Create(blockKey, statement, parametric, cache);

                return executableName;
            }
        }

        private string Create(string blockKey, string statement, bool parametric, bool cache)
        {
            string executableName = 
                string.Format("X{0}", Guid.NewGuid().ToString().Replace("-", string.Empty));

            statement =
                this.Prepare(executableName, blockKey, statement, parametric);

            this.Compile(executableName, blockKey, statement, cache);

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

            codeBlock.AppendFormat("public class {0} : IDomainExecutable {{", executableName);
            codeBlock.AppendLine("public void Initialize() {}");
            codeBlock.AppendLine("public void Terminate() {}");
            codeBlock.AppendLine("public ResolutionResult ResolveURL(string requestFilePath) => null;");
            codeBlock.AppendLine("public void PreExecute(string executionID, ref MethodInfo mI) {}");
            codeBlock.AppendLine("public void PostExecute(string executionID, ref object result) {}");
            codeBlock.AppendLine("} /* class */");

            codeBlock.AppendFormat("public class {0} {{", blockKey);
            codeBlock.AppendFormat("public static object Execute({0}) {{", (parametric ? "params object[] p" : string.Empty));
            int lastIndex = 0;
            if (parametric)
            {
                MatchCollection paramMatches = this._ParamRegEx.Matches(statement);
                Match paramMatch = null;
                IEnumerator remEnum = paramMatches.GetEnumerator();

                while (remEnum.MoveNext())
                {
                    paramMatch = (Match)remEnum.Current;

                    if (paramMatch.Index > lastIndex)
                    {
                        codeBlock.Append(statement.Substring(lastIndex, paramMatch.Index - lastIndex));
                        lastIndex = paramMatch.Index;
                    }

                    codeBlock.AppendFormat("p[{0}]", paramMatch.Result("${ID}"));

                    lastIndex = (paramMatch.Index + paramMatch.Value.Length);
                }
            }
            codeBlock.Append(statement.Substring(lastIndex));
            codeBlock.AppendLine("} /* method */");
            codeBlock.AppendLine("} /* class */");

            codeBlock.AppendLine("} /* namespace */");

            return codeBlock.ToString();
        }

        private void Compile(string executableName, string blockKey, string codeBlock, bool cache)
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

                if (assembly.Location.IndexOf(Loader.Current.Path) > -1)
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

            Stream assemblyFS = null;
            try
            {
                assemblyFS = 
                    new FileStream(
                        Path.Combine(Loader.Current.Path, string.Format("{0}.dll", executableName)), 
                        FileMode.Create, 
                        FileAccess.ReadWrite
                    );

                EmitResult eR = compiler.Emit(assemblyFS);
                if (!eR.Success)
                {
                    System.Text.StringBuilder sB =
                        new System.Text.StringBuilder();

                    foreach (Diagnostic diag in eR.Diagnostics)
                        sB.AppendLine(diag.ToString());

                    throw new System.Exception(sB.ToString());
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (assemblyFS != null)
                    assemblyFS.Close();
            }

            if (cache)
                this._StatementExecutables.TryAdd(blockKey, executableName);
        }

        public static void Dispose()
        {
            foreach (string key in StatementFactory.Current._StatementExecutables.Keys)
            {
                string dummy;
                StatementFactory.Current._StatementExecutables.TryRemove(key, out dummy);
            }
        }
    }
}
