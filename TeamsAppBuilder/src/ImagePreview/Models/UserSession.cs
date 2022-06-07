using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class UserSession : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get => RowKeyStaticVal; set { } }
        public static string RowKeyStaticVal => "Sessions";
        public DateTimeOffset? Timestamp { get; set; } = DateTimeOffset.Now;
        public ETag ETag { get; set; }

        public AppDetails AppDetails { get; set; }
    }
}