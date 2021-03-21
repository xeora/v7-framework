namespace Xeora.Web.Service.Dss
{
    public interface IManager
    {
        // Returns If Reservation was created before (true: created before, false: newly created)
        bool Reserve(string uniqueId, short reservationTimeout, out Basics.Dss.IDss reservationObject);
    }
}
