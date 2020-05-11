using System.Collections.Generic;

namespace Lib
{
    public class BlobModel : IBlobModel
    {
        public string Name { get; set; }
        public object Content { get; set; }
        public IDictionary<string, string> Metadata { get; set; }
    }
}