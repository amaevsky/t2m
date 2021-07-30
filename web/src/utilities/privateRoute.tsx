import React from 'react';
import { Redirect, Route } from 'react-router';
import { routes } from '../components/App';
import { userService } from '../services/userService';
export class PrivateRoute extends React.Component<any>  {

  render() {
    const { component: Component, ...rest } = this.props;
    return (
      <Route
        {...rest}
        render={props =>
          userService.isAccountReady ? (
            <Component {...props} />
          ) : (
            <Redirect
              to={{
                pathname: routes.default,
                state: { from: props.location }
              }}
            />
          )
        }
      />
    );
  }
}