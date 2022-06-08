import React, { useEffect } from 'react';

import { WizardButtons } from '../WizardButtons';
import { Rings } from 'react-loader-spinner';

export const SitePreview: React.FC<{ url: string, siteConfirmed: Function, siteCancel: Function }> = (props) => {

  const [imgPreview, setImgPreview] = React.useState<string | null>(null);
  const [loadError, setLoadError] = React.useState<boolean>(false);

  const handleError = () => {
    alert('Got an error loading the preview. Check JavaScript console for more info.');
    setLoadError(true);
  }

  const loadImg = (imgBase64: string) => {

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
    <>
      <div id='webPreview'>

        <>
          <img src='imgs/TeamsPreview.png' alt='Teams preview' />

          <div className='previewWeb'>
            {!loadError ?
              <>
                {imgPreview ?
                  <img src={imgPreview} alt="Preview" />
                  :
                  <Rings ariaLabel="loading-indicator" color='#43488F' />
                }
              </>
              :
              <div>Got error loading site preview. It's complicated.</div>
            }
          </div>
        </>

      </div>
      <WizardButtons nextClicked={() => props.siteConfirmed()} nextText="Looks Good - Build the App"
        previousText="Go Back" previousClicked={() => props.siteCancel()} />
    </>

  );
};
