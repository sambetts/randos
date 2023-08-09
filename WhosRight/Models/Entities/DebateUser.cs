using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class DebateUser : Abstract.BaseModel
    {
        public string Email { get; set; }
        public string DisplayName { get; set; }

        public override string ToString()
        {
            return $"{DisplayName} ({Email})";
        }
    }
}
