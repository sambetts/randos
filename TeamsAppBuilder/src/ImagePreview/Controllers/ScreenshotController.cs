using Freezer.Core;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace ImagePreview.Controllers
{
    public class ScreenshotController : ApiController
    {

        // GET api/Screenshot?url={url}
        public HttpResponseMessage Get(string url)
        {
            // https://github.com/haga-rak/Freezer/wiki
            var screenshotJob = ScreenshotJobBuilder.Create(url)
                          .SetBrowserSize(1366, 768)
                          .SetCaptureZone(CaptureZone.VisibleScreen) // Set what should be captured
                          .SetTrigger(new WindowLoadTrigger()); // Set when the picture is taken

            var response = new HttpResponseMessage();
            var bytes = screenshotJob.Freeze();

            if (bytes != null)
            {
                response.StatusCode = HttpStatusCode.OK;
                response.Content = new ByteArrayContent(bytes);

                string contentDisposition = string.Concat("attachment; filename=", "screenshot.jpg");
                response.Content.Headers.ContentDisposition =
                              ContentDispositionHeaderValue.Parse(contentDisposition);

                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            }
            
            return response;
        }

    }
}
