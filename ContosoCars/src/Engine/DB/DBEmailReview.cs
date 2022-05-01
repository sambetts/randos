using DocumentFormat.OpenXml.Packaging;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoCars.Engine.DB
{
    public class DBEmailReview : BaseDB
    {
        public DBEmailReview()
        { 
        }

        public async static Task<DBEmailReview> BuildFrom(byte[] fileData, string sender, string fileName, CognitiveConfigConfigReader cognitiveConfig)
        {
            ApiKeyServiceClientCredentials credentials = new ApiKeyServiceClientCredentials(cognitiveConfig.CognitiveKey);
            TextAnalyticsClient cognitiveClient = new TextAnalyticsClient(credentials) { Endpoint = cognitiveConfig.CognitiveEndpoint };

            var newDoc = new DBEmailReview()
            {
                Content = fileData,
                Filename = fileName,
                EmailAddress = sender,
                Received = DateTime.Now
            };


            await newDoc.PopulateKeywords(cognitiveClient);

            return newDoc;
        }

        public async Task Save(DocParserDB db)
        {
            if (this.ID == 0)
            {
                db.Reviews.Add(this);
            }

            await db.SaveChangesAsync();
        }

        public async Task PopulateKeywords(TextAnalyticsClient cognitiveClient)
        {
            string docText = string.Empty;
            if (this.Filename.EndsWith(".docx"))
            {
                docText = TextFromWord();
            }
            else if(this.Filename.EndsWith(".pdf"))
            {
                docText = TextFromPDF();
            }
            else
            {
                throw new NotSupportedException("Unexpected file type");
            }

            LargeTextSummariser textSummariser = new LargeTextSummariser(docText);

            string kws = string.Empty;
            foreach (var kw in await textSummariser.GetTopKeyPhrasesAsync(cognitiveClient))
            {
                kws += kw + ", ";
            }

            this.KeyWords = kws.TrimEnd(", ".ToCharArray());
        }

        private string TextFromPDF()
        {


            PdfReader reader = new PdfReader(this.Content);
            string allPagesText = string.Empty;
            
            for (int page = 1; page < reader.NumberOfPages + 1; page++) //(int page = 1; page <= reader.NumberOfPages; page++) <- for scanning all the pages in A PDF
            {
                var its = new iTextSharp.text.pdf.parser.LocationTextExtractionStrategy();
                var pageText = PdfTextExtractor.GetTextFromPage(reader, page, its);

                pageText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(pageText)));

                allPagesText += pageText + "\n";
                //creating the string array and storing the PDF line by line
            }

            reader.Close();

            return allPagesText;
        }

        string TextFromWord()
        {
            const string wordmlNamespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            StringBuilder textBuilder = new StringBuilder();
            using (var memoryStream = new MemoryStream(this.Content))
            {
                //byte[] fileBytes = System.IO.File.ReadAllBytes(file);
                using (WordprocessingDocument wdDoc = WordprocessingDocument.Open(memoryStream, false))
                {
                    // Manage namespaces to perform XPath queries.  
                    var nt = new System.Xml.NameTable();
                    var nsManager = new System.Xml.XmlNamespaceManager(nt);
                    nsManager.AddNamespace("w", wordmlNamespace);

                    // Get the document part from the package.  
                    // Load the XML in the document part into an XmlDocument instance.  
                    var xdoc = new System.Xml.XmlDocument(nt);
                    xdoc.Load(wdDoc.MainDocumentPart.GetStream());

                    var paragraphNodes = xdoc.SelectNodes("//w:p", nsManager);
                    foreach (System.Xml.XmlNode paragraphNode in paragraphNodes)
                    {
                        var textNodes = paragraphNode.SelectNodes(".//w:t", nsManager);
                        foreach (System.Xml.XmlNode textNode in textNodes)
                        {
                            textBuilder.Append(textNode.InnerText);
                        }
                        textBuilder.Append(Environment.NewLine);
                    }

                }
            }


            return textBuilder.ToString();
        }


        public string EmailAddress { get; set; }

        public string KeyWords { get; set; }
        public bool? Accepted { get; set; }
        public string Filename { get; set; }
        public DateTime Received { get; set; }

        public byte[] Content { get; set; }
    }

}
