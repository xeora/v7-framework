using System.Collections.Generic;

namespace Xeora.Web.Basics.Domain.Info
{
    public class DomainCollection : List<Domain>
    {
        private readonly Dictionary<string, int> _NameIndexMap;

        public DomainCollection() : base() =>
            this._NameIndexMap = new Dictionary<string, int>();

        public new void Add(Domain value)
        {
            base.Add(value);

            this._NameIndexMap.Add(value.Id, base.Count - 1);
        }

        public void Remove(string Id)
        {
            if (this._NameIndexMap.ContainsKey(Id))
            {
                base.RemoveAt(this._NameIndexMap[Id]);

                this._NameIndexMap.Clear();

                // Rebuild, NameIndexMap
                int Index = 0;
                foreach (Domain item in this)
                {
                    this._NameIndexMap.Add(item.Id, Index);

                    Index += 1;
                }
            }
        }

        public new void Remove(Domain value) =>
            this.Remove(value.Id);

        public new Domain this[int index]
        {
            get
            {
                if (index < this.Count)
                    return base[index];

                return null;
            }
            set
            {
                this.Remove(value.Id);
                this.Add(value);
            }
        }

        public Domain this[string Id]
        {
            get
            {
                if (this._NameIndexMap.ContainsKey(Id))
                    return base[this._NameIndexMap[Id]];

                return null;
            }
            set
            {
                this.Remove(value.Id);
                this.Add(value);
            }
        }
    }
}