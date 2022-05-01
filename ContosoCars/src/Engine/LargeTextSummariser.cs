using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoCars.Engine
{
    /// <summary>
    /// Chops large text into smaller chunks
    /// </summary>
    public class LargeTextSummariser
    {
        public LargeTextSummariser(string fullText)
        {
            this.FullText = fullText;
        }
        public string FullText { get; set; }

        public List<string> GetSegments()
        {
            var segments = new List<string>();

            foreach (var line in FullText.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {

                char[] delimiters = new char[] { ' ', '\r', '\n' };
                int words = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;

                if (words > 3)
                {
                    segments.Add(line);
                }
            }

            return segments;
        }

        /// <summary>
        /// Use Azure cognitive to get key-phrases
        /// </summary>
        public async Task<List<string>> GetTopKeyPhrasesAsync(TextAnalyticsClient cognitiveClient)
        {
            var segments = GetSegments();

            var phrases = new Dictionary<string, int>();

            MultiLanguageBatchInput batchInput = new MultiLanguageBatchInput() { Documents = new List<MultiLanguageInput>() };

            int segId = 0;
            foreach (var seg in segments)
            {
                batchInput.Documents.Add(new MultiLanguageInput()
                {
                    Id = segId.ToString(),
                    Text = seg
                });
                segId++;
            }

            var kpResult = await cognitiveClient.KeyPhrasesBatchAsync(batchInput);


            foreach (var kpDoc in kpResult.Documents)
            {
                foreach (var kp in kpDoc.KeyPhrases)
                {
                    if (!phrases.ContainsKey(kp))
                    {
                        phrases.Add(kp, 1);
                    }
                    else
                    {
                        phrases[kp]++;
                    }
                }
            }

            // Find the most popular keywords. Filter for increasingly higher keyword repetitions until we find none; then use the last set found
            List<string> lastTopPhrases = phrases.OrderBy(p => p.Value).Select(p => p.Key)
                .Where(kps => !_ignored.Contains(kps))
                .Take(20)
                .ToList();

            return lastTopPhrases;
        }

        static List<string> _ignored = new List<string>()
        { 
            "COVID", "Home delivery COVID", "Live Home COVID",
            "SO23", "0QF", "Live video COVID", "BEST-SELLING CARS", "FLEXIBILITY",
            "website", "Seller's number", "safety measures", "Total", "Address", "TERMS"
        };
    }
}
