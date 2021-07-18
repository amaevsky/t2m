import { Dropdown, Menu, Avatar, Row, Col, Modal, Button, Space } from 'antd';
import React from 'react';
import { Link, RouteComponentProps, withRouter } from 'react-router-dom';
import { authService } from '../services/authService';
import { userService } from '../services/userService';
import { UserProfileEdit } from './UserProfileEdit';
import { RoomEdit } from './RoomEdit';
import { Room, RoomCreateOptions, roomsService } from '../services/roomsService';
import { MenuOutlined } from '@ant-design/icons';
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

    const tabItems =
      <>
        <Menu.Item><Link to='/rooms/find'>Find a room</Link></Menu.Item>
        <Menu.Item><Link to='/rooms/my'>My rooms</Link></Menu.Item>
      </>;

    const accountActions =
      <>
        <Menu.Item key="signout">
          <Button size='small' type='link' onClick={() => this.logout()}>Sign out</Button>
        </Menu.Item>
        <Menu.Item key="edi2t">
          <Button size='small' type='link' onClick={() => this.setState({ isEditProfileOpen: true })}>Edit profile</Button>
        </Menu.Item>
      </>

    const tabs =
      <Menu style={{ background: 'initial', fontSize: 14, border: 'none', fontWeight: 600 }} mode='horizontal'>
        {tabItems}
      </Menu>;

    const menu = (
      <Menu mode='inline'>
        {accountActions}
      </Menu>
    );

    const createBtn =
      <Button
        type='primary'
        size='middle'
        onClick={() => this.setState({ isAddRoomOpen: true })}
      >Create a room
      </Button>;

    const mobile =
      <Menu mode='vertical'>
        <Menu.Item>
          {createBtn}
        </Menu.Item>
        <Menu.ItemGroup title="Navigation">
          {tabItems}
        </Menu.ItemGroup>
        <Menu.ItemGroup title="Account">
          {accountActions}
        </Menu.ItemGroup>
      </Menu>;

    return (
      <>
        <header style={{ lineHeight: '78px', padding: '0 50px' }} className='primary-background'>
          <Row justify='space-between' align='middle'>
            <Col>
              <Link to="/" className="primary-color" style={{ fontSize: 32, fontWeight: 700 }}>talk2me</Link>
            </Col>
            {userService.user &&
              <>
                {md &&
                  <>
                    <Col>
                      <Row align='middle'>
                        {tabs}
                      </Row>
                    </Col>
                    <Col>
                      <Row>
                        <Space size='large'>
                          {createBtn}
                          <a role='button'>
                            <Dropdown overlay={menu} trigger={['click']}>
                              <Space>
                                <MenuOutlined />
                                <Avatar size='default' src={userService.user?.avatarUrl}></Avatar>
                              </Space>
                            </Dropdown>
                          </a>
                        </Space>
                      </Row>

                    </Col>
                  </>
                }

                {!md &&
                  <>
                    <Col>
                      <a role='button'>
                        <Dropdown overlay={mobile} trigger={['click']}>
                          <Space>
                            <MenuOutlined />
                          </Space>
                        </Dropdown>
                      </a>
                    </Col>
                  </>
                }
              </>
            }
          </Row>
        </header>

        <Modal width={700} title="Edit profile"
          visible={this.state.isEditProfileOpen}
          footer={null}
          onCancel={() => this.setState({ isEditProfileOpen: false })}>
          <UserProfileEdit />
        </Modal>

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

export const Header = withBreakpoint(withRouter(HeaderComponent));