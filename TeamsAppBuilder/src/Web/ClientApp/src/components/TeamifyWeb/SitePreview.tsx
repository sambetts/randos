import '../NavMenu.css';
import React from 'react';

import { WizardButtons } from '../WizardButtons';

export const SitePreview: React.FC<{url: string, siteConfirmed: Function, siteCancel: Function}> = (props) => {

  
  return (
    <div>

      <p>How's it look?</p>

      <iframe src={props.url} title="Preview" />

      <WizardButtons nextClicked={() => props.siteConfirmed()} nextText="Confirm" 
          previousText="Cancel" previousClicked={() => props.siteCancel()} />
    </div>
  );
};
