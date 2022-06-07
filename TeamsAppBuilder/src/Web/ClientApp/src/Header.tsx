import * as React from 'react';
import AppBar from '@mui/material/AppBar';
import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid';
import HelpIcon from '@mui/icons-material/Help';
import IconButton from '@mui/material/IconButton';
import Toolbar from '@mui/material/Toolbar';
import Tooltip from '@mui/material/Tooltip';
import Typography from '@mui/material/Typography';
import { Stage } from 'components/models/WizardModels';

const lightColor = 'rgba(255, 255, 255, 0.7)';

interface HeaderProps {
  onDrawerToggle: () => void;
  stage: Stage;
}

export default function Header(props: HeaderProps) {

  
  const renderSwitch = (stage: Stage) => {
    switch (stage) {
      case Stage.SiteSelection:
        return <span>Webify</span>;
      case Stage.VerifySite:
        return <span>How does this look?</span>
      case Stage.EnterData:
        return <span>Enter data</span>
      case Stage.Download:
        return <span>Download your App</span>
      default:
        return <p>No idea what to display</p>;
    }
  }

  return (
    <React.Fragment>
      
      <AppBar
        component="div"
        color="primary"
        position="static"
        elevation={0}
        sx={{ zIndex: 0 }}
      >
        <Toolbar>
          <Grid container alignItems="center" spacing={1}>
            <Grid item xs>
              <Typography color="inherit" variant="h5" component="h1">
                {renderSwitch(props.stage)}
              </Typography>
            </Grid>
            <Grid item>
              <Button
                sx={{ borderColor: lightColor }}
                variant="outlined"
                color="inherit"
                size="small"
              >
                Teamsify you Website
              </Button>
            </Grid>
            <Grid item>
              <Tooltip title="Help">
                <IconButton color="inherit">
                  <HelpIcon />
                </IconButton>
              </Tooltip>
            </Grid>
          </Grid>
        </Toolbar>
      </AppBar>
    </React.Fragment>
  );
}