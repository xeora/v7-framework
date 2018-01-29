namespace Xeora.Web.Controller.Directive.Control
{
    public class AttributeDefinition
    {
        public AttributeDefinition(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        public string Key { get; private set; }
        public string Value { get; private set; }
    }
}