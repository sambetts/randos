namespace SPO.ColdStorage.Web.Models
{
    public class ServiceConfiguration
    {
        public StorageInfo StorageInfo { get; set; } = new StorageInfo();

        public SearchConfiguration SearchConfiguration { get; set; } = new SearchConfiguration();
    }

    public class StorageInfo
    {
        public string SharedAccessToken { get; set; } = string.Empty;
        public string AccountURI { get; set; } = string.Empty;
        public string ContainerName { get; set; } = string.Empty;
    }
}
