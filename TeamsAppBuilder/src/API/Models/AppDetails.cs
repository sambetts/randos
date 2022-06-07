﻿namespace API.Models
{
    public class AppDetails
    {
        public string ShortName { get; set; } = string.Empty;
        public string LongName { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string LongDescription { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyWebsite { get; set; } = string.Empty;

        public string EntityName => ShortName.Replace(" ", string.Empty);

        public bool IsValid => !string.IsNullOrEmpty(ShortName) && !string.IsNullOrEmpty(LongName)
            && !string.IsNullOrEmpty(ShortDescription) && !string.IsNullOrEmpty(LongDescription)
            && !string.IsNullOrEmpty(CompanyName) && !string.IsNullOrEmpty(CompanyWebsite);


        /// <summary>
        /// Build a simple Teams manifest from our wizard collected info.
        /// </summary>
        public TeamsAppManifest ToTeamsAppManifest(string appSiteUrl)
        {
            var manifest = new TeamsAppManifest()
            {
                Name = new TeamsAppManifest.AppName { Short = ShortName, Full = LongName },
                Description = new TeamsAppManifest.AppDescription { Short = ShortName, Full = LongName },
                Developer = new TeamsAppManifest.AppDeveloper { Name = CompanyName, PrivacyUrl = CompanyWebsite, TermsOfUseUrl = CompanyWebsite, WebsiteUrl = CompanyWebsite },
            };
            manifest.StaticTabs.Add(new TeamsAppManifest.AppStaticTab
            {
                ContentUrl = appSiteUrl,
                EntityId = EntityName,
                Name = EntityName,
                WebsiteUrl = appSiteUrl
            });

            return manifest;
        }
    }

}