using API;
using API.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class AppDetailsTests
    {
        const string URL = "https://contoso.com";

        [TestMethod]
        public void AppDetailsIsValid()
        {
            Assert.IsFalse(new AppDetails().IsValid);
            Assert.IsTrue(GetAppDetails().IsValid);
        }

        [TestMethod]
        public void ToTeamsAppManifestTests()
        {
            var manifest = GetAppDetails().ToTeamsAppManifest(URL);
            Assert.IsTrue(manifest.PackageName.Length > 0 && manifest.PackageName.IndexOf(" ") == -1);
            Assert.IsTrue(manifest.StaticTabs.Count == 1);
        }

        [TestMethod]
        public async Task UserSessionTests()
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Storage"]?.ConnectionString;
            var client = new UserSessionTableClient(connectionString);
            var newSession = await UserSession.AddNewSessionToAzTable(client);

            Assert.IsNotNull(newSession.RowKey);

            newSession.SavedManifestUrl = URL;

            await newSession.UpdateTableRecord(client);
            var newSession2 = await UserSession.GetSessionFromAzTable(newSession.RowKey, client);

            // Make sure we get null for a random ID
            var nullSession = await UserSession.GetSessionFromAzTable(Guid.NewGuid().ToString(), client);
            Assert.IsNull(nullSession);

            Assert.IsTrue(newSession2.SavedManifestUrl == URL);
        }

        [TestMethod]
        public void GenerateZipBytes()
        {
            var a = GetAppDetails();
            var bytes = a.ToTeamsAppManifest(URL).BuildZip("bob.zip");

            Assert.IsNotNull(bytes);
        }

        AppDetails GetAppDetails()
        {
            return new AppDetails()
            {
                CompanyName = "Testing",
                CompanyWebsite = URL,
                LongDescription = "Testing long",
                ShortDescription = "Test",
                ShortName = "Test",
                LongName = "Test"
            };
        }
    }
}
