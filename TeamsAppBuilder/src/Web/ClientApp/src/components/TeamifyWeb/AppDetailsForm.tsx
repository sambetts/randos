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
      <p>More info about you.</p>
      <Grid container>
        <Grid container>

          <TextField id="standard-basic" label="Short name" required />

        </Grid>
        <Grid container>
          <TextField id="outlined-basic" label="Long name" required />
        </Grid>
      </Grid>

      <WizardButtons nextClicked={() => formSave()} nextText="Save" />

    </form>
  );
};
