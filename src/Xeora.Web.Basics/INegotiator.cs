using Xeora.Web.Basics.Service;

namespace Xeora.Web.Basics
{
    public interface INegotiator
    {
        IHandler GetHandler(string handlerId);
        IVariablePool GetVariablePool(string sessionId, string keyId);
        //void TransferVariablePool(string keyId, string fromSessionId, string toSessionId);
        Domain.IDomain CreateNewDomainInstance(string[] domainIdAccessTree, string domainLanguageId);
        IStatusTracker StatusTracker { get; }
        ITaskSchedulerEngine TaskScheduler { get; }
        Configuration.IXeora XeoraSettings { get; }
        void ClearCache();
    }
}
