using System;
using System.Collections.Generic;
using System.Text;

namespace ModelFactory
{
    public class Debate : BaseEFClass
    {
        // For EF
        public Debate() 
        {
            this.answers = new List<Answer>();
            this.tags = new List<DebateTags>();

        }

        public Debate(string title, string body, DebateUser byWho)
        {
            this.answers = new List<Answer>();
            this.answers.Add(new Answer(this, title, body, byWho));
            this.tags = new List<DebateTags>();
        }

        public virtual List<Answer> answers { get; set; }

        public virtual List<DebateTags> tags { get; set; }

        public virtual Answer root_answer
        {
            get
            {
                foreach (var a in this.answers)
                {
                    if (a.parent_answer == null)
                    {
                        return a;
                    }
                }
                throw new ArgumentOutOfRangeException("We should have a root");
            }
        }
    }
}
