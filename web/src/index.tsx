import React from 'react';
import ReactDOM from 'react-dom';
import App from './components/App';

import './styles/index.less';

import { configService } from './services/configService';
import { Route, Router, Switch } from 'react-router-dom';
import createBrowserHistory from 'history/createBrowserHistory'
import createHashHistory from 'history/createHashHistory'
import { userService } from './services/userService';
import { initAmplitude, sendAmplitudeData } from './services/amplitude';

var history: any;
const startApp = () => {
  configService.initialize()
    .then(() => initAmplitude(configService.config.amplitudeApiKey))
    .then(() => userService.initialize())
    .then(() => sendAmplitudeData('Session Started'))
    .then(() =>
      ReactDOM.render(
        <React.StrictMode>

          <Router history={history}>
            <Switch>
              <Route path="/" component={App} />
            </Switch>

          </Router>
        </React.StrictMode>,
        document.getElementById('root')
      )
    );
}

if (!(window as any).cordova) {
  history = createBrowserHistory()
  startApp()
}
else {
  history = createHashHistory()
  document.addEventListener('deviceready', startApp, false)
}



