import '../NavMenu.css';
import React from 'react';
import { AppDetails } from '../models/WizardModels';

import { WizardButtons } from '../WizardButtons';
import { Button } from '@mui/material';

interface Props
{
  sessionId: string,
  details: AppDetails, 
  url: string,
  startOver : Function,
  goBack : Function
}
export const AppDownload: React.FC<Props> = (props) => {

const downloadApp = () =>
{
  console.log(JSON.stringify(props.details));
  
  fetch("https://localhost:44373/api/GetTeamsApp?url=" + props.url, 
  { 
    mode: 'cors', 
    method: "POST" ,
    headers: {
      'Content-Type': 'application/json'
      // 'Content-Type': 'application/x-www-form-urlencoded',
    },
    body: JSON.stringify(props.details)
  })
  .then(res => {
    res.text().then(body => {
      if (!res.ok) {
        handleError();
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
      
      <Button type="submit" variant="outlined" size="large" onClick={() => downloadApp()} style={{marginTop: 50, marginBottom: 50}}>Download App</Button>

      <p>You just need to deploy it to Teams. It's super-easy:</p>
      
      <WizardButtons nextClicked={() => props.startOver()} nextText="Start Over" 
          previousText="Back" previousClicked={() => props.goBack()} />
    </div>
  );
};
