import '../NavMenu.css';
import React from 'react';

import TextField from '@mui/material/TextField';
import { Grid } from '@mui/material';
import { AppDetails } from '../../models/AppDetails';
import { WizardButtons } from '../WizardButtons';

export const AppDetailsForm: React.FC<{ detailsDone: Function }> = (props) => {
  const formRef = React.useRef<HTMLFormElement>(null);

  const formSave = () => {
    submitForm();
    let details: AppDetails =
    {
      shortName: "",
      longName: ""
    }
    props.detailsDone(details);
  }

  const submitForm = () => {
    formRef.current?.dispatchEvent(
      new Event("submit", { bubbles: true, cancelable: true })
    );
  };


  return (
    <form noValidate autoComplete="off" ref={formRef}>
      <p>To make any Teams app, we need this information to build the manifest:</p>

      <Grid container>
        <Grid>
          <h3>App names</h3>
          <p>A short name (30 characters or less) is required. Feel free to also include a longer version if your preferred name exceeds 30 characters.</p>
        </Grid>
        <Grid container>
          <TextField id="standard-basic" label="Short name" required size='small' className='field' inputProps={{ maxLength: 30 }} />
          <TextField id="outlined-basic" label="Long name" required size='small' className='longField' />
        </Grid>

        <Grid>
          <h3>Descriptions</h3>
          <p>Include both short and full descriptions of your app. The short description must be under 80 characters and not repeated in the full description.</p>
        </Grid>
        <Grid container>
          <TextField id="standard-basic" label="Short description (80 characters or less)" required size='small' className='longField' />
          <TextField id="outlined-basic" label="Full description (4000 characters or less)" required size='small' fullWidth  />
        </Grid>
        
        <Grid>
          <h3>Developer/company information</h3>
          <p>Enter your developer or company name and website. Make sure the website is a valid https URL.</p>
        </Grid>
        <Grid container>
          <TextField id="standard-basic" label="Developer/Company Name" required size='small' className='field'  />
          <TextField id="outlined-basic" label="Website" required type="url" size='small' className='longField' />
        </Grid>
        
      </Grid>

      <WizardButtons nextClicked={() => formSave()} nextText="Save Info &amp; get app" />

    </form>
  );
};
