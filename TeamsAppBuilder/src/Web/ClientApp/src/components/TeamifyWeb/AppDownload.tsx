import React from 'react';
import { AppDetails } from '../models/WizardModels';

import { WizardButtons } from '../WizardButtons';
import { Button } from '@mui/material';

interface Props {
  sessionId: string,
  details: AppDetails,
  url: string,
  startOver: Function,
  goBack: Function
}
export const AppDownload: React.FC<Props> = (props) => {
  const [downloadRedirectUrl, setDownloadRedirectUrl] = React.useState<string | null>();
  const apiRoot: string | undefined = process.env.REACT_APP_API_BASE;

  const downloadApp = () => {
    console.log(JSON.stringify(props.details));

    fetch(apiRoot + "/api/TeamsApp/CreateApp?url=" + props.url + "&sessionId=" + props.sessionId,
      {
        mode: 'cors',
        method: "POST",
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(props.details)
      })
      .then(res => {
        res.text().then(body => {
          if (!res.ok) {
            handleError();
          }
          else {
            // Response is a URL back to our service which will generate a redirect to blob storage if GET-ed
            const downloadUrl: string = `${apiRoot}/api/TeamsApp/DownloadApp?fileUrl=${body}&sessionId=${props.sessionId}`;
            setDownloadRedirectUrl(downloadUrl);
          }

        })
      })
      .catch(err => handleError());
  }

  const handleError = () => {
    alert('Got an error loading the package. Check JavaScript console for more info.');
  }

  return (
    <div>
      <h3>Excellent!</h3>
      <p>You app '{props.details.shortName}' is ready to download and deploy to your Teams.</p>

      <Button type="submit" variant="contained" size="large" onClick={() => downloadApp()}
        style={{ marginTop: 50, marginBottom: 50 }}>Download Teams App Zip</Button>

      <p>You just need to deploy it to Teams.&nbsp;
        <a href='https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/deploy-and-publish/apps-publish-overview' rel="noreferrer" target='_blank'>It's super-easy.</a></p>
      <p>You can side-load the app, or if you have the right permissions, deploy to entire group(s) of users.</p>
      {downloadRedirectUrl &&
        <>
          <iframe src={downloadRedirectUrl} className='Hidden' title='Redirect' />
        </>
      }

      <WizardButtons nextClicked={() => props.startOver()} nextText="Start Over"
        previousText="Back" previousClicked={() => props.goBack()} />
    </div>
  );
};
