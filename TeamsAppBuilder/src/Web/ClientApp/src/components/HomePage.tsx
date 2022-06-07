import './NavMenu.css';
import React from 'react';
import { SelectSite } from './TeamifyWeb/SelectSite';
import { SitePreview } from './TeamifyWeb/SitePreview';
import { AppDetailsForm } from './TeamifyWeb/AppDetailsForm';
import { AppDownload } from './TeamifyWeb/AppDownload';
import { AppDetails, Stage } from './models/WizardModels';


export const HomePage: React.FC<{wizardStageChange: Function}> = (props) => {

  const [currentStage, setCurrentStage] = React.useState<Stage>(Stage.SiteSelection);
  const [url, setUrl] = React.useState<string>("");
  const [appDetails, setAppDetails] = React.useState<AppDetails | null>(null);


  const siteSelect = (url: string) => {
    setUrl(url);
    setStage(Stage.VerifySite);
  }

  const appDetailsSet = (details: AppDetails) => {
    setAppDetails(details);
    setStage(Stage.Download);
  }

  const setStage = (stage: Stage) => 
  {
    setCurrentStage(stage);
    props.wizardStageChange(stage);
  }

  const renderSwitch = (stage: Stage) => {
    switch (stage) {
      case Stage.SiteSelection:
        return <SelectSite siteSelected={(url: string) => siteSelect(url)} />;
      case Stage.VerifySite:
        return <SitePreview url={url} siteConfirmed={() => setStage(Stage.EnterData)} 
          siteCancel={() => setStage(Stage.SiteSelection)} />
      case Stage.EnterData:
        return <AppDetailsForm detailsDone={(details: AppDetails) => appDetailsSet(details)} />
      case Stage.Download:
        return <AppDownload details={appDetails!} url={url} 
          startOver={()=> setStage(Stage.SiteSelection)} 
          goBack={() => setStage(Stage.EnterData)} />
      default:
        return <p>No idea what to display</p>;
    }
  }

  return (
    <div>
      {renderSwitch(currentStage)}
    </div>
  );
};
