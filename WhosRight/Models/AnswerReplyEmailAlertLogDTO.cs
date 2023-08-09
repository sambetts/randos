using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    /// <summary>
    /// Email about a reply to a debate
    /// </summary>
    public class AnswerReplyEmailAlertLogDTO
    {
        public AnswerReplyEmailAlertLogDTO()
        {
            this.Initiator = new EmailRecipientDTO();
            this.ToUser = new EmailRecipientDTO();
        }
        public AnswerReplyEmailAlertLogDTO(DebateUser toUser, Answer triggerAnswer) : this()
        {
            this.ToUser = new EmailRecipientDTO(toUser);
            this.Initiator = new EmailRecipientDTO(triggerAnswer.ByUser);
            this.RelatedAnswer = new AnswerDataOnlyTreeNode(triggerAnswer);
        }

        public EmailRecipientDTO Initiator { get; set; }
        public EmailRecipientDTO ToUser { get; set; }
        public AnswerDataOnlyTreeNode RelatedAnswer { get; set; }

        public string Subject { get; set; }
        public string Body { get; set; }

        public DateTime SentOn { get; set; }
    }

    public class EmailRecipientDTO
    {
        public EmailRecipientDTO()
        {
        }

        public EmailRecipientDTO(DebateUser fromUser) : this()
        {
            this.Email = fromUser.Email;
            this.UserID = fromUser.ID;
        }

        public string Email { get; set; }
        public int UserID { get; set; }
    }


    /// <summary>
    /// For new answer replies
    /// </summary>
    public class NewAnswerDTO
    {
        public string Title { get; set; }

        public string Body { get; set; }

        public int ReactionIDToParent { get; set; }

        public int ParentAnswerID { get; set; }

    }
}
