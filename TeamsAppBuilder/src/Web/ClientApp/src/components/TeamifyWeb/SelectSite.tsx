import '../NavMenu.css';
import React from 'react';

import TextField from '@mui/material/TextField';
import { WizardButtons } from '../WizardButtons';
import ReCAPTCHA from "react-google-recaptcha";

export const SelectSite: React.FC<{ siteSelected: Function }> = (props) => {

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
    if(!regexp.test(url))
      error = true;

      if (!error) {
        setIsLoading(true);
        fetch("https://localhost:44373/api/TeamsApp/NewSession?captchaResponseOnPage=" + captchaValue, {
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

        <p>What web page do you want to pin to Teams?</p>
        <TextField type="url" value={url} onChange={(e: React.ChangeEvent<HTMLInputElement>) => onChange(e)}
          label="Your Teams app URL" required fullWidth />

        <p style={{ marginTop: 20 }}>Please confirm you're a real person:</p>
        <ReCAPTCHA
          sitekey="6LfheE8gAAAAAEMxyPAefAz2CYRdB1kJRKmT9fHM"
          onChange={onCapChange}
        />

        {!isLoading &&
          <WizardButtons nextClicked={() => startSession()} nextText="Teamsify This Website" 
            disabled={captchaValue === null || captchaValue === undefined} />
        }
      </form>
    </div>
  );
};
