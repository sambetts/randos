using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Controllers
{
    /// <summary>
    /// Handles React app requests for app building
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class BuildAppController : ControllerBase
    {
        private readonly Config _config;

        public BuildAppController(Config config)
        {
            this._config = config;
        }

        // Generate a Teams app
        // POST: AppConfiguration/ServiceConfiguration
        [HttpPost()]
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
                }
            };
        }

    }
}