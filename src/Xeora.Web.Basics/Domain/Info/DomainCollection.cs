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

            this._NameIndexMap.Add(value.ID, base.Count - 1);
        }

        public void Remove(string ID)
        {
            if (this._NameIndexMap.ContainsKey(ID))
            {
                base.RemoveAt(this._NameIndexMap[ID]);

                this._NameIndexMap.Clear();

                // Rebuild, NameIndexMap
                int Index = 0;
                foreach (Domain item in this)
                {
                    this._NameIndexMap.Add(item.ID, Index);

                    Index += 1;
                }
            }
        }

        public new void Remove(Domain value) =>
            this.Remove(value.ID);

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
                this.Remove(value.ID);
                this.Add(value);
            }
        }

        public Domain this[string ID]
        {
            get
            {
                if (this._NameIndexMap.ContainsKey(ID))
                    return base[this._NameIndexMap[ID]];

                return null;
            }
            set
            {
                this.Remove(value.ID);
                this.Add(value);
            }
        }
    }
}