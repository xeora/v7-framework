namespace Xeora.Web.Service.Dss
{
    public interface IManager
    {
        void Reserve(string uniqueId, int reservationTimeout, out Basics.Dss.IDss reservationObject);
    }
}
