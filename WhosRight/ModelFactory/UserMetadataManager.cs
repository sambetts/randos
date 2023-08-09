using Microsoft.Azure.Cosmos.Table;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelFactory
{

    public class UserMetadataManager
    {
        #region Constructors Etc

        private CloudTable table = null;

        private UserMetadataManager(string connectionString)
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();

            table = tableClient.GetTableReference("UserMeta");
            table.CreateIfNotExistsAsync().Wait();
        }

        private static UserMetadataManager _instance = null;
        public static UserMetadataManager CreateInstance(string connectionString)
        {
            if (_instance == null)
            {
                _instance = new UserMetadataManager(connectionString);
            }

            return _instance;
        }

        #endregion

        public async Task<UserMetaTableEntity> GetUserMeta(Models.DebateUser user)
        {
            UserMetaTableEntity meta = null;

            var retrieveOperation = TableOperation.Retrieve<UserMetaTableEntity>(user.Email, user.Email);
            var userMetaQuery = await table.ExecuteAsync(retrieveOperation);

            if (userMetaQuery.Result == null)
            {
                meta = new UserMetaTableEntity(user);

                TableOperation insertOp = TableOperation.Insert(meta);

                await table.ExecuteAsync(insertOp);
            }
            else
            {
                meta = userMetaQuery.Result as UserMetaTableEntity;
            }

            return meta;
        }

        public async Task Update(UserMetaTableEntity userMeta)
        {
            TableOperation updateOp = TableOperation.InsertOrReplace(userMeta);
            await table.ExecuteAsync(updateOp);
        }
    }


    public class UserMetaTableEntity : TableEntity
    {
        public UserMetaTableEntity()
        {
            this.DebatesSeenString = string.Empty;
        }
        public UserMetaTableEntity(Models.DebateUser user) : this()
        {
            this.RowKey = user.Email;
            this.PartitionKey = user.Email;
        }

        public UserMeta ToModel()
        {
            return new UserMeta 
            {
                DebatesSeenString = this.DebatesSeenString
            };
        }

        public string DebatesSeenString { get; set; }

    }
}
