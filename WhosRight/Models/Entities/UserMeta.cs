using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Entities
{
    public class UserMeta
    {

        public string DebatesSeenString { get; set; }

        public void AddDebate(Answer debate)
        {
            if (this.SeenDebates.Contains(debate.ID))
            {
                return;
            }
            else
            {
                this.DebatesSeenString += "-" + debate.ID;
            }
        }

        public List<int> SeenDebates
        {
            get
            {
                List<int> ids = new List<int>();
                foreach (var idString in this.DebatesSeenString.Split("-".ToCharArray()))
                {
                    if (!string.IsNullOrEmpty(idString))
                    {
                        ids.Add(int.Parse(idString));
                    }
                }
                return ids;
            }
        }

    }
}
