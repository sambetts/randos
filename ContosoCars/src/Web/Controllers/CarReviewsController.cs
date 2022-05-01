using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using ContosoCars.Engine.DB;
using ContosoCars.Models;
using ContosoCars.Engine;
using ContosoCarsML.Model;

namespace ContosoCars.Controllers
{
    public class CarReviewsController : Controller
    {
        private DocParserDB db = new DocParserDB();

        // GET: EmailReviews
        public async Task<ActionResult> Index()
        {
            return View(await db.Reviews.OrderByDescending(r => r.Received).ToListAsync());
        }

        // GET: EmailReviews/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var dBEmailReview = await db.Reviews.FindAsync(id);
            if (dBEmailReview == null)
            {
                return HttpNotFound();
            }
            return View(dBEmailReview);
        }

        // GET: EmailReviews/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmailReviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(HttpPostedFileBase file, string sender)
        {
            byte[] fileData = null;
            if (file != null)
            {
                using (var reader = new BinaryReader(file.InputStream))
                {
                    fileData = reader.ReadBytes(file.ContentLength);
                }

                // Build new & save
                var newReview = await DBEmailReview.BuildFrom(fileData, sender, file.FileName, new CognitiveConfigConfigReader());
                await newReview.Save(db);


                return RedirectToAction(nameof(Index));
            }
            else
            {
                throw new ArgumentNullException(nameof(file));
            }
        }

        // GET: EmailReviews/AcceptReject/5
        public async Task<ActionResult> AcceptReject(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DBEmailReview dBEmailReview = await db.Reviews.FindAsync(id);
            if (dBEmailReview == null)
            {
                return HttpNotFound();
            }
            return View(new ReviewApproveFormModel(dBEmailReview));
        }

        // POST: EmailReviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AcceptReject(ReviewApproveFormModel reviewApproveFormModel)
        {
            var review = await db.Reviews.Where(r => r.ID == reviewApproveFormModel.Review.ID).SingleOrDefaultAsync();
            reviewApproveFormModel.Review = review;

            review.Accepted = reviewApproveFormModel.AcceptOverride;
            await db.SaveChangesAsync();

            if (reviewApproveFormModel.AutoAccept)
            {
                // Add input data
                var input = new ModelInput() { Filename = review.Filename, Keywords = review.KeyWords };

                // Load model and predict output of sample data
                ModelOutput result = ConsumeModel.Predict(input);
                reviewApproveFormModel.AIModel = result;

                return View(reviewApproveFormModel);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: EmailReviews/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DBEmailReview dBEmailReview = await db.Reviews.FindAsync(id);
            if (dBEmailReview == null)
            {
                return HttpNotFound();
            }
            return View(dBEmailReview);
        }

        // POST: EmailReviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            DBEmailReview dBEmailReview = await db.Reviews.FindAsync(id);
            db.Reviews.Remove(dBEmailReview);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
