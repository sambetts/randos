import '../NavMenu.css';
import React from 'react';

import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';

export const SelectSite: React.FC<{ siteSelected: Function }> = (props) => {

  const [url, setUrl] = React.useState<string>("https://www.bbc.com/news");
  
  const onChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const newValue = (e.target as HTMLInputElement).value;
    setUrl(newValue);
  }

  return (
    <div>

      <p>This application is for adding your website as an app in Teams.</p>
      <TextField type="url" size="small" value={url} onChange={(e: React.ChangeEvent<HTMLInputElement>) => onChange(e)} label="Add URL" required />
      <Button type="submit" variant="outlined" size="large" onClick={() => props.siteSelected(url)}>Next</Button>

    </div>
  );
};
