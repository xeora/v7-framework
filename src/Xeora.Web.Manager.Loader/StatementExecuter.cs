using System;
using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom.Compiler;

namespace Xeora.Web.Manager
{
    internal class StatementExecuter
    {
        private ConcurrentDictionary<string, Assembly> _StatementExecutables;

        private StatementExecuter() =>
            this._StatementExecutables = new ConcurrentDictionary<string, Assembly>();

        private static StatementExecuter _Current = null;
        private static StatementExecuter Current
        {
            get
            {
                if (StatementExecuter._Current == null)
                    StatementExecuter._Current = new StatementExecuter();

                return StatementExecuter._Current;
            }
        }

        public static object Execute(string[] domainIDAccessTree, string statementBlockID, string statement, bool noCache)
        {
            try
            {
                string blockKey =
                    string.Format(
                        "BLOCKCALL_{0}_{1}",
                        string.Join<string>("_", domainIDAccessTree),
                        statementBlockID.Replace('.', '_')
                    );

                Assembly objAssembly = 
                    StatementExecuter.Current.Prepare(blockKey, statement, noCache);

                if (objAssembly == null)
                    throw new Exception.GrammerException();

                Type assemblyObject =
                    objAssembly.CreateInstance(string.Format("Xeora.Domain.Statement.{0}", blockKey)).GetType();
                MethodInfo MethodObject = assemblyObject.GetMethod("Execute");

                return MethodObject.Invoke(assemblyObject, BindingFlags.DeclaredOnly | BindingFlags.InvokeMethod, null, null, null);
            }
            catch (System.Exception ex)
            {
                return ex;
            }
        }

        private Assembly Prepare(string blockKey, string statement, bool noCache)
        {
            if (statement == null)
                return null;

            Assembly assemblyResult;
            if (StatementExecuter.Current._StatementExecutables.TryGetValue(blockKey, out assemblyResult))
                return assemblyResult;

            System.Text.StringBuilder codeBlock =
                new System.Text.StringBuilder();

            codeBlock.AppendLine("using System;");
            codeBlock.AppendLine("using System.Data;");
            codeBlock.AppendLine("using System.Xml;");
            codeBlock.AppendLine("namespace Xeora.Domain.Statement");
            codeBlock.AppendLine("{");
            codeBlock.AppendFormat("public class {0}", blockKey);
            codeBlock.AppendLine("{");
            codeBlock.AppendLine("public static object Execute()");
            codeBlock.AppendLine("{");
            codeBlock.AppendFormat("{0}", statement);
            codeBlock.AppendLine("} // method");
            codeBlock.AppendLine("} // class");
            codeBlock.AppendLine("} // namespace");

            CSharpCodeProvider cSharpProvider =
                new CSharpCodeProvider();
            CodeDomProvider objCompiler =
                CodeDomProvider.CreateProvider("CSharp");
            CompilerParameters compilerParams =
                new CompilerParameters();

            compilerParams.GenerateExecutable = false;
            compilerParams.GenerateInMemory = true;
            compilerParams.IncludeDebugInformation = false;

            Assembly[] currentDomainAssemblies =
                AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in currentDomainAssemblies)
                compilerParams.ReferencedAssemblies.Add(assembly.Location);

            CompilerResults compilerResults =
                objCompiler.CompileAssemblyFromSource(compilerParams, codeBlock.ToString());

            if (compilerResults.Errors.HasErrors)
            {
                System.Text.StringBuilder sB =
                    new System.Text.StringBuilder();

                foreach (CompilerError cRE in compilerResults.Errors)
                    sB.AppendLine(cRE.ErrorText);

                throw new System.Exception(sB.ToString());
            }

            if (!noCache)
                StatementExecuter.Current._StatementExecutables.TryAdd(blockKey, compilerResults.CompiledAssembly);

            return compilerResults.CompiledAssembly;
        }

        public static void Dispose()
        {
            foreach (string key in StatementExecuter.Current._StatementExecutables.Keys)
            {
                Assembly dummy;
                StatementExecuter.Current._StatementExecutables.TryRemove(key, out dummy);
            }
        }
    }
}
