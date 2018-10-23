namespace Xeora.Web.Service.DSS
{
    public interface IDSSService
    {
        bool IsExpired { get; }
        void Extend();
    }
}
