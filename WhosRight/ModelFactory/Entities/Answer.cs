using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelFactory
{
    public class Answer : BaseEFClass
    {
        public Answer(Debate debate, string title, string body, DebateUser byWho) : this(debate, title, body, byWho, null)
        {
        }
        public Answer(Debate debate, string title, string body, DebateUser byWho, ResponseType reactionToParent) : this()
        {
            this.title = title;
            this.body = body;
            this.user = byWho;
            this.reaction_to_parent = reactionToParent;
            this.parent_debate = debate;
        }
        
        public Answer()
        {
            this.child_answers = new HashSet<Answer>();
        }

        [Required]
        public virtual DebateUser user { get; set; }

        [Required]
        public string title { get; set; }

        public Debate parent_debate { get; set; }
        public Answer parent_answer { get; set; }

        public string body { get; set; }

        public virtual List<Link> references { get; set; }

        public ResponseType reaction_to_parent { get; set; }

        

        /// <summary>
        /// Attaches answer as child of this one. Returns the modified answer.
        /// </summary>
        /// <param name="responseAnswer"></param>
        /// <returns></returns>
        public Answer AddResponse(string title, string body, DebateUser byWho, ResponseType reactionToParent)
        {
            Answer responseAnswer = new Answer(this.parent_debate, title, body, byWho, reactionToParent);
            responseAnswer.parent_answer = this;
            this.child_answers.Add(responseAnswer);

            return responseAnswer;
        }

        public ICollection<Answer> child_answers { get; set; }

        public Models.Answer ToModel()
        {
            return new Models.Answer
            {
                Body = this.body,
                ByUser = this.user?.ToModel(),
                ID = this.id,
                ReactionToParent = this.reaction_to_parent?.ToModel(),
                Title = this.title
            };
        }
    }
}
