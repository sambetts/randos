using Freezer.Core;
using Freezer.Engines;
using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;

namespace API.Controllers
{
    public class ScreenshotController : ApiController
    {

        // GET api/Screenshot?url={url}
        public HttpResponseMessage Get(string url)
        {
            // https://github.com/haga-rak/Freezer/wiki
            var screenshotJob = ScreenshotJobBuilder.Create(url)
                          .SetBrowserSize(800, 550)
                          .SetCaptureZone(CaptureZone.VisibleScreen)
                          .SetTrigger(new WindowLoadTrigger()); // Set when the picture is taken

            
            // This engine isn't the most reliable, so we try a few times. It almost always works in the end.
            int retries = 0;
            Screenshot screenshot = null;
            while (screenshot == null && retries < 5)
            {
                screenshot = GetScreenShotAndRestartEngineIfNeeded(screenshotJob);
                retries++;
            }
            
            var base64 = Convert.ToBase64String(screenshot.AsBytes());
            Console.WriteLine(base64.Length);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(base64, System.Text.Encoding.UTF8);
            return response;

        }

        private Screenshot GetScreenShotAndRestartEngineIfNeeded(ScreenshotJob screenshotJob)
        {
            Screenshot screenshot = null;
            try
            {
                InitializeFreezer();
                screenshot = screenshotJob.Freeze();
            }
            catch (CaptureEngineException ex)
            {
                if (ex.Message != "RemoteWorker has shutdown")
                {
                    //If an error occurs that isn't already shutting down, attempt to dispose of the worker process
                    try { FreezerGlobalWorkerProcess()?.Dispose(); }
                    catch { /*Ignore Error*/ }
                }
                else
                    FreezerGlobal.Initialize(); // otherwise the worker process was disposed of. create a new one. don't use method since that checks active processes
            }

            return screenshot;
        }

        private void InitializeFreezer()
        {
            if (FreezerGlobalWorkerProcess() == null)
                FreezerGlobal.Initialize();
        }

        private IDisposable FreezerGlobalWorkerProcess()
        {
            var workers = typeof(FreezerGlobal).GetField("_workers", BindingFlags.NonPublic | BindingFlags.Static);
            return workers?.GetValue(null) as IDisposable;
        }

    }
}
