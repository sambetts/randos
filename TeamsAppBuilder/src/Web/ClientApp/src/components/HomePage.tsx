import './NavMenu.css';
import React from 'react';
import { SelectSite } from './TeamifyWeb/SelectSite';
import { SitePreview } from './TeamifyWeb/SitePreview';
import { AppDetailsForm } from './TeamifyWeb/AppDetailsForm';
import { AppDownload } from './TeamifyWeb/AppDownload';
import { AppDetails } from '../models/AppDetails';


export interface Stage {

}

export const HomePage: React.FC<{}> = () => {

  const [url, setUrl] = React.useState<string>("");
  const [siteConfirmed, setSiteConfirmed] = React.useState<boolean>(false);
  const [appDetailsConfirmed, setAppDetailsConfirmed] = React.useState<boolean>(false);
  const [appDetails, setAppDetails] = React.useState<AppDetails | null>(null);


  const siteSelectCancel = () =>
  {
    setSiteConfirmed(false);
    setUrl("");
  }

  return (
    <div>

      {url === "" ?
        <SelectSite siteSelected={(url: string) => setUrl(url)} /> 
      :
        <>
        <p>{url}</p>
          {!siteConfirmed ?

            <SitePreview url={url} siteConfirmed={() => setSiteConfirmed(true)} siteCancel={() => siteSelectCancel()} />
            :
            <>
              {!appDetailsConfirmed && appDetails != null ?
                <AppDetailsForm detailsDone={(details: AppDetails) => setAppDetails(details)} />
                :
                <AppDownload details={appDetails!} url={url} />
              }
            </>

          }
        </>
      }

    </div>
  );
};
