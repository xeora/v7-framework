using System.Reflection;

namespace Xeora.Web.Basics
{
    public interface IDomainExecutable
    {
        /// <summary>
        /// Initialize Procedure will be called when Xeora Domain has been loaded.
        /// </summary>
        void Initialize();

        /// <summary>
        /// PreExecute Procedure has been called right before the Xeora Executable Call
        /// </summary>
        /// <param name="executionID">A Unique ID to tracked the Execution between PreExecute and PostExecute</param>
        /// <param name="mI">The MethodInfo that will be called after PreExecute function. Modifications will effect the Executable Call</param>
        void PreExecute(string executionID, ref MethodInfo mI);

        /// <summary>
        /// PostExecute Procedure has been called right after the Xeora Executable Call
        /// </summary>
        /// <param name="executionID">A Unique ID to tracked the Execution between PreExecute and PostExecute</param>
        /// <param name="result">The Result of the Xeora Executable Call (If any, otherwise null). Result will be changable</param>
        void PostExecute(string executionID, ref object result);

        /// <summary>
        /// Finalize Procedure has been called right before the Xeora Domain unload.
        /// Unload can happen in two ways. One is domain will reach the memory limit and 
        /// try to do a garbage collection and executable will be unloaded, and the
        /// second way is IIS is stopping and Xeora Framework will be unloaded.
        /// </summary>
        void Finalize();

        /// <summary>
        /// URLResolver is called when URLMapping is active and defined resolutions would not
        /// reach any success about resolving the request file path. If you have any custom
        /// resolution for request file path, do it in this function and return the result.
        /// </summary>
        /// <param name="requestFilePath">Requested File Path comes right after Application Root</param>
        /// <returns>Return ResolvedMapped to proceed</returns>
        URLMapping.ResolvedMapped URLResolver(string requestFilePath);
    }
}