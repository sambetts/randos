using Freezer.Core;
using System;
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
                          .SetBrowserSize(1024, 650)
                          .SetCaptureZone(CaptureZone.VisibleScreen) // Set what should be captured
                          .SetTrigger(new WindowLoadTrigger()); // Set when the picture is taken

            var bytes = screenshotJob.Freeze();
            var base64 = Convert.ToBase64String(bytes.AsBytes());
            Console.WriteLine(base64.Length);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(base64, System.Text.Encoding.UTF8);
            return response;

        }

    }
}
