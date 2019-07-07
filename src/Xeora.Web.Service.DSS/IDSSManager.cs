namespace Xeora.Web.Service.Dss
{
    public interface IDssManager
    {
        void Reserve(string uniqueId, int reservationTimeout, out Basics.Dss.IDss reservationObject);
    }
}
