using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class AutoRating : Abstract.BaseModel
    {
        public AutoRating(int score, string reason)
        {
            this.Score = score;
            this.Reason = reason;
        }

        public int Score { get; set; }
        public string Reason { get; set; }

        public override string ToString()
        {
            return $"{this.Reason} ({this.Score})";
        }
    }
}
