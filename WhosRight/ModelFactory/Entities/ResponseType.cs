using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ModelFactory
{
    public class ResponseType: BaseEFClass
    {

        [Required]
        public string response_text { get; set; }

        public bool? agree_with_parent { get; set; }

        public Models.ResponseType ToModel()
        {
            return new Models.ResponseType() 
            {
                ID = this.id,
                AgreeWithParent = this.agree_with_parent,
                ResponseText = this.response_text
            };
        }
    }
}
