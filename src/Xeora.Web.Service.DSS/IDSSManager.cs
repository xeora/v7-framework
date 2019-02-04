namespace Xeora.Web.Service.DSS
{
    public interface IDSSManager
    {
        void Reserve(string uniqueID, int reservationTimeout, out Basics.DSS.IDSS reservationObject);
    }
}
