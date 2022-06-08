using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class EmailMessage
    {
        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        public bool IsValid => !string.IsNullOrWhiteSpace(From) && !string.IsNullOrWhiteSpace(Message);

        internal SendGridMessage ToSendGridMessage(string to, string from)
        {
            if (string.IsNullOrEmpty(to))
            {
                throw new ArgumentException($"'{nameof(to)}' cannot be null or empty.", nameof(to));
            }
            if (string.IsNullOrEmpty(from))
            {
                throw new ArgumentException($"'{nameof(from)}' cannot be null or empty.", nameof(to));
            }

            var msg = new SendGridMessage()
            {
                From = new EmailAddress(from),
                ReplyTo = new EmailAddress(to),
                Subject = "Teamsify Contact",
                PlainTextContent = $"From: {from}: {Environment.NewLine}{this.Message}"
            };
            msg.AddTo(to);

            return msg;
        }
    }
}
