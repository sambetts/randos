using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Entities
{
    /// <summary>
    /// A change in a debate
    /// </summary>
    public class AnswerHistoryDTO
    {
        public AnswerHistoryDTO(AnswerDataOnlyTreeNode fullTree, DebateUser byUser)
        {
            this.AnswerTree = fullTree;
            this.TriggeredBy = byUser;
            this.TriggeredOn = DateTime.Now;
            this.Id = Guid.NewGuid();
        }

        public AnswerDataOnlyTreeNode AnswerTree { get; set; }

        public DebateUser TriggeredBy { get; set; }
        public DateTime TriggeredOn { get; set; }

        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
    }
}
