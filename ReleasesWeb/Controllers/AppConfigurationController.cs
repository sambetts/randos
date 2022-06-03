using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Mvc;
using ReleasesWeb.Models;
using SPO.ColdStorage.Web.Models;

namespace SPO.ColdStorage.Web.Controllers
{
    /// <summary>
    /// Handles React app requests for app configuration
    /// </summary>
    [Microsoft.AspNetCore.Authorization.Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AppConfigurationController : ControllerBase
    {
        private readonly Config _config;

        public AppConfigurationController(Config config)
        {
            this._config = config;
        }


        // Generate app ServiceConfiguration + storage configuration + key to read blobs
        // GET: AppConfiguration/ServiceConfiguration
        [HttpGet("[action]")]
        public ActionResult<ServiceConfiguration> GetServiceConfiguration()
        {
            var client = new BlobServiceClient(_config.ConnectionStrings.Storage);

            // Generate a new shared-access-signature
            var sasUri = client.GenerateAccountSasUri(AccountSasPermissions.List | AccountSasPermissions.Read,
                DateTime.Now.AddDays(1),
                AccountSasResourceTypes.Container | AccountSasResourceTypes.Object);

            // Return for react app
            return new ServiceConfiguration 
            {
                StorageInfo = new StorageInfo
                {
                    AccountURI = client.Uri.ToString(),
                    SharedAccessToken = sasUri.Query,
                    ContainerName = _config.BlobContainerName
                },
                SearchConfiguration = new SearchConfiguration 
                {
                    IndexName = _config.SearchConfig.IndexName,
                    QueryKey = _config.SearchConfig.QueryKey, 
                    ServiceName = _config.SearchConfig.ServiceName
                }
            };
        }

    }
}