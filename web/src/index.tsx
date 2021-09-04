import React from 'react';
import ReactDOM from 'react-dom';
import App from './components/App';

import './styles/index.less';

import { configService } from './services/configService';
import { Route, BrowserRouter as Router, Switch } from 'react-router-dom';
import { userService } from './services/userService';
import { initAmplitude, sendAmplitudeData } from './services/amplitude';

configService.initialize()
  .then(() => initAmplitude(configService.config.amplitudeApiKey))
  .then(() => userService.initialize())
  .then(() => sendAmplitudeData('Session Started'))
  .then(() =>
    ReactDOM.render(
      <React.StrictMode>

        <Router>
          <Switch>
            <Route path="/" component={App} />
          </Switch>

        </Router>
      </React.StrictMode>,
      document.getElementById('root')
    )
  );



