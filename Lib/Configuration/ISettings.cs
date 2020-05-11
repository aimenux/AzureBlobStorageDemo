using System.Collections.Generic;

namespace Lib.Configuration
{
    public interface ISettings
    {
        string ConnectionString { get; }
        string ContainerName { get; }
    }
}
