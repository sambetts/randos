using CommonUtils.Config;
using Microsoft.Extensions.Configuration;

namespace ModelFactory.Config
{
    /// <summary>
    /// All client settings used in app
    /// </summary>
    public class ClientSideKeyVaultSettings : CommonConfig
    {
        
        public ClientSideKeyVaultSettings(IConfiguration config) : base(config)
        {
        }

        [ConfigValue]
        public string BackendApiBaseUrl { get; set; }

    }
}
