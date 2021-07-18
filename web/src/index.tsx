import React from 'react';
import ReactDOM from 'react-dom';
import App from './components/App';

import './styles/index.less';

import { configService } from './services/configService';

configService.initialize().then(() =>
  ReactDOM.render(
    <React.StrictMode>
      <App />
    </React.StrictMode>,
    document.getElementById('root')
  )
);



