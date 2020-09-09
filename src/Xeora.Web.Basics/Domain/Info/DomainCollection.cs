using System.Collections.Generic;

namespace Xeora.Web.Basics.Domain.Info
{
    public class DomainCollection : List<Domain>
    {
        private readonly Dictionary<string, int> _NameIndexMap;

        public DomainCollection() =>
            this._NameIndexMap = new Dictionary<string, int>();

        public new void Add(Domain value)
        {
            base.Add(value);

            this._NameIndexMap.Add(value.Id, Count - 1);
        }

        public void Remove(string id)
        {
            if (!this._NameIndexMap.ContainsKey(id)) return;
            
            RemoveAt(this._NameIndexMap[id]);

            this._NameIndexMap.Clear();

            // Rebuild, NameIndexMap
            int index = 0;
            foreach (Domain item in this)
            {
                this._NameIndexMap.Add(item.Id, index);
                index += 1;
            }
        }

        public new void Remove(Domain value) =>
            this.Remove(value.Id);

        public new Domain this[int index]
        {
            get => index < this.Count ? base[index] : null;
            set
            {
                this.Remove(value.Id);
                this.Add(value);
            }
        }

        public Domain this[string id]
        {
            get => this._NameIndexMap.ContainsKey(id) ? base[this._NameIndexMap[id]] : null;
            set
            {
                this.Remove(value.Id);
                this.Add(value);
            }
        }
    }
}