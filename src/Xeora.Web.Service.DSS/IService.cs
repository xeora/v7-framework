namespace Xeora.Web.Service.Dss
{
    public interface IService
    {
        bool IsExpired { get; }
        void Extend();
    }
}
