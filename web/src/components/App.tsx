import React from 'react';
import { Route, BrowserRouter as Router, Switch } from 'react-router-dom';
import { FindRooms } from './FindRooms';
import { MyRooms } from './MyRooms';
import { Header } from './Header';
import { PrivateRoute } from '../utilities/privateRoute';
import { LoginRedirect } from './LoginRedirect';
import { Login } from './Login';
import { AccountSetup } from './AccountSetup';
import { Affix } from 'antd';

export default class App extends React.Component<any> {

  render() {
    return (
      <>
        <Affix offsetTop={0}>
          <Header />
        </Affix>
        <div style={{ padding: '0 10px' }}>
          <Switch>
            <Route path="/redirect" component={LoginRedirect} />
            <Route path="/login" component={Login} />
            <Route path="/account/setup" component={AccountSetup} />

            <PrivateRoute path="/rooms/my" component={MyRooms} />
            <PrivateRoute path="/" component={FindRooms} />
          </Switch>
        </div>
      </>
    );
  }
}