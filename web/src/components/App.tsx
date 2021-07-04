import { Menu } from 'antd';
import React from 'react';
import { Route, BrowserRouter as Router, Switch, Link } from 'react-router-dom';
import { Login } from './Login';
import { LoginRedirect } from './LoginRedirect';
import { RoomList } from './Rooms';

export default function App() {
  return (
    <Router>
      <Menu theme='dark' mode='horizontal'>
        <Menu.Item key='rooms'><Link to="/rooms">Rooms</Link></Menu.Item>
        <Menu.Item key='login'><Link to="/login">Login</Link></Menu.Item>
      </Menu>
      <Switch>
        <Route path="/redirect" component={LoginRedirect} />
        <Route path="/login" component={Login} />
        <Route path="/" component={RoomList} />
      </Switch>
    </Router>
  );
}
