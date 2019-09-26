using System.Collections.Generic;

namespace Xeora.Web.Basics
{
    public interface IMetaRecordCollection
    {
        void Add(MetaRecord.TagSpaces tagSpace, string name, string value);
        void Add(MetaRecord.Tags tag, string value);
        void Remove(string name);
        void Remove(MetaRecord.Tags tag);

        IEnumerable<KeyValuePair<MetaRecord.Tags, string>> CommonTags { get; }
        IEnumerable<KeyValuePair<string, string>> CustomTags { get; }
    }
}
