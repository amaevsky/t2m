import React from 'react';
import ReactDOM from 'react-dom';
import App from './components/App';

import './styles/index.less';

import { configService } from './services/configService';
import { Route, BrowserRouter as Router, Switch } from 'react-router-dom';
import { Landing } from './components/Landing';
import { userService } from './services/userService';

Promise.all([
  configService.initialize(),
  userService.initialize()
]).then(() =>
  ReactDOM.render(
    <React.StrictMode>

      <Router>
        <Switch>
          <Route path="/landing" component={Landing} />
          <Route path="/" component={App} />
        </Switch>
      </Router>
    </React.StrictMode>,
    document.getElementById('root')
  )
);



