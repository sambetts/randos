import '../NavMenu.css';
import React from 'react';
import { AppDetails } from '../models/WizardModels';

import { WizardButtons } from '../WizardButtons';
import { Button } from '@mui/material';

interface Props
{
  details: AppDetails, 
  url: string,
  startOver : Function,
  goBack : Function
}
export const AppDownload: React.FC<Props> = (props) => {

const downloadApp = () =>
{

}

  return (
    <div>
      <h3>Excellent!</h3>
      <p>You app is ready.</p>
      
      <Button type="submit" variant="outlined" size="large" onClick={() => downloadApp()}>Download</Button>

      <p>You just need to deploy it to Teams. It's super-easy:</p>
      
      <WizardButtons nextClicked={() => props.startOver()} nextText="Start Over" 
          previousText="Back" previousClicked={() => props.goBack()} />
    </div>
  );
};
