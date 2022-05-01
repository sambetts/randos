using ContosoCars.Engine.DB;
using ContosoCarsML.Model;
using System;

namespace ContosoCars.Models
{
    public class ReviewApproveFormModel
    {
        public ReviewApproveFormModel()
        {
            this.AutoAccept = false;
        }
        public ReviewApproveFormModel(DBEmailReview dBEmailReview) : this()
        {
            this.Review = dBEmailReview ?? throw new ArgumentNullException(nameof(dBEmailReview));
            this.AcceptOverride = dBEmailReview.Accepted;
        }

        public DBEmailReview Review { get; set; }

        public ModelOutput AIModel { get; set; }

        public bool? AcceptOverride { get; set; }

        public bool AutoAccept { get; set; }
    }
}