using CommonUtils.Config;
using Microsoft.Extensions.Configuration;

namespace ModelFactory.Config
{
    public class CommonConfig : PropertyBoundConfig
    {
        public CommonConfig(IConfiguration config) : base(config)
        {
        }

        [ConfigSection("AzureB2C")] public AzureB2CConfig AzureB2CConfig { get; set; } = null!;
    }

    public class ServerSideKeyVaultSettings : CommonConfig
    {
        public ServerSideKeyVaultSettings(IConfiguration config) : base(config)
        {
        }

        [ConfigValue]
        public string CosmosDbUrl { get; set; }
        [ConfigValue] 
        public string CosmosDbKey { get; set; }

        [ConfigSection("ConnectionStrings")] public ConnectionStrings ConnectionStrings { get; set; } = null!;

    }

    public class ConnectionStrings : PropertyBoundConfig
    {
        public ConnectionStrings(IConfigurationSection config) : base(config)
        {
        }

        [ConfigValue]
        public string Storage { get; set; } = string.Empty;

        [ConfigValue]
        public string ServiceBusNewAnswersConnectionString { get; set; } = string.Empty;
        [ConfigValue]
        public string ServiceBusEmailConnectionString { get; set; } = string.Empty;

        [ConfigValue]
        public string DatabaseString { get; set; }

    }

    public class AzureB2CConfig : PropertyBoundConfig
    {
        public AzureB2CConfig(IConfiguration config) : base(config)
        {
        }
        public string Authority => $"{Instance}/tfp/{Domain}/{SignUpSignInPolicyId}";

        [ConfigValue]
        public string ClientId { get; set; } = string.Empty;

        [ConfigValue(true)]
        public string ClientSecret { get; set; } = string.Empty;
        [ConfigValue]
        public string TenantId { get; set; } = string.Empty;

        [ConfigValue]
        public string Domain { get; set; } = string.Empty;

        [ConfigValue]
        public string Instance { get; set; } = string.Empty;

        [ConfigValue]
        public string SignUpSignInPolicyId { get; set; }
    }
}
