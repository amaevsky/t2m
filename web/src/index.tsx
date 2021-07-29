import React from 'react';
import ReactDOM from 'react-dom';
import App from './components/App';

import './styles/index.less';

import { configService } from './services/configService';
import { Route, BrowserRouter as Router, Switch, Link } from 'react-router-dom';
import { Landing } from './components/Landing';
import { userService } from './services/userService';
import { Space } from 'antd';

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

        <footer>
          <Space size='large'>
            <a target='_blank' href="/help/terms">Terms</a>
            <a target='_blank' href="/help/privacy">Privacy</a>
            <a target='_blank' href="/help/contact-us">Contact Us</a>
          </Space>
        </footer>

      </Router>
    </React.StrictMode>,
    document.getElementById('root')
  )
);



