export as namespace Ass;

export const enum Stage {
  SiteSelection,
  VerifySite,
  EnterData,
  Download
}

export interface AppDetails {
  shortName: string;
  longName: string;
}
