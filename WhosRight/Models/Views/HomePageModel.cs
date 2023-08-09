using Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Views
{
    public class HomePageModel
    {
        /// <summary>
        /// List of debates (root-level answers)
        /// </summary>
        public List<DebateDTO> Debates { get; set; }

        public UserMeta UserMeta { get; set; }
    }
}
