import '../NavMenu.css';
import React from 'react';
import { AppDetails } from '../../models/AppDetails';

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
      <p>You app is ready.</p>
      
      <Button type="submit" variant="outlined" size="large" onClick={() => downloadApp()}>Download</Button>

      <WizardButtons nextClicked={() => props.startOver()} nextText="Start Over" 
          previousText="Back" previousClicked={() => props.goBack()} />
    </div>
  );
};
