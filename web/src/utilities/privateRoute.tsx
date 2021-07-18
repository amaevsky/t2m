import React from 'react';
import { Redirect, Route } from 'react-router';
import { userService } from '../services/userService';
export class PrivateRoute extends React.Component<any>  {

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