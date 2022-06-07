import '../NavMenu.css';
import React from 'react';

import TextField from '@mui/material/TextField';
import { WizardButtons } from '../WizardButtons';
import ReCAPTCHA from "react-google-recaptcha";

export const SelectSite: React.FC<{ siteSelected: Function }> = (props) => {

  const [captchaValue, setCaptchaValue] = React.useState<string | null>();
  const [url, setUrl] = React.useState<string>("https://www.bbc.com/news");

  const onChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const newValue = (e.target as HTMLInputElement).value;
    setUrl(newValue);
  }

  const onCapChange = (value: string | null) => {
    setCaptchaValue(value);
    console.log("Captcha value:", value);
  }

  const startSession = () => {
    fetch("https://localhost:44373/api/TeamsApp/NewSession?captchaResponseOnPage=" + captchaValue, {
      mode: 'cors',
      method: "POST"
    })
      .then(res => {
        res.text().then(sessionId => {
          if (!res.ok) {
            console.log(sessionId);
            handleError();
          }
          else
            props.siteSelected(url, sessionId);
        })
      })
      .catch(err => handleError());
  }

  const handleError = () => {
    alert('Got an error loading starting a new session. Check JavaScript console for more info.');
  }

  return (
    <div>

      <p>What website do you want to pin to Teams?</p>
      <TextField type="url" value={url} onChange={(e: React.ChangeEvent<HTMLInputElement>) => onChange(e)}
        label="Your Teams app Website URL" required fullWidth />

      <p style={{marginTop: 20}}>Please confirm you're a real person:</p>
      <ReCAPTCHA
        sitekey="6LfheE8gAAAAAEMxyPAefAz2CYRdB1kJRKmT9fHM"
        onChange={onCapChange}
      />

      <WizardButtons nextClicked={() => startSession()} nextText="Teamsify This Website" disabled={captchaValue === null || captchaValue === undefined} />
    </div>
  );
};
