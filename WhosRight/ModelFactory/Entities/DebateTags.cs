using System;
using System.Collections.Generic;
using System.Text;

namespace ModelFactory
{
    public class DebateTags : BaseEFClass
    {
        public virtual Tag tag { get; set; }
        public virtual Debate debate { get; set; }
    }
}
