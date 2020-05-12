using System.Collections.Generic;

namespace Lib
{
    public interface IBlobModel<TBlobDocument>
    {
        string Name { get; set; }
        TBlobDocument Content { get; set; }
        IDictionary<string, string> Metadata { get; set; }
    }
}