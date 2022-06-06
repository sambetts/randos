import React from 'react';

import Button from '@mui/material/Button';

interface Props {
  previousClicked?: Function,
  nextClicked?: Function,
  nextText?: string,
  previousText?: string
}
export const WizardButtons: React.FC<Props> = (props) => {

  const nextClick = () => 
  {
    if (props.nextClicked) {
      props.nextClicked();
    }
  }

  const previousClick = () => 
  {
    if (props.previousClicked) {
      props.previousClicked();
    }
  }

  return (
    <div>
      {props.previousText &&
        <Button type="submit" variant="outlined" size="large" onClick={() => previousClick()}>{props.previousText}</Button>

      }
      {props.nextText &&
        <Button type="submit" variant="outlined" size="large" onClick={() => nextClick()}>{props.nextText}</Button>

      }
    </div>
  );
};
