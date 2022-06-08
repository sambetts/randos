import React from 'react';

import TextField from '@mui/material/TextField';
import { WizardButtons } from '../WizardButtons';
import { Rings } from "react-loader-spinner";
import { Captcha } from 'components/Captcha';

export const SelectSite: React.FC<{ siteSelected: Function }> = (props) => {

  const apiRoot: string | undefined = process.env.REACT_APP_API_BASE;

  const [captchaValue, setCaptchaValue] = React.useState<string | null>();
  const [url, setUrl] = React.useState<string>("");
  const [isLoading, setIsLoading] = React.useState<boolean>(false);

  const onChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const newValue = (e.target as HTMLInputElement).value;
    setUrl(newValue);
  }

  const onCapChange = (value: string | null) => {
    setCaptchaValue(value);
  }

  const startSession = () => {
    let error = false;
    var regexp = new RegExp('^[Hh][Tt][Tt][Pp][Ss]?://');
    if (!regexp.test(url))
      error = true;

    if (!error) {
      setIsLoading(true);
      fetch(apiRoot + "/api/TeamsApp/NewSession?captchaResponseOnPage=" + captchaValue, {
        mode: 'cors',
        method: "POST"
      })
        .then(res => {
          res.text().then(sessionId => {
            if (!res.ok) {
              console.log("Got unexpected response: " + sessionId);
              handleError();
            }
            else {
              console.log("Got session id: " + sessionId);

              setIsLoading(false);
              props.siteSelected(url, sessionId);
            }
          })
        })
        .catch(err => handleError());
    }

  }

  const handleError = () => {
    setIsLoading(false);
    alert('Got an error loading starting a new session. Check JavaScript console for more info.');
  }

  return (
    <div>
      <form autoComplete="off">

        <p>What web page do you want to pin to Teams? We'll create a Teams app with a personal-scope tab with this URL.</p>
        <TextField type="url" value={url} onChange={(e: React.ChangeEvent<HTMLInputElement>) => onChange(e)}
          label="Your Teams app URL" required fullWidth />

        <p style={{fontSize: 12}}>Please note: this should be a website owned by your organisation.</p>
        <p style={{ marginTop: 20 }}>Confirm you are a real person:</p>
        <Captcha onChange={onCapChange} />

        <p style={{marginTop: 50}}>Next we'll take a photo of the page...</p>
        {!isLoading ?
          <WizardButtons nextClicked={() => startSession()} nextText="Teamsify This Website"
            disabled={captchaValue === null || captchaValue === undefined} />
          :
          <Rings ariaLabel="loading-indicator" color='#43488F' />
        }
      </form>
    </div>
  );
};
