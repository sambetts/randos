import '../NavMenu.css';
import React from 'react';
import { AppDetails } from '../../models/AppDetails';

import { WizardButtons } from '../WizardButtons';

interface Props
{
  details: AppDetails, 
  url: string,
  startOver : Function
}
export const AppDownload: React.FC<Props> = (props) => {

const downloadApp = () =>
{

}

  return (
    <div>
      <p>You app is ready.</p>

      <WizardButtons nextClicked={() => props.startOver()} nextText="Start Over" 
          previousText="Download App" previousClicked={() => downloadApp()} />
    </div>
  );
};
