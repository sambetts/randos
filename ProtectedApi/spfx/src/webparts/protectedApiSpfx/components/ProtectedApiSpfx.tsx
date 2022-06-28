import * as React from 'react';
import styles from './ProtectedApiSpfx.module.scss';
import { IProtectedApiSpfxProps } from './IProtectedApiSpfxProps';
import { escape } from '@microsoft/sp-lodash-subset';
import { PrimaryButton, Stack, TextField } from '@fluentui/react';
import { AadHttpClient, HttpClientResponse } from '@microsoft/sp-http';
import { MSGraphClient } from '@microsoft/sp-http';
import * as MicrosoftGraph from '@microsoft/microsoft-graph-types';
export default class ProtectedApiSpfx extends React.Component<IProtectedApiSpfxProps, { apiEndpoint: string, apiAppId: string, jsonResponse: any | null }> {

  constructor(props: IProtectedApiSpfxProps) {
    super(props);
    this.state = {
      apiEndpoint: 'https://protectedapi.eu.ngrok.io/WeatherForecast',
      apiAppId: 'c0225ba5-3297-4e5e-afed-4e24ef3fa7d4',
      jsonResponse: null
    };
  }

  
  callApi() {
    console.log('Trying to call API...');
    if (this.props.context.aadHttpClientFactory) {
      this.props.context.aadHttpClientFactory
        .getClient(this.state.apiAppId)
        .then((client: AadHttpClient): void => {
          client
            .get(this.state.apiEndpoint, AadHttpClient.configurations.v1)
            .then(async (response: HttpClientResponse): Promise<any> => {
              if (response.ok)
                return response.json();
              else
                var msg: string;
              msg = await response.text();
              return response.statusText + msg;
            })
            .then((data: any): void => {
              // Show data
              this.setState({ jsonResponse: JSON.stringify(data, null, 2) });
            });
        })
        .catch(err => this.setState({ jsonResponse: err }));
    }
    else {
      this.setState({ jsonResponse: "Config error: no this.props.context.aadHttpClientFactory found" });
    }
  }

  callGraph() {
    console.log('Trying to call Graph...');
    if (this.props.context.msGraphClientFactory) {
      this.props.context.msGraphClientFactory
      .getClient()
      .then((client: MSGraphClient): void => {
        // get information about the current user from the Microsoft Graph
        client
          .api('/me/messages')
          .get((error, response: MicrosoftGraph.Message[], rawResponse?: any) => {
            // handle the response
            this.setState({ jsonResponse: JSON.stringify(response, null, 2) });
          });
      });

    }
    else {
      this.setState({ jsonResponse: "Config error: no this.context.msGraphClientFactory found" });
    }
  }

  public render(): React.ReactElement<IProtectedApiSpfxProps> {
    const {
      isDarkTheme,
      environmentMessage,
      hasTeamsContext,
      userDisplayName
    } = this.props;
    return (
      <section className={`${styles.protectedApiSpfx} ${hasTeamsContext ? styles.teams : ''}`}>
        <div className={styles.welcome}>
          <img alt="" src={isDarkTheme ? require('../assets/welcome-dark.png') : require('../assets/welcome-light.png')} className={styles.welcomeImage} />
          <h2>Well done, {escape(userDisplayName)}!</h2>
          <div>{environmentMessage}</div>
        </div>
        <div>
          <h3>Welcome to SharePoint Framework!</h3>
          <p>
            Click the button below to call the protected API.
          </p>
          <div>
            <TextField label="API endpoint" value={this.state.apiAppId} onChange={e => { this.setState({ apiAppId: (e.target as HTMLInputElement).value }); }} />
            <TextField label="App ID" value={this.state.apiEndpoint} onChange={e => { this.setState({ apiEndpoint: (e.target as HTMLInputElement).value }); }} />
          </div>
          <Stack>
            <PrimaryButton text="Call API" onClick={() => this.callApi()} />
            <PrimaryButton text="Call Graph" onClick={() => this.callGraph()} />
          </Stack>
          <p>API Response:</p>
          <div><pre>{this.state.jsonResponse}</pre></div>

          <p>Graph:</p>
          <p></p>
        </div>
      </section>
    );
  }
}
