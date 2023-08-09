using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ModelFactory
{
    public abstract class BaseEFClass
    {
        [Key]
        public int id { get; set; }

        public override string ToString()
        {
            return $"id: {id}";
        }
    }
}
