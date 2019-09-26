namespace Xeora.Web.Basics.Context
{
    public interface IKeyValueCollection<TK, out TV>
    {
        TK[] Keys { get; }
        TV this[TK key] { get; }
    }
}
