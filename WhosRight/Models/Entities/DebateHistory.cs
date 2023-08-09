using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Entities
{
    public class DebateHistory
    {
        public DebateUser User { get; set; }
        public DateTime ChangeDateTime { get; set; }

        public Guid ID { get; set; }
    }
}
