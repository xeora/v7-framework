namespace Xeora.Web.Basics.ControlResult
{
    public interface IDataSource
    {
        DataSourceTypes Type { get; }

        Message Message { get; set; }
        long Count { get; }
        long Total { get; set; }

        object GetResult();
    }
}
