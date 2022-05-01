using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoCars.Engine
{
    public class CognitiveConfigConfigReader
    {
        public CognitiveConfigConfigReader(string cognitiveEndpoint, string cognitiveKey)
        {
            this.CognitiveEndpoint = cognitiveEndpoint;
            this.CognitiveKey = cognitiveKey;
        }

        /// <summary>
        /// Construct from config
        /// </summary>
        public CognitiveConfigConfigReader()
        {
            this.CognitiveEndpoint = ConfigurationManager.AppSettings.Get("CognitiveEndpoint");
            this.CognitiveKey = ConfigurationManager.AppSettings.Get("CognitiveKey");
        }

        public string CognitiveEndpoint { get; set; }
        public string CognitiveKey { get; set; }

        public bool IsValid
        {
            get
            {
                return !(string.IsNullOrEmpty(this.CognitiveEndpoint) || string.IsNullOrEmpty(this.CognitiveKey));
            }
        }
    }
}
