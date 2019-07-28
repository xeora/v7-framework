using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Xeora.Web.Basics;

namespace Xeora.Web.Application.Domain
{
    public class MetaRecord : IMetaRecordCollection
    {
        private readonly ConcurrentDictionary<Basics.MetaRecord.Tags, string> _Records;
        private readonly ConcurrentDictionary<string, string> _CustomRecords;

        public MetaRecord()
        {
            this._Records = new ConcurrentDictionary<Basics.MetaRecord.Tags, string>();
            this._CustomRecords = new ConcurrentDictionary<string, string>();
        }

        public void Add(Basics.MetaRecord.TagSpaces tagSpace, string name, string value)
        {
            if (string.IsNullOrEmpty(name))
                throw new NullReferenceException("Name can not be null!");

            if (string.IsNullOrEmpty(value))
                value = string.Empty;

            switch (tagSpace)
            {
                case Basics.MetaRecord.TagSpaces.name:
                    name = $"name::{name}";

                    break;
                case Basics.MetaRecord.TagSpaces.httpequiv:
                    name = $"httpequiv::{name}";

                    break;
                case Basics.MetaRecord.TagSpaces.property:
                    name = $"property::{name}";

                    break;
            }

            this._CustomRecords.AddOrUpdate(name, value, (cName, cValue) => value);
        }

        public void Add(Basics.MetaRecord.Tags tag, string value)
        {
            if (string.IsNullOrEmpty(value))
                value = string.Empty;

            this._Records.AddOrUpdate(tag, value, (cName, cValue) => value);
        }

        public void Remove(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new NullReferenceException("Name can not be null!");

            this._CustomRecords.TryRemove(name, out string dummy);
        }

        public void Remove(Basics.MetaRecord.Tags tag) =>
            this._Records.TryRemove(tag, out string dummy);

        public KeyValuePair<Basics.MetaRecord.Tags, string>[] CommonTags
        {
            get
            {
                KeyValuePair<Basics.MetaRecord.Tags, string>[] metaTags =
                    new KeyValuePair<Basics.MetaRecord.Tags, string>[this._Records.Keys.Count];

                int keyCount = 0;
                foreach (Basics.MetaRecord.Tags key in this._Records.Keys)
                {
                    this._Records.TryGetValue(key, out string value);

                    metaTags[keyCount] = new KeyValuePair<Basics.MetaRecord.Tags, string>(key, value);
                    keyCount++;
                }

                return metaTags;
            }
        }

        public KeyValuePair<string, string>[] CustomTags
        {
            get
            {
                KeyValuePair<string, string>[] metaTags =
                    new KeyValuePair<string, string>[this._CustomRecords.Keys.Count];

                int keyCount = 0;
                foreach (string key in this._CustomRecords.Keys)
                {
                    this._CustomRecords.TryGetValue(key, out string value);

                    metaTags[keyCount] = new KeyValuePair<string, string>(key, value);
                    keyCount++;
                }

                return metaTags;
            }
        }
    }
}
