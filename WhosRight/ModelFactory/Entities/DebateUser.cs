using System;
using System.ComponentModel.DataAnnotations;

namespace ModelFactory
{
    public class DebateUser : BaseEFClass
    {

        [Required]
        public string email { get; set; }

        [Required]
        public string display_name { get; set; }

        public Models.DebateUser ToModel()
        {
            return new Models.DebateUser 
            {
                ID = this.id,
                Email = email,
                DisplayName = display_name
            };
        }
    }
}
