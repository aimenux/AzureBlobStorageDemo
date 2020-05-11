namespace Lib.Configuration
{
    public class Settings : ISettings
    {
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
    }
}