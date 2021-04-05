namespace Xeora.Web.Basics
{
    public interface IStatusTracker
    {
        int Get(short statusCode);
        int GetRange(short min, short max);
        int Get1xx();
        int Get2xx();
        int Get3xx();
        int Get4xx();
        int Get5xx();
    }
}
