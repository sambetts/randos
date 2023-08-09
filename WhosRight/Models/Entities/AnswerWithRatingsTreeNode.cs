using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    /// <summary>
    /// Answer + ratings
    /// </summary>
    public class AnswerWithRatingsTreeNode : AnswerDataOnlyTreeNode
    {
        #region Constructors

        public AnswerWithRatingsTreeNode() { }
        public AnswerWithRatingsTreeNode(AnswerDataOnlyTreeNode answer) : this(answer, ScoringRules.LoadRules())
        {
        }
        public AnswerWithRatingsTreeNode(AnswerDataOnlyTreeNode answer, ScoringRules rules) : this(rules)
        {
            this.ID = answer.ID;
            this.Body = answer.Body;
            this.ChildAnswers = answer.ChildAnswers;
            this.CreatedBy = answer.CreatedBy;
            this.ParentID = answer.ParentID;
            this.ReactionToParent = answer.ReactionToParent;
            this.Title = answer.Title;

            this.GenerateScore();
        }

        // Called by context GetAnswerTree
        public AnswerWithRatingsTreeNode(ScoringRules rules)
        {
            this.RatingsGenerated = new List<AutoRating>();
            this.Rules = rules;
        }

        #endregion


        public ScoringRules Rules { get; set; }

        /// <summary>
        /// All the ratings this answer has generated automatically. 
        /// </summary>
        public List<AutoRating> RatingsGenerated { get; set; }


        /// <summary>
        /// Builds new ratings tree each time
        /// </summary>
        public List<AnswerWithRatingsTreeNode> ChildAnswersWithRatings
        {
            get
            {
                List<AnswerWithRatingsTreeNode> r = new List<AnswerWithRatingsTreeNode>();
                foreach (var item in this.ChildAnswers)
                {
                    r.Add(new AnswerWithRatingsTreeNode(item, this.Rules));
                }
                return r;
            }
        }

        /// <summary>
        /// Total sum of all ratings in this answer
        /// </summary>
        public int TotalScore
        {
            get
            {
                int total = 0;
                foreach (var r in this.RatingsGenerated)
                {
                    total += r.Score;
                }
                return total;
            }
        }

        public override string ToString()
        {
            return $"{this.Title} (Score: {this.TotalScore}; RatingsGenerated: {RatingsGenerated.Count})";
        }


        private void GenerateScore()
        {
            // Score based on JSon file rules
            foreach (var rule in Rules)
            {
                if (rule.IsValidRule)
                {
                    // Does the rule hit?
                    if (rule.IsCompliant(this))
                    {
                        // Add the rating modifier
                        this.RatingsGenerated.Add(new AutoRating(rule.Reaction.PointsModifier, rule.Reaction.Message));
                        Console.WriteLine($"'{this.Title}' gets rating {rule.Reaction}");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Invalid rules");
                }
            }
        }
    }
}
