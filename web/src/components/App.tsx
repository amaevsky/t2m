import { Dropdown, Menu, Avatar, Row, Col, Modal, Button, Divider } from 'antd';
import Layout, { Content, Header } from 'antd/lib/layout/layout';
import React from 'react';
import { Route, Switch } from 'react-router-dom';
import { authService } from '../services/authService';
import { userService } from '../services/userService';
import { UserProfileEdit } from './UserProfileEdit';
import { RoomList } from './Rooms';
import { RoomEdit } from './RoomEdit';
import { Room, RoomCreateOptions, roomsService } from '../services/roomsService';

interface State {
  isEditProfileOpen: boolean;
  isAddRoomOpen: boolean
}

export default class App extends React.Component<any, State> {

  constructor(props: any) {
    super(props);
    this.state = { isEditProfileOpen: false, isAddRoomOpen: false }
  }

  render() {

    const menu = (
      <Menu>
        <Menu.Item key="signout">
          <a role='button' onClick={() => this.logout()}>Sign out</a>
        </Menu.Item>
        <Menu.Item key="edit">
          <a role='button' onClick={() => this.setState({ isEditProfileOpen: true })}>Edit profile</a>
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
      <>
        <Layout style={{ background: '#fff' }}>
          <Header style={{ background: '#fff', boxShadow: '0 2px 8px #f0f1f2' }}>
            <Row justify='space-between'>
              <Col span={1}>
                <p style={{ fontSize: 20, fontWeight: 600 }}>Lingua</p>
                <Divider type='vertical'></Divider>
              </Col>
              <Col span={20}>
                <Button type='primary' onClick={() => this.setState({ isAddRoomOpen: true })}>Create a room</Button>
              </Col>
              <Col>
                <a role='button'>
                  <Dropdown overlay={menu} trigger={['click']}>
                    <Avatar size='default' src={userService.user?.avatarUrl}></Avatar>
                  </Dropdown>
                </a>
              </Col>
            </Row>
          </Header>
          <Content>
            <Switch>
              <Route path="/" component={RoomList} />
            </Switch>
          </Content>
        </Layout>
        <Modal title="Create new room"
          visible={this.state.isAddRoomOpen}
          footer={null}
          onCancel={() => this.setState({ isAddRoomOpen: false })}>
          <RoomEdit
            room={{}}
            onEdit={(room) => this.createRoom(room)}
            submitBtnText="Create" />
        </Modal>
      </>
    );
  }

  private async createRoom(room: Room) {
    const options: RoomCreateOptions = { ...room }
    var resp = await roomsService.create(options);
    if (!resp.errors) {
      this.setState({ isAddRoomOpen: false });
    }
  }

  async logout(): Promise<void> {
    await authService.logout();
    this.props.history.push('/');
  }
}