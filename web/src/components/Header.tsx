import { Dropdown, Menu, Avatar, Row, Col, Modal, Button, Space } from 'antd';
import React from 'react';
import { Link, RouteComponentProps } from 'react-router-dom';
import { authService } from '../services/authService';
import { userService } from '../services/userService';
import { UserProfileEdit } from './UserProfileEdit';
import { RoomEdit } from './RoomEdit';
import { Room, RoomCreateOptions, roomsService } from '../services/roomsService';
import { MenuOutlined } from '@ant-design/icons';
import { Tile } from './Card';
import { IHasBreakpoint, withBreakpoint } from '../utilities/withBreakpoints';

interface State {
  isEditProfileOpen: boolean;
  isAddRoomOpen: boolean
}

interface Props extends IHasBreakpoint, RouteComponentProps {

}

class HeaderComponent extends React.Component<Props, State> {

  constructor(props: any) {
    super(props);
    this.state = { isEditProfileOpen: false, isAddRoomOpen: false }
  }

  render() {

    const { md } = this.props.breakpoint;

    const menu = (
      <Menu mode='inline'>
        <Menu.Item key="signout">
          <Button type='link' onClick={() => this.logout()}>Sign out</Button>
        </Menu.Item>
        <Menu.Item key="edit">
          <Button type='link' onClick={() => this.setState({ isEditProfileOpen: true })}>Edit profile</Button>
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
        <header style={{ lineHeight: '78px', padding: '0 50px' }} className='primary-background'>
          <Row justify='space-between' align='middle'>
            <Col>
              <p className='primary-color' style={{ fontSize: 32, fontWeight: 700 }}>talk2me</p>
            </Col>
            <Col>
              <Row align='middle'>
                <Button type='primary' size='middle' onClick={() => this.setState({ isAddRoomOpen: true })}>Create a room</Button>
                <Menu mode='horizontal'>
                  <Menu.Item><Link to='/rooms/find'>Find a room</Link></Menu.Item>
                  <Menu.Item><Link to='/rooms/my'>My rooms</Link></Menu.Item>
                </Menu>
              </Row>
            </Col>
            <Col>
              <a role='button'>
                <Dropdown overlay={menu} trigger={['click']}>
                  <Tile style={{ padding: '8px 16px', lineHeight: '24px' }}>
                    <Space>
                      <MenuOutlined />
                      <Avatar size='default' src={userService.user?.avatarUrl}></Avatar>
                    </Space>
                  </Tile>
                </Dropdown>
              </a>
            </Col>
          </Row>
        </header>

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

export const Header = withBreakpoint(HeaderComponent);