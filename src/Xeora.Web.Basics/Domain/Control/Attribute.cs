namespace Xeora.Web.Basics.Domain.Control
{
    public class Attribute
    {
        public Attribute(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        public string Key { get; private set; }
        public string Value { get; private set; }
    }
}