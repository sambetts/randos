import '../NavMenu.css';
import React from 'react';

import TextField from '@mui/material/TextField';
import { Grid } from '@mui/material';
import { AppDetails } from '../../models/AppDetails';
import { WizardButtons } from '../WizardButtons';

export const AppDetailsForm: React.FC<{ detailsDone: Function }> = (props) => {

  const formSave = () => {
    let details: AppDetails =
    {
      shortName: "",
      longName: ""
    }
    props.detailsDone(details);
  }

  return (
    <form noValidate autoComplete="off">
      <p>More info about you.</p>
      <Grid container>
        <Grid container>

          <TextField id="standard-basic" label="Short name" required />
          <TextField id="outlined-basic" label="Long name" required />

        </Grid>
        <Grid container>
          <TextField id="outlined-basic" label="Outlined" />
        </Grid>
      </Grid>

      <WizardButtons nextClicked={() => formSave()} nextText="Save" />

    </form>
  );
};
