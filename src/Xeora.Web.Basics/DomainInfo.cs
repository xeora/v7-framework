using System.Collections.Generic;

namespace Xeora.Web.Basics
{
    public class DomainInfo
    {
        public enum DeploymentTypes
        {
            Development,
            Release
        }

        public DomainInfo(DeploymentTypes deploymentType, string ID, LanguageInfo[] languages)
        {
            this.DeploymentType = deploymentType;
            this.ID = ID;
            this.Languages = languages;
            this.Children = new DomainInfoCollection();
        }

        public DeploymentTypes DeploymentType { get; private set; }
        public string ID { get; private set; }
        public LanguageInfo[] Languages { get; private set; }
        public DomainInfoCollection Children { get; private set; }

        public class LanguageInfo
        {
            public LanguageInfo(string ID, string name)
            {
                this.ID = ID;
                this.Name = name;
            }

            public string ID { get; private set; }
            public string Name { get; private set; }
        }

        public class DomainInfoCollection : List<DomainInfo>
        {
            private Dictionary<string, int> _NameIndexMap;

            public DomainInfoCollection() : base() =>
                this._NameIndexMap = new Dictionary<string, int>();

            public new void Add(DomainInfo value)
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
                    foreach (DomainInfo item in this)
                    {
                        this._NameIndexMap.Add(item.ID, Index);

                        Index += 1;
                    }
                }
            }

            public new void Remove(DomainInfo value) =>
                this.Remove(value.ID);

            public new DomainInfo this[int index]
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

            public DomainInfo this[string ID]
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
}
