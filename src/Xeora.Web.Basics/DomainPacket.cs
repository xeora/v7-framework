namespace Xeora.Web.Basics
{
    public class DomainPacket
    {
        public DomainPacket(string name, INegotiator negotiator)
        {
            this.Name = name;
            this.Negotiator = negotiator;
        }
        
        internal string Name { get; }
        internal INegotiator Negotiator { get; }
    }
}