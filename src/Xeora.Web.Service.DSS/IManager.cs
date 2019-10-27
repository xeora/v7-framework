namespace Xeora.Web.Service.Dss
{
    public interface IManager
    {
        void Reserve(string uniqueId, short reservationTimeout, out Basics.Dss.IDss reservationObject);
    }
}
