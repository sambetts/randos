using API.Models;
using Azure;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace API.Controllers
{
    public class TeamsAppController : ApiController
    {
        const string TABLE_NAME = "Sessions";
        private TableClient _tableClient;
        private BlobContainerClient _blobClient;
        public TeamsAppController()
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Storage"]?.ConnectionString;
            _tableClient = new TableClient(
                connectionString,
                TABLE_NAME);

            _blobClient = new BlobContainerClient(connectionString, "manifests");

        }

        // POST api/TeamsApp/NewSession?captchaResponseOnPage={captchaResponseOnPage}
        [HttpPost]
        public async Task<HttpResponseMessage> NewSession(string captchaResponseOnPage)
        {
            // Validate recaptcha - https://developers.google.com/recaptcha/docs/verify
            var captchSecret = System.Configuration.ConfigurationManager.AppSettings["CaptchaSecret"];

            var httpClient = new HttpClient();
            var uri = $"https://www.google.com/recaptcha/api/siteverify?secret={captchSecret}&response={captchaResponseOnPage}";

            var recaptchaResponse = await httpClient.GetAsync(uri);
            var recaptchaResponseBody = await recaptchaResponse.Content.ReadAsStringAsync();
            if (!recaptchaResponse.IsSuccessStatusCode)
            {
                throw new Exception("Captcha validation failed");
            }
            var captchaResult = JsonConvert.DeserializeObject<CaptchaResponse>(recaptchaResponseBody);

            if (captchaResult.Success)
            {
                // Generate new session now we know it's a human
                var id = await GetNewSession();
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(id.ToString());
                return response;
            }
            else
            {
                throw new Exception("Captcha validation failed");
            }
        }

        private async Task<Guid> GetNewSession()
        {
            _tableClient.CreateIfNotExists();

            var id = Guid.NewGuid();
            var newRandomSession = new UserSession { PartitionKey = id.ToString() };
            await _tableClient.AddEntityAsync(newRandomSession);

            return id;
        }

        async Task<UserSession> GetSessionFromCache(string sessionId)
        {
            Response<UserSession> entityResponse = null;
            try
            {
                entityResponse = await _tableClient.GetEntityAsync<UserSession>(sessionId, UserSession.RowKeyStaticVal);
            }
            catch (RequestFailedException ex)
            {
                if (ex.ErrorCode == "ResourceNotFound")
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }

            return entityResponse;
        }

        // POST api/TeamsApp/CreateApp?url={url}&sessionId={sessionId}
        [HttpPost]
        public async Task<HttpResponseMessage> CreateApp([FromBody] AppDetails appDetails, string url, string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(url) || appDetails == null || !appDetails.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var sesh = await GetSessionFromCache(sessionId);

            if (sesh == null) // No session with that ID
                return Request.CreateResponse(HttpStatusCode.NotFound);

            var manifest = appDetails.ToTeamsAppManifest(url);
            var bytes = manifest.BuildZip(appDetails.EntityName);

            // Save file to blob storage
            await _blobClient.CreateIfNotExistsAsync();

            var fileName = $"{DateTime.Now.Ticks}/{appDetails.EntityName}.zip";
            Response<BlobContentInfo> manifestRef = null;
            using (var ms = new MemoryStream(bytes))
            {
                manifestRef = await _blobClient.UploadBlobAsync(fileName, ms);
            }

            // Remember deets in cache
            sesh.AppDetails = appDetails;
            sesh.SavedManifestUrl = fileName;
            await _tableClient.UpdateEntityAsync<UserSession>(sesh, sesh.ETag);

            // Respond with filename
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(fileName);
            return response;
        }

        // GET api/TeamsApp/DownloadApp?fileUrl={fileUrl}&sessionId={sessionId}
        [HttpGet]
        public async Task<HttpResponseMessage> DownloadApp(string fileUrl, string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(fileUrl))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var sesh = await GetSessionFromCache(sessionId);

            if (sesh == null) // No session with that ID
                return Request.CreateResponse(HttpStatusCode.NotFound);


            var blob = _blobClient.GetBlobClient(fileUrl);
            var exists = await blob.ExistsAsync();
            if (!exists)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            var url = blob.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.Now.AddMinutes(2));

            // Respond with filename
            var response = Request.CreateResponse(HttpStatusCode.Moved);
            response.Headers.Location = url;

            return response;
        }
    }
}
