using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Entities
{
    public class PlayerRanking
    {
        public PlayerRanking()
        { 
        }
        public PlayerRanking(DebateUser user, int score)
        {
            this.User = user;
            this.Score = score;
        }

        public DebateUser User { get; set; }
        public int Score { get; set; }
    }

}
