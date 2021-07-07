import { Button, Menu } from 'antd';
import React from 'react';
import { Route, BrowserRouter as Router, Switch, Link } from 'react-router-dom';
import { authService } from '../services/authService';
import { RoomList } from './Rooms';

export default class App extends React.Component<any> {

  render() {
    return (
      <>
        <Menu mode='horizontal'>
          <Menu.Item key='rooms'><Link to="/">Rooms</Link></Menu.Item>
          <Menu.Item key='signout'><Button onClick={() => this.logout()} type='link'>Sign out</Button></Menu.Item>
        </Menu>
        <Switch>
          <Route path="/" component={RoomList} />
        </Switch>
      </>
    );
  }

  async logout(): Promise<void> {
    await authService.logout();
    this.props.history.push('/');
  }
}