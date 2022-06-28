declare interface IProtectedApiSpfxWebPartStrings {
  PropertyPaneDescription: string;
  BasicGroupName: string;
  ApiEndpointFieldLabel: string;
  AppLocalEnvironmentSharePoint: string;
  AppLocalEnvironmentTeams: string;
  AppSharePointEnvironment: string;
  AppTeamsTabEnvironment: string;
}

declare module 'ProtectedApiSpfxWebPartStrings' {
  const strings: IProtectedApiSpfxWebPartStrings;
  export = strings;
}
