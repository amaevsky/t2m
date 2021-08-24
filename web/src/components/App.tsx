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
import { ZoomDocumentation } from './ZoomDocumentation';
import useHeight from '../utilities/useHeight';

export const routes = {
  help: {
    contactUs: '/help/contact-us',
    zoomDocs: '/help/zoom',
    privacy: '/help/privacy',
    terms: '/help/terms'
  },
  login: {
    redirect: '/redirect',
    login: '/login',
    accountSetup: '/account/setup',
  },
  app: {
    myRooms: '/rooms/my',
    findRoom: '/rooms/find'
  },
  default: '/'
}

export default class App extends React.Component<any> {

  render() {
    return (
      <>
        <Switch>
          <Route path={Object.values(routes.help)} component={HelpPages} />
          <Route path={Object.values(routes.login)} component={LoginPages} />
          <PrivateRoute path={Object.values(routes.app)} component={AppPages} />
          <Route path={routes.default} component={Landing} />
        </Switch>
      </>
    );
  }
}

export const HelpPages = () => {
  const height = useHeight();

  return (
    <>
      <Affix offsetTop={0}>
        <Header empty={true} />
      </Affix>
      <div style={{ padding: '16px 26px', minHeight: `calc(${height} - 156px)` }}>
        <div className='help-container'>
          <Switch>
            <Route path={routes.help.contactUs} component={ContactUs} />
            <Route path={routes.help.zoomDocs} component={ZoomDocumentation} />
            <Route path={routes.help.terms} component={Terms} />
            <Route path={routes.help.privacy} component={Privacy} />
          </Switch>
        </div>
      </div>
      <Footer />
    </>
  );
}

export const AppPages = () => {
  const height = useHeight();

  return (
    <>
      <Affix offsetTop={0}>
        <Header />
      </Affix>
      <div style={{ display: 'flex', flexDirection: 'column', padding: '16px 26px', minHeight: `calc(${height} - 156px)` }}>
        <Switch>
          <PrivateRoute path={routes.app.myRooms} component={MyRooms} />
          <PrivateRoute path={routes.app.findRoom} component={FindRooms} />
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
        <a target='_blank' href={routes.help.terms}>Terms</a>
        <a target='_blank' href={routes.help.privacy}>Privacy</a>
        <a target='_blank' href={routes.help.contactUs}>Contact Us</a>
      </Space>
    </footer>
  );
}

export const LoginPages = () => {
  const height = useHeight();

  return (
    <>
      <div style={{ display: 'flex', padding: '16px 26px', minHeight: `calc(${height} - 78px)` }}>
        <Switch>
          <Route path={routes.login.redirect} component={LoginRedirect} />
          <Route path={routes.login.login} component={Login} />
          <Route path={routes.login.accountSetup} component={AccountSetup} />
        </Switch>
      </div>
      <Footer />
    </>
  );
}