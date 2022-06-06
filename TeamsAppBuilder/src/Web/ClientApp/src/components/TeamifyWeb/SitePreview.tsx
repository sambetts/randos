import '../NavMenu.css';
import React, { useEffect } from 'react';

import { WizardButtons } from '../WizardButtons';

export const SitePreview: React.FC<{ url: string, siteConfirmed: Function, siteCancel: Function }> = (props) => {

  const [imgPreview, setImgPreview] = React.useState<string | null>(null);
  const [loadError, setLoadError] = React.useState<boolean>(false);

  const handleError = () => {
    alert('Got an error loading the preview. Check JavaScript console for more info.');
    setLoadError(true);
  }

  const loadImg = (imgBase64: string) => {
    console.log(imgBase64);
    const imgSource = "data:image/png;base64, " + imgBase64;
    setImgPreview(imgSource);

  }

  useEffect(() => {
    fetch("https://localhost:44373/api/Screenshot?url=" + props.url, { mode: 'cors' })
      .then(res => {
        res.text().then(body => {
          if (!res.ok) {
            handleError();
          }
          else
            loadImg(body);
        })
      })
      .catch(err => handleError());
  }, [props.url]);

  return (
    <div id='webPreview'>

      <>
        <p>How's it look?</p>
        <img src='imgs/TeamsPreview.png' alt='Teams preview' />

        <div className='previewWeb'>
          {!loadError ?
            <>
              {imgPreview ?
                <img src={imgPreview} alt="Preview" />
                :
                <div>Loading preview...</div>
              }
            </>
            :
            <div>Got error loading site preview. It's complicated.</div>
          }
        </div>
      </>

      <WizardButtons nextClicked={() => props.siteConfirmed()} nextText="Confirm"
        previousText="Cancel" previousClicked={() => props.siteCancel()} />
    </div>
  );
};
