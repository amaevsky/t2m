import React from 'react';
import { Route, Switch } from 'react-router-dom';
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
import { Landing } from './Landing';

export default class App extends React.Component<any> {

  render() {
    return (
      <>
        <Switch>
          <Route path="/landing" component={Landing} />
          <Route path="/help" component={HelpPages} />
          <Route path={['/redirect', '/login', '/account/setup']} component={LoginPages} />
          <PrivateRoute path="/" component={AppPages} />
        </Switch>
      </>
    );
  }
}

export const HelpPages = () => {
  return (
    <>
      <Affix offsetTop={0}>
        <Header empty={true} />
      </Affix>
      <div style={{ padding: '16px 26px' }}>
        <div className='help-container'>
          <Switch>
            <Route path="/help/contact-us" component={ContactUs} />
            <Route path="/help/terms" component={Terms} />
            <Route path="/help/privacy" component={Privacy} />
          </Switch>
        </div>
      </div>
      <Footer />
    </>
  );
}

export const AppPages = () => {
  return (
    <>
      <Affix offsetTop={0}>
        <Header />
      </Affix>
      <div style={{ padding: '16px 26px', minHeight: 'calc(100vh - 158px)' }}>
        <Switch>
          <PrivateRoute path="/rooms/my" component={MyRooms} />
          <PrivateRoute path="/" component={FindRooms} />
        </Switch>
      </div>
      <Footer />
    </>
  );
}

export const Footer = () => {
  return (
    <footer>
      <Space size='large'>
        <span>&copy; 2021 Talk2Me</span>
        <a target='_blank' href="/help/terms">Terms</a>
        <a target='_blank' href="/help/privacy">Privacy</a>
        <a target='_blank' href="/help/contact-us">Contact Us</a>
      </Space>
    </footer>
  );
}

export const LoginPages = () => {
  return (
    <>
      <Switch>
        <Route path="/redirect" component={LoginRedirect} />
        <Route path="/login" component={Login} />
        <Route path="/account/setup" component={AccountSetup} />
      </Switch>
      <Footer />
    </>
  );
}