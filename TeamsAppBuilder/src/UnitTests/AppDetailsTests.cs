using API.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class AppDetailsTests
    {
        [TestMethod]
        public void AppDetailsIsValid()
        {
            Assert.IsFalse(new AppDetails().IsValid);
            Assert.IsTrue(GetAppDetails().IsValid);
        }

        [TestMethod]
        public void ToTeamsAppManifestTests()
        {
            var manifest = GetAppDetails().ToTeamsAppManifest("https://contoso.com");
            Assert.IsTrue(manifest.StaticTabs.Count == 1);
        }

        [TestMethod]
        public void GenerateZipBytes()
        {
            var a = GetAppDetails();
            var bytes = a.ToTeamsAppManifest("https://contoso.com").BuildZip("bob.zip");

            Assert.IsNotNull(bytes);
        }

        AppDetails GetAppDetails()
        {
            return new AppDetails()
            {
                CompanyName = "Testing",
                CompanyWebsite = "https://contoso.com",
                LongDescription = "Testing long",
                ShortDescription = "Test",
                ShortName = "Test",
                LongName = "Test"
            };
        }
    }
}
