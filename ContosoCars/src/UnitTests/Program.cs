
using ContosoCars.Engine;
using ContosoCars.Engine.DB;
using ContosoCarsML.Model;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace UnitTests
{
    class Program
    {
        private static void Main()
        {
            Console.WriteLine("Start...");

            EvalTestFiles();

            //SaveExampleFilesToDB();

            Console.WriteLine("\nDone.");
            Console.ReadKey();
        }

        private static void SaveExampleFilesToDB()
        {
            using (var db = new DocParserDB())
            {
                string[] pdfs = Directory.GetFiles(@"C:\Users\sambetts\Downloads\Cars", "*.pdf", SearchOption.AllDirectories);

                foreach (var pdf in pdfs)
                {
                    SaveFile(pdf, db).Wait();
                }

                string[] docs = Directory.GetFiles(@"C:\Users\sambetts\Downloads\Cars\", "*.docx", SearchOption.AllDirectories);

                foreach (var doc in docs)
                {
                    SaveFile(doc, db).Wait();
                }

            }
        }

        private static void EvalTestFiles()
        {
            string[] testData = Directory.GetFiles(@"C:\Users\sambetts\Desktop\ContosoCars\Data\Test Data", "*.pdf", SearchOption.AllDirectories);
            foreach (var testDataFile in testData)
            {
                var r = GetReview(testDataFile).Result;
                var testFileInfo = new FileInfo(testDataFile);


                // Add input data
                var input = new ModelInput() { Filename = testFileInfo.Name, Keywords = r.KeyWords };

                // Load model and predict output of sample data
                ModelOutput result = ConsumeModel.Predict(input);


                Console.WriteLine($"{testFileInfo.Name}: {result.Prediction} ({result.ScoreString})");
            }
        }

        static async Task<DBEmailReview> GetReview(string file)
        {
            FileInfo fileInfo = new FileInfo(file);
            try
            {
                var review = await DBEmailReview.BuildFrom(File.ReadAllBytes(fileInfo.FullName), "jim@bob.com", fileInfo.Name, new CognitiveConfigConfigReader());
                return review;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Got exception {ex.Message} importing {fileInfo.Name}");
                return null;
            }
        }

        private static async Task SaveFile(string file, DocParserDB db)
        {
            FileInfo fileInfo = new FileInfo(file);
            if (!db.Reviews.Where(r => r.Filename == fileInfo.Name).Any())
            {
                DBEmailReview review = await GetReview(file);
                if (review != null)
                {
                    db.Reviews.Add(review);
                    await db.SaveChangesAsync();
                    Console.WriteLine($"Added {fileInfo.Name}");
                }
            }
            else
            {
                Console.WriteLine($"Already have {fileInfo.Name}");
            }
        }
    }
}
