using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    /// <summary>
    /// An answer is some kind of reaction to a parent answer. For tree representation.
    /// </summary>
    public class AnswerDataOnlyTreeNode
    {
        #region Constructors

        public AnswerDataOnlyTreeNode(Answer databaseAnswer) : this()
        {
            this.ID = databaseAnswer.ID;
            this.Title = databaseAnswer.Title;
            this.CreatedBy = databaseAnswer.ByUser;
            this.ReactionToParent = databaseAnswer.ReactionToParent;
            this.Body = databaseAnswer.Body;
        }

        // Called by context GetAnswerTree
        public AnswerDataOnlyTreeNode()
        {
            this.ChildAnswers = new List<AnswerDataOnlyTreeNode>();
        }

        #endregion


        #region Props

        /// <summary>
        /// All answers one level below this one.
        /// </summary>
        public List<AnswerDataOnlyTreeNode> ChildAnswers { get; set; }

        public int ID { get; set; }

        public int? ParentID { get; set; }


        /// <summary>
        /// Who made this answer
        /// </summary>
        public DebateUser CreatedBy { get; set; }

        /// <summary>
        /// The one-line answer
        /// </summary>
        public string Title { get; set; }
        public string Body { get; set; }

        /// <summary>
        /// The DB path. Not used for app logic
        /// </summary>
        public string LoadPath { get; set; }

        /// <summary>
        /// What kind of reaction this answer is to the parent
        /// </summary>
        public ResponseType ReactionToParent { get; set; }

        #endregion

        public override string ToString()
        {
            return $"{Title}";
        }
    }
}
