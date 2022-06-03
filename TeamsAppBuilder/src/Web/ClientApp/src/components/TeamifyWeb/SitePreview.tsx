import '../NavMenu.css';
import React from 'react';

import Button from '@mui/material/Button';

export const SitePreview: React.FC<{url: string, siteConfirmed: Function, siteCancel: Function}> = (props) => {

  
  return (
    <div>

      <p>How's it look?</p>

      <iframe src={props.url} title="Preview" />

      <Button type="submit" variant="outlined" size="large" onClick={() => props.siteConfirmed()}>Confirm</Button>
      <Button type="submit" variant="outlined" size="large" onClick={() => props.siteCancel()}>Cancel</Button>

    </div>
  );
};
