using API.Models;
using Azure;
using Azure.Data.Tables;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace API.Controllers
{
    public class TeamsAppController : ApiController
    {
        const string TABLE_NAME = "Sessions";
        private TableClient _tableClient;
        public TeamsAppController()
        {
            _tableClient = new TableClient(
                System.Configuration.ConfigurationManager.ConnectionStrings["Storage"]?.ConnectionString,
                TABLE_NAME);
        }

        // POST api/TeamsApp/NewSession?captchaResponseOnPage={captchaResponseOnPage}
        [HttpPost]
        public async Task<Guid> NewSession(string captchaResponseOnPage)
        {
            // Validate recaptcha - https://developers.google.com/recaptcha/docs/verify
            var captchSecret = System.Configuration.ConfigurationManager.AppSettings["CaptchaSecret"];

            var httpClient = new HttpClient();
            var uri = $"https://www.google.com/recaptcha/api/siteverify?secret={captchSecret}&response={captchaResponseOnPage}";

            var response = await httpClient.GetAsync(uri);
            var responseBody = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Captcha validation failed");
            }
            var captchaResult = JsonConvert.DeserializeObject<CaptchaResponse>(responseBody);

            if (captchaResult.Success)
            {
                // Generate new session now we know it's a human
                return await GetNewSession();
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

        async Task<UserSession> GetSessionId(string sessionId)
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

            var sesh = await GetSessionId(sessionId);

            if (sesh == null) // No session with that ID
                return Request.CreateResponse(HttpStatusCode.NotFound);


            // Save file


            // Remember deets
            sesh.AppDetails = appDetails;
            await _tableClient.UpdateEntityAsync<UserSession>(sesh, sesh.ETag);


            var response = Request.CreateResponse(HttpStatusCode.OK);
            var manifest = appDetails.ToTeamsAppManifest(url);

            byte[] bytes = manifest.BuildZip(appDetails.EntityName);
            using (var stream = new MemoryStream(bytes))
            {
                response.Content = new StreamContent(stream);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = $"{appDetails.EntityName}.zip"
                };
                return response;
            }
        }
    }
}
