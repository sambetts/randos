import '../NavMenu.css';
import React from 'react';

import TextField from '@mui/material/TextField';
import { WizardButtons } from '../WizardButtons';

export const SelectSite: React.FC<{ siteSelected: Function }> = (props) => {

  const [url, setUrl] = React.useState<string>("https://www.bbc.com/news");
  
  const onChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const newValue = (e.target as HTMLInputElement).value;
    setUrl(newValue);
  }

  return (
    <div>

      <p>This application is for adding your website as an app in Teams.</p>
      <TextField type="url" value={url} onChange={(e: React.ChangeEvent<HTMLInputElement>) => onChange(e)} 
      label="Add URL" required fullWidth />

      <WizardButtons nextClicked={() => props.siteSelected(url)} nextText="Teamsify This Website" />
    </div>
  );
};
