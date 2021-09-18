import { Dropdown, Menu, Avatar, Row, Col, Modal, Button, Space } from 'antd';
import React from 'react';
import { Link, RouteComponentProps, withRouter } from 'react-router-dom';
import { authService } from '../services/authService';
import { userService } from '../services/userService';
import { UserProfileEdit } from './UserProfileEdit';
import { RoomEdit } from './RoomEdit';
import { Room, RoomCreateOptions, roomsService } from '../services/roomsService';
import { CalendarOutlined, PlusOutlined, SearchOutlined } from '@ant-design/icons';
import { IHasBreakpoint, withBreakpoint } from '../utilities/withBreakpoints';
import { routes } from './App';
import { CreateRoomButton } from './CreateRoomButton';

interface State {
  isEditProfileOpen: boolean;
}

interface Props extends IHasBreakpoint, RouteComponentProps {
  empty?: boolean
}

class HeaderComponent extends React.Component<Props, State> {

  constructor(props: any) {
    super(props);
    this.state = { isEditProfileOpen: false }
  }

  render() {

    const { md } = this.props.breakpoint;
    const active = this.props.location.pathname;
    const tabs =
      <Menu overflowedIndicator={false} selectedKeys={[active]} style={{ background: 'initial', fontSize: 14, border: 'none', fontWeight: 600 }} mode='horizontal'>
        {md ?
          <>
            <Menu.Item key={routes.app.findRoom}><Link to={routes.app.findRoom}>Find a room</Link></Menu.Item>
            <Menu.Item key={routes.app.myRooms}><Link to={routes.app.myRooms}>My rooms</Link></Menu.Item>
          </>
          :
          <>
            <Menu.Item key={routes.app.findRoom}><Link to={routes.app.findRoom}><SearchOutlined style={{ fontSize: 16 }} /></Link></Menu.Item>
            <Menu.Item key={routes.app.myRooms}><Link to={routes.app.myRooms}><CalendarOutlined style={{ fontSize: 16 }} /></Link></Menu.Item>
          </>
        }
      </Menu>;

    const accountMenu = (
      <Menu mode='inline'>
        <Menu.Item key="edi2t">
          <Button size='small' type='link' onClick={() => this.setState({ isEditProfileOpen: true })}>Edit profile</Button>
        </Menu.Item>
        <Menu.Item key="signout">
          <Button size='small' type='link' onClick={() => this.logout()}>Sign out</Button>
        </Menu.Item>
      </Menu>
    );

    const createBtn =
      md ?
        <CreateRoomButton type='button' />
        :
        <CreateRoomButton type='icon' />

    return (
      <>
        <header>
          <Row justify='space-between' align='middle'>
            <Col>
              <Link to={routes.default}>
                <img height={45} src='talk2me-unfilled.png' />
              </Link>
            </Col>
            {!this.props.empty &&
              <>
                <Col>
                  <Row align='middle'>
                    <Space>
                      {tabs}
                      {createBtn}
                    </Space>
                  </Row>
                </Col>
                <Col>
                  <Row>
                    <a role='button'>
                      <Dropdown overlay={accountMenu} trigger={['click']}>
                        <Avatar size='default' src={userService.user?.avatarUrl}></Avatar>
                      </Dropdown>
                    </a>
                  </Row>
                </Col>
              </>
            }
          </Row>
        </header>

        <Modal
          title="Edit profile"
          destroyOnClose={true}
          visible={this.state.isEditProfileOpen}
          footer={null}
          onCancel={() => this.setState({ isEditProfileOpen: false })}>
          <UserProfileEdit afterSave={() => this.setState({ isEditProfileOpen: false })} />
        </Modal>


      </>
    );
  }

  async logout(): Promise<void> {
    await authService.logout();
    this.props.history.push(routes.default);
  }
}

export const Header = withRouter(withBreakpoint(HeaderComponent));