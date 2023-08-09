using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class ResponseType : Abstract.BaseModel
    {
        public string ResponseText { get; set; }

        public bool? AgreeWithParent { get; set; }
    }
}
