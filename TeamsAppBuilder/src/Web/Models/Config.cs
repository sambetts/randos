namespace Models
{

    public class Config : BaseConfig
    {
        public Config(IConfiguration config) : base(config)
        {
        }


        [ConfigValue]
        public string BaseServerAddress { get; set; } = string.Empty;

        public string ServiceBusQueueName => "filediscovery";

        [ConfigValue]
        public string KeyVaultUrl { get; set; } = string.Empty;


        [ConfigValue]
        public string BlobContainerName { get; set; } = string.Empty;

        [ConfigValue(true)]
        public string AppInsightsInstrumentationKey { get; set; } = string.Empty;

        public bool HaveAppInsightsConfigured => !string.IsNullOrEmpty(AppInsightsInstrumentationKey);

        [ConfigSection("ConnectionStrings")]
        public ConnectionStrings ConnectionStrings { get; set; } = null!;

    }

    public class ConnectionStrings : BaseConfig
    {
        public ConnectionStrings(IConfigurationSection config) : base(config)
        {
        }

        [ConfigValue]
        public string Storage { get; set; } = string.Empty;

        [ConfigValue]
        public string SQLConnectionString { get; set; } = string.Empty;

        [ConfigValue]
        public string ServiceBus { get; set; } = string.Empty;

    }

    public class ConfigException : Exception
    {
        public ConfigException(string message) : base(message) { }
    }
}
