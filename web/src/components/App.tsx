import { Dropdown, Menu, Avatar, Row, Col, Modal } from 'antd';
import Layout, { Content, Header } from 'antd/lib/layout/layout';
import React from 'react';
import { Route, Switch } from 'react-router-dom';
import { authService } from '../services/authService';
import { userService } from '../services/userService';
import { UserProfileEdit } from './AccountSetup';
import { RoomList } from './Rooms';

interface State {
  isEditProfileOpen: boolean;
}

export default class App extends React.Component<any, State> {

  constructor(props: any) {
    super(props);
    this.state = { isEditProfileOpen: false }
  }

  render() {

    const menu = (
      <Menu>
        <Menu.Item key="signout">
          <a onClick={() => this.logout()}>Sign out</a>
        </Menu.Item>
        <Menu.Item key="edit">
          <a onClick={() => this.setState({ isEditProfileOpen: true })}>Edit profile</a>
        </Menu.Item>

        <Modal width={700} title="Edit profile"
          visible={this.state.isEditProfileOpen}
          footer={null}
          onCancel={() => this.setState({ isEditProfileOpen: false })}>
          <UserProfileEdit />
        </Modal>
      </Menu>
    );

    return (
      <Layout>
        <Header>
          <Row justify='end'>
            <Col>
              <a>
                <Dropdown overlay={menu} trigger={['click']}>
                  <Avatar size='default' src={userService.user?.avatarUrl}></Avatar>
                </Dropdown>
              </a>
            </Col>
          </Row>
        </Header>
        <Content style={{ background: '#fff' }}>
          <Switch>
            <Route path="/" component={RoomList} />
          </Switch>
        </Content>
      </Layout>
    );
  }

  async logout(): Promise<void> {
    await authService.logout();
    this.props.history.push('/');
  }
}