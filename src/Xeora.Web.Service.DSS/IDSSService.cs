namespace Xeora.Web.Service.Dss
{
    public interface IDssService
    {
        bool IsExpired { get; }
        void Extend();
    }
}
