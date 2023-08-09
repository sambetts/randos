using System;
using System.Collections.Generic;
using System.Linq;

namespace Models.Entities
{
    /// <summary>
    /// Debate container used for loading data only. Basically a logical view on lots of related answers.
    /// </summary>
    public class DebateDTO
    {
        #region Constructors & Privates

        public DebateDTO()
        {
            this.PlayerRanking = new List<PlayerRanking>();
        }
        public DebateDTO(AnswerWithRatingsTreeNode rootAnswer) : this()
        {
            this.RootAnswerWithRating = rootAnswer;

            var scoresAllAnswers = new List<PlayerRanking>();

            // Figure out winner from scores
            ComputeScores(this.RootAnswerWithRating, scoresAllAnswers);

            AnswerCount = scoresAllAnswers.Count;

            var scores = new Dictionary<DebateUser, int>();
            foreach (var scoredAnswer in scoresAllAnswers.OrderByDescending(s=> s.Score))
            {
                if (!scores.ContainsKey(scoredAnswer.User))
                {
                    scores.Add(scoredAnswer.User, scoredAnswer.Score);
                }
                else
                {
                    scores[scoredAnswer.User] += scoredAnswer.Score;
                }
            }
            foreach (var scoreKvP in scores)
            {
                this.PlayerRanking.Add(new PlayerRanking { User = scoreKvP.Key, Score = scoreKvP.Value });
            }

            this.Tags = new List<string>();
        }


        #endregion

        private void ComputeScores(AnswerWithRatingsTreeNode rootAnswer, List<PlayerRanking> scores)
        {
            var exisingScore = scores.Where(s => s.User == rootAnswer.CreatedBy).SingleOrDefault();
            // Add each player
            if (exisingScore != null)
            {
                exisingScore.Score += rootAnswer.TotalScore;
            }
            else
            {
                scores.Add(new PlayerRanking(rootAnswer.CreatedBy, rootAnswer.TotalScore));
            }

            ScoringRules rules = ScoringRules.LoadRules();

            // Score each child answer
            foreach (var childAnswer in rootAnswer.ChildAnswers)
            {
                ComputeScores(new AnswerWithRatingsTreeNode(childAnswer, rules), scores);
            }
        }

        #region Props

        /// <summary>
        /// Current winner
        /// </summary>
        public DebateUser Winner
        {
            get
            {
                if (PlayerRanking.Count > 0)
                {
                    // Ordered list is smallest (score) to largest.
                    return PlayerRanking.OrderByDescending(r => r.Score).First().User;
                }
                else
                {
                    return null;
                }
            }
        }

        public string Title => RootAnswerWithRating?.Title;
        public string TitleWebSafe => string.IsNullOrEmpty(Title) ? string.Empty : Utils.StringUtils.GetWebSafeString(Title);
        public AnswerWithRatingsTreeNode RootAnswerWithRating { get; set; }

        public int AnswerCount { get; set; }

        public List<PlayerRanking> PlayerRanking { get; set; }

        public List<string> Tags { get; set; }

        #endregion
    }
}
