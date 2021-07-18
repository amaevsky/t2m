import React from 'react';
import { Route, BrowserRouter as Router, Switch } from 'react-router-dom';
import { FindRooms } from './FindRooms';
import { MyRooms } from './MyRooms';
import { Header } from './Header';
import { PrivateRoute } from '../utilities/privateRoute';
import { LoginRedirect } from './LoginRedirect';
import { Login } from './Login';
import { AccountSetup } from './AccountSetup';

export default class App extends React.Component<any> {

  render() {
    return (

      <Router>
        <Header />
        <div style={{ padding: '0 10px' }}>
          <Switch>
            <Route path="/redirect" component={LoginRedirect} />
            <Route path="/login" component={Login} />

            <PrivateRoute path="/rooms/my" component={MyRooms} />
            <PrivateRoute path="/account/setup" component={AccountSetup} />
            <PrivateRoute path="/" component={FindRooms} />
          </Switch>
        </div>
        <Switch>


        </Switch>
      </Router>
    );
  }
}