using System.Reflection;

namespace Xeora.Extension.Executable
{
    public interface ILoader
    {
        ProcessorArchitecture FrameworkArchitecture(string frameworkBinLocation);
        string[] GetAssemblies(string searchPath);
        string[] GetClasses(string assemblyFileLocation, string[] classIDs = null);
        object[] GetMethods(string assemblyFileLocation, string[] classIDs);
    }
}
