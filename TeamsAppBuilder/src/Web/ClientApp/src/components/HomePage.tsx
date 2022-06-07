import './NavMenu.css';
import React from 'react';
import { SelectSite } from './TeamifyWeb/SelectSite';
import { SitePreview } from './TeamifyWeb/SitePreview';
import { AppDetailsForm } from './TeamifyWeb/AppDetailsForm';
import { AppDownload } from './TeamifyWeb/AppDownload';
import { AppDetails } from './models/WizardModels';
import { Stage } from './models/enums';
import { Button, Grid } from '@mui/material';
import BuildIcon from '@mui/icons-material/Build';
import DownloadIcon from '@mui/icons-material/Download';
import SettingsIcon from '@mui/icons-material/Settings';


export const HomePage: React.FC<{ wizardStageChange: Function }> = (props) => {

  const [currentStage, setCurrentStage] = React.useState<Stage>(Stage.Home);
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

  const setStage = (stage: Stage) => {
    setCurrentStage(stage);
    props.wizardStageChange(stage);
  }

  const renderSwitch = (stage: Stage) => {
    switch (stage) {
      case Stage.Home:
        return (
          <div>
            <p>Add &amp; pin your website to Teams.</p>
            <img src='imgs/example.png' alt='Example Teamsified Website' />
            <p>Get your website added to Teams in 3 easy steps. 5 minutes tops.</p>

            <Grid container alignItems="center" justifyContent="center">

              <table className='processInfoGraphic'>
                <tr>
                  <td>Build</td>
                  <td>Download</td>
                  <td>Deploy</td>
                </tr>
                <tr>
                  <td><BuildIcon fontSize='large' /></td>
                  <td><DownloadIcon fontSize='large' /></td>
                  <td><SettingsIcon fontSize='large' /></td>
                </tr>
              </table>
            </Grid>
            <Grid container justifyContent="center" style={{marginTop: 20}}>
              <Button variant="contained" size="large" onClick={() => setStage(Stage.SiteSelection)}>Start Wizard</Button>
            </Grid>

          </div>);

      case Stage.SiteSelection:
        return <SelectSite siteSelected={(url: string) => siteSelect(url)} />;
      case Stage.VerifySite:
        return <SitePreview url={url} siteConfirmed={() => setStage(Stage.EnterData)}
          siteCancel={() => setStage(Stage.Home)} />
      case Stage.EnterData:
        return <AppDetailsForm detailsDone={(details: AppDetails) => appDetailsSet(details)} cancel={() => setStage(Stage.Home)} />
      case Stage.Download:
        return <AppDownload details={appDetails!} url={url}
          startOver={() => setStage(Stage.Home)}
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
