using System.Collections.Generic;

namespace Lib
{
    public class BlobModel<TBlobDocument> : IBlobModel<TBlobDocument>
    {
        public string Name { get; set; }
        public TBlobDocument Content { get; set; }
        public IDictionary<string, string> Metadata { get; set; }
    }
}