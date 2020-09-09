using System.Collections;
using System.Collections.Generic;

namespace Xeora.Web.Basics.Mapping
{
    public class MappingItemCollection : IEnumerable
    {
        private readonly List<MappingItem> _Items;

        public MappingItemCollection(MappingItem[] items)
        {
            this._Items = new List<MappingItem>();
            this.AddRange(items);
        }

        public MappingItem this[int index] => this._Items[index];

        public void Add(MappingItem item)
        {
            this._Items.Add(item);
            this.Sort();
        }

        public void AddRange(MappingItem[] items)
        {
            if (items == null)
                return;
            
            this._Items.AddRange(items);
            this.Sort();
        }

        public void Sort() =>
            this._Items.Sort(new PriorityComparer());

        public MappingItem[] ToArray() => this._Items.ToArray();

        public IEnumerator GetEnumerator() => 
            this._Items.GetEnumerator();

        private class PriorityComparer : IComparer<MappingItem>
        {
            public int Compare(MappingItem x, MappingItem y)
            {
                if (x.Priority > y.Priority) return -1;
                if (x.Priority < y.Priority) return 1;

                return 0;
            }
        }
    }
}
