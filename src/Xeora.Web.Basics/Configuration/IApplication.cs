using System.Collections.Generic;

namespace Xeora.Web.Basics.Configuration
{
    public interface IApplication
    {
        IMain Main { get; }
        IRequestTagFilter RequestTagFilter { get; }
        IEnumerable<IMimeItem> CustomMimes { get; }
        string[] BannedFiles { get; }
    }
}
