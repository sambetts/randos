using Microsoft.Azure.ServiceBus;
using ModelFactory.Config;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api
{
    public class MessageQueueManager
    {
        public MessageQueueManager(ServerSideKeyVaultSettings settings)
        {
            Settings = settings;
        }

        public ServerSideKeyVaultSettings Settings { get; set; }

        public async Task SendEmailToQueue(AnswerReplyEmailAlertLogDTO m)
        {
            var queueClient = GetClient(QueueType.Email);

            var queueMessage = new Message();
            string emailJson = Newtonsoft.Json.JsonConvert.SerializeObject(m);
            queueMessage.Body = Encoding.UTF8.GetBytes(emailJson);

            await queueClient.SendAsync(queueMessage);
        }

        public async Task SendNewAnswerToQueue(AnswerDataOnlyTreeNode answer)
        {
            var queueClient = GetClient(QueueType.Answer);

            Message queueMessage = new Message();

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(answer);
            queueMessage.Body = Encoding.UTF8.GetBytes(json);

            await queueClient.SendAsync(queueMessage);
        }

        QueueClient GetClient(QueueType type)
        {
            string conString = string.Empty;
            if (type == QueueType.Answer)
            {
                conString = Settings.ConnectionStrings.ServiceBusNewAnswersConnectionString;
            }
            else if (type == QueueType.Email)
            {
                conString = Settings.ConnectionStrings.ServiceBusEmailConnectionString;
            }
            else
            {
                throw new ArgumentOutOfRangeException("type");
            }
            ServiceBusConnectionStringBuilder connectionStringBuilder = new ServiceBusConnectionStringBuilder(conString);
            return new QueueClient(connectionStringBuilder);
        }

        enum QueueType
        {
            Unknown,
            Email,
            Answer
        }
    }
}
