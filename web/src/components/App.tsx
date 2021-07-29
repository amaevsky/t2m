import React from 'react';
import { Route, Switch, Link } from 'react-router-dom';
import { FindRooms } from './FindRooms';
import { MyRooms } from './MyRooms';
import { Header } from './Header';
import { PrivateRoute } from '../utilities/privateRoute';
import { LoginRedirect } from './LoginRedirect';
import { Login } from './Login';
import { AccountSetup } from './AccountSetup';
import { Affix, Space } from 'antd';
import { ContactUs } from './ContactUs';
import { Terms } from './Terms';
import { Privacy } from './Privacy';

export default class App extends React.Component<any> {

  render() {
    return (
      <>
        <Affix offsetTop={0}>
          <Header />
        </Affix>
        <div style={{ padding: '16px 26px' }}>
          <Switch>
            <Route path="/help" component={HelpPages} />
            <Route path={['/redirect', '/login', '/account/setup']} component={LoginPages} />
            <PrivateRoute path="/rooms/my" component={MyRooms} />
            <PrivateRoute path="/" component={FindRooms} />
          </Switch>
        </div>
      </>
    );
  }
}

export const HelpPages = () => {
  return (
    <Switch>
      <Route path="/help/contact-us" component={ContactUs} />
      <Route path="/help/terms" component={Terms} />
      <Route path="/help/privacy" component={Privacy} />
    </Switch>
  );
}

export const LoginPages = () => {
  return (
    <Switch>
      <Route path="/redirect" component={LoginRedirect} />
      <Route path="/login" component={Login} />
      <Route path="/account/setup" component={AccountSetup} />
    </Switch>
  );
}