using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Xeora.Extension.Executable
{
    public class Loader : MarshalByRefObject, ILoader
    {
        private Guid _HandlerGuid;
        private string _FrameworkBinPath = null;
        private string _DomainDependenciesPath = null;

        public Loader()
        {
            this._HandlerGuid = Guid.NewGuid();

            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += this.SearchDependencies;
        }

        private Assembly SearchDependencies(object sender, ResolveEventArgs e)
        {
            string dllName = e.Name.Split(',')[0].Trim();
            string dllFileLocation =
                Path.Combine(this._DomainDependenciesPath, string.Format("{0}.dll", dllName));

            if (!File.Exists(dllFileLocation))
            {
                dllFileLocation =
                    Path.Combine(this._FrameworkBinPath, string.Format("{0}.dll", dllName));

                if (!File.Exists(dllFileLocation))
                    dllFileLocation = string.Empty;
            }

            if (!string.IsNullOrEmpty(dllFileLocation))
                return Assembly.ReflectionOnlyLoadFrom(this.CopyAssembly(dllFileLocation));

            try
            {
                return Assembly.ReflectionOnlyLoad(e.Name);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private bool _IsTempLocationGenerated = false;
        private string PrepareTempLocation()
        {
            string rString =
                Path.Combine(
                    Environment.GetEnvironmentVariable("TEMP"),
                    string.Format("XeoraCubeAddInTemp\\{0}", this._HandlerGuid.ToString()));

            if (!this._IsTempLocationGenerated)
            {
                if (Directory.Exists(rString))
                {
                    this._HandlerGuid = Guid.NewGuid();

                    return this.PrepareTempLocation();
                }

                Directory.CreateDirectory(rString);

                this._IsTempLocationGenerated = true;
            }

            return rString;
        }

        private string CopyAssembly(string assemblyFileLocation)
        {
            string rString =
                Path.Combine(this.PrepareTempLocation(), Path.GetFileName(assemblyFileLocation));

            if (!File.Exists(rString))
                File.Copy(assemblyFileLocation, rString);

            return rString;
        }

        private Assembly GetAssemblyLoaded(string searchDependenciesPath, string assemblyFileName)
        {
            this._DomainDependenciesPath = searchDependenciesPath;

            // Take Care The XeoraCube FrameWork Libraries Location
            DirectoryInfo FrameworkBinLocationDI = new DirectoryInfo(searchDependenciesPath);
            do
            {
                if (string.Compare(FrameworkBinLocationDI.Name, "Domains", true) == 0)
                {
                    FrameworkBinLocationDI = FrameworkBinLocationDI.Parent;

                    this._FrameworkBinPath = Path.Combine(FrameworkBinLocationDI.FullName, "bin");

                    break;
                }

                FrameworkBinLocationDI = FrameworkBinLocationDI.Parent;
            } while (FrameworkBinLocationDI != null);
            // !---

            try
            {
                AssemblyName assemblyName =
                    AssemblyName.GetAssemblyName(
                        Path.Combine(searchDependenciesPath, assemblyFileName));

                foreach (Assembly asm in AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies())
                {
                    if (string.Compare(asm.GetName().Name, assemblyName.Name, true) == 0)
                        return asm;
                }

                return Assembly.ReflectionOnlyLoadFrom(
                            this.CopyAssembly(
                                Path.Combine(searchDependenciesPath, assemblyFileName)));
            }
            catch (Exception)
            {
                // It is probably not an .net dll.
                return null;
            }
        }

        public ProcessorArchitecture FrameworkArchitecture(string frameworkBinLocation)
        {
            ProcessorArchitecture pA = ProcessorArchitecture.None;

            if (Directory.Exists(frameworkBinLocation))
            {
                this._FrameworkBinPath = frameworkBinLocation;

                foreach (string libFile in Directory.GetFiles(frameworkBinLocation, "*.dll"))
                {
                    try
                    {
                        // Xeora.Web.RegularExpression file is always x86 because of that, Ignore...
                        if (libFile.IndexOf("Xeora.Web.RegularExpression") > -1) continue;

                        Assembly Assembly =
                            Assembly.ReflectionOnlyLoadFrom(this.CopyAssembly(libFile));

                        if (Assembly.GetName().ProcessorArchitecture == ProcessorArchitecture.Amd64)
                        {
                            pA = Assembly.GetName().ProcessorArchitecture;

                            continue;
                        }

                        return ProcessorArchitecture.X86;
                    }
                    catch (Exception)
                    {
                        return ProcessorArchitecture.None;
                    }
                }
            }

            return pA;
        }

        public string[] GetAssemblies(string searchPath)
        {
            List<string> rStringList = new List<string>();

            string assemblyID;
            Assembly assemblyDll;
            string[] dllFileNames = Directory.GetFiles(searchPath, "*.dll");

            foreach (string dllFileLocation in dllFileNames)
            {
                assemblyID = Path.GetFileNameWithoutExtension(dllFileLocation);
                assemblyDll = this.GetAssemblyLoaded(searchPath, Path.GetFileName(dllFileLocation));

                if (assemblyDll != null)
                {
                    Type type = assemblyDll.GetType(string.Format("Xeora.Domain.{0}", assemblyID));

                    if (type != null && type.GetInterface("Xeora.Web.Basics.IDomainExecutable") != null)
                        rStringList.Add(assemblyID);
                }
            }

            return rStringList.ToArray();
        }

        public string[] GetClasses(string assemblyFileLocation, string[] classIDs = null)
        {
            List<string> rStringList = new List<string>();

            string assemblyID = Path.GetFileNameWithoutExtension(assemblyFileLocation);
            Assembly assemblyDll =
                this.GetAssemblyLoaded(
                    Path.GetDirectoryName(assemblyFileLocation),
                    Path.GetFileName(assemblyFileLocation));

            if (assemblyDll == null)
                throw new FileNotFoundException();

            Type[] assemblyClasses = assemblyDll.GetTypes();

            foreach (Type baseClass in assemblyClasses)
            {
                if (string.Compare(baseClass.Namespace, "Xeora.Domain") == 0)
                {
                    if (classIDs == null || classIDs.Length == 0)
                    {
                        if (string.Compare(baseClass.Name, assemblyID) != 0 &&
                            baseClass.Attributes == TypeAttributes.Public)
                            rStringList.Add(baseClass.Name);

                        continue;
                    }

                    if (string.Compare(baseClass.Name, classIDs[0]) == 0)
                    {
                        Type searchingClass = baseClass;

                        for (int cC = 1; cC < classIDs.Length; cC++)
                        {
                            searchingClass = searchingClass.GetNestedType(classIDs[cC]);

                            if (searchingClass == null)
                                break;
                        }

                        if (searchingClass != null)
                        {
                            foreach (Type nT in searchingClass.GetNestedTypes())
                            {
                                if (nT.IsNestedPublic)
                                    rStringList.Add(nT.Name);
                            }
                        }

                        break;
                    }
                }
            }

            return rStringList.ToArray();
        }

        public object[] GetMethods(string assemblyFileLocation, string[] classIDs)
        {
            List<object[]> rObjectList = new List<object[]>();
            List<string> tStringList;

            string assemblyID =
                Path.GetFileNameWithoutExtension(assemblyFileLocation);
            Assembly assemblyDll =
                this.GetAssemblyLoaded(
                    Path.GetDirectoryName(assemblyFileLocation),
                    Path.GetFileName(assemblyFileLocation));

            if (assemblyDll == null)
                throw new FileNotFoundException();

            Type[] assemblyClasses = assemblyDll.GetTypes();

            foreach (Type baseClass in assemblyClasses)
            {
                if (string.Compare(baseClass.Namespace, "Xeora.Domain") == 0)
                {
                    Type searchingClass = null;

                    if (classIDs == null || classIDs.Length == 0)
                    {
                        if (string.Compare(baseClass.Name, assemblyID) == 0)
                            searchingClass = baseClass;

                        continue;
                    }

                    if (string.Compare(baseClass.Name, classIDs[0]) == 0)
                    {
                        searchingClass = baseClass;

                        for (int cC = 1; cC < classIDs.Length; cC++)
                        {
                            searchingClass = searchingClass.GetNestedType(classIDs[cC]);

                            if (searchingClass == null)
                                break;
                        }
                    }

                    if (searchingClass != null)
                    {
                        foreach (System.Reflection.MethodInfo mI in searchingClass.GetMethods())
                        {
                            if (mI.IsPublic && mI.IsStatic)
                            {
                                tStringList = new List<string>();

                                try
                                {
                                    foreach (ParameterInfo pI in mI.GetParameters())
                                        tStringList.Add(pI.Name);
                                }
                                catch (Exception)
                                {
                                    tStringList.Add("~PARAMETERSARENOTCOMPILED~");
                                }

                                rObjectList.Add(new object[] {
                                        mI.Name,
                                        tStringList.ToArray()
                                    });
                            }
                        }

                        break;
                    }
                }
            }

            return rObjectList.ToArray();
        }
    }
}
