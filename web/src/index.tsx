import React from 'react';
import ReactDOM from 'react-dom';
import App from './components/App';

import 'antd/dist/antd.css';
import './index.css';
import { Route, BrowserRouter as Router, Switch, Redirect } from 'react-router-dom';
import { LoginRedirect } from './components/LoginRedirect';
import { Login } from './components/Login';
import { userService } from './services/userService';

class PrivateRoute extends React.Component<any>  {

  render() {
    const { component: Component, ...rest } = this.props;
    return (
      <Route
        {...rest}
        render={props =>
          userService.user ? (
            <Component {...props} />
          ) : (
            <Redirect
              to={{
                pathname: "/login",
                state: { from: props.location }
              }}
            />
          )
        }
      />
    );
  }
}

ReactDOM.render(
  <React.StrictMode>
    <Router>
      <Switch>
        <Route path="/redirect" component={LoginRedirect} />
        <Route path="/login" component={Login} />
        <PrivateRoute path="/" component={App} />
      </Switch>
    </Router>
  </React.StrictMode>,
  document.getElementById('root')
);

