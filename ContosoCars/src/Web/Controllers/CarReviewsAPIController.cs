using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using ContosoCars.Engine;
using ContosoCars.Engine.DB;

namespace ContosoCars.Controllers
{
    public class CarReviewsAPIController : ApiController
    {
        private DocParserDB db = new DocParserDB();

        // PUT: api/EmailReviewsAPI?sender=bob@contoso.com
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDBEmailReview(string sender, string fileName)
        {

            if (string.IsNullOrEmpty(sender))
            {
                return BadRequest("No sender param");
            }
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest("No fileName param");
            }
            var fileData = await Request.Content.ReadAsByteArrayAsync();
            if (fileData == null || fileData.Length == 0)
            {
                return BadRequest("No file data");
            }

            var doc = await DBEmailReview.BuildFrom(fileData, sender, fileName, new CognitiveConfigConfigReader());
            db.Reviews.Add(doc);

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}