namespace Xeora.Web.Basics.Domain.Control
{
    public class Attribute
    {
        public Attribute(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        public string Key { get; }
        public string Value { get; }
    }
}