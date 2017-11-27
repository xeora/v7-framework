namespace Xeora.Web.Controller.Directive.Control
{
    public class AttributeInfo
    {
        public AttributeInfo(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        public string Key { get; private set; }
        public string Value { get; private set; }
    }
}