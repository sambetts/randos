using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Answer : Abstract.BaseModel
    {
        public string Title { get; set; }
        public DebateUser ByUser { get; set; }
        public ResponseType ReactionToParent { get; set; }
        public string Body { get; set; }

    }
}
