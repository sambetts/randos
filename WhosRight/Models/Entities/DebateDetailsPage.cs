using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Entities
{

    public class DebateDetailsPage
    {
        [Required]
        public DebateDTO Debate { get; set; }

        /// <summary>
        /// Types of response a user can add: "agree", "disagree" etc.
        /// </summary>
        [Required]
        public List<ResponseType> AllResponseTypes { get; set; }

        /// <summary>
        /// A stupid way of introducing a debate proposition
        /// </summary>
        public string RandomIntro
        {
            get
            {
                string[] intros = {
                    "Yea, ",
                    "Behold, ",
                    "Hark, ",
                    "And thus it was that ",
                    "Hark! ",
                    "Indeed, ",
                    "Lo! ",
                    "I proclaim! ",
                    "Hark thee! ",
                    "And so it was...",
                    "Rejoice I say!",
                    "Yea, brothers &amp; sisters..."
                };
                System.Random rnd = new System.Random();
                int idx = rnd.Next(intros.Length - 1);

                return intros[idx];
            }
        }

        public List<DebateHistory> Changes { get; set; }
    }
}
