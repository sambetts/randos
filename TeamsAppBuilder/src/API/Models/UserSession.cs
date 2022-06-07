using Azure;
using Azure.Data.Tables;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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

        public string AppDetailsJson { get; set; } = null;

        [IgnoreDataMember]
        public AppDetails AppDetails
        {
            get
            {
                if (string.IsNullOrEmpty(AppDetailsJson))
                {
                    return new AppDetails();
                }
                try
                {
                    return JsonConvert.DeserializeObject<AppDetails>(AppDetailsJson);
                }
                catch (JsonException)
                {
                    return new AppDetails();
                }
            }
            set 
            {
                AppDetailsJson = JsonConvert.SerializeObject(value);
            }
        }

        public string SavedManifestUrl { get; set; } = string.Empty;
    }
}