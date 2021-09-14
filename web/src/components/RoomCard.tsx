import { CalendarOutlined, ClockCircleOutlined, MoreOutlined, UserOutlined } from '@ant-design/icons';
import { Avatar, Button, Col, Dropdown, Menu, Modal, Row, Space, Tooltip } from 'antd';
import React from 'react';
import { Room, roomsService } from '../services/roomsService';

import moment from 'moment';
import { User, userService } from '../services/userService';
import { Tile } from './Tile';
import { DateFormat_DayOfWeek, TimeFormat } from '../utilities/date';
import { RoomMessages } from './RoomMessages';

export interface RoomCardAction {
  title: string;
  action: () => void;
  disabled?: boolean;
  tooltip?: string;
}

interface State {
  isMessagesOpen: boolean;
}

interface Props {
  room: Room;
  type: 'full' | 'shortcut';
  primaryAction?: RoomCardAction;
  secondaryActions?: RoomCardAction[];
  showMessages?: boolean;
}

type CardUser = {
  name: string,
  avatar: React.ReactElement,
  level: string;
}

export class RoomCard extends React.Component<Props, State> {

  constructor(props: Props) {
    super(props);

    this.state = { isMessagesOpen: false };
  }

  render() {
    const getUsers = (room: Room): CardUser[] => {
      const { participants } = room;
      const partner = participants.find(p => userService.user?.id !== p.id);
      const you = participants.find(p => userService.user?.id === p.id);

      const users: CardUser[] = [];

      const CreateAvatar = (size: any, user: User) =>
        <Tooltip title={
          <>
            <b>{user.firstname} {user.lastname}</b>
            <div style={{ fontSize: 11 }}>{user.targetLanguage} {user.languageLevel}</div>
          </>
        }>
          <Avatar size={size} src={user.avatarUrl}></Avatar>
        </Tooltip>
        ;

      if (you) {
        users.push({
          name: 'You',
          level: you.languageLevel,
          avatar: CreateAvatar('default', you)
        });
        if (partner) {
          users.push({
            name: partner.firstname,
            level: partner.languageLevel,
            avatar: CreateAvatar('default', partner)
          });
        }
        else {
          users.push({
            name: '<???>',
            level: '<?>',
            avatar: <Avatar size='default' icon={<UserOutlined />}></Avatar>
          });
        }
      } else {
        participants.forEach(p => users.push({
          name: p.firstname,
          level: p.languageLevel,
          avatar: CreateAvatar(participants.length == 1 ? 'large' : 'default', p)
        }))
      }
      return users;
    }

    const { room, primaryAction, type } = this.props;
    let { secondaryActions, showMessages } = this.props;
    const { isMessagesOpen: isCommentsModalOpen } = this.state;

    if (showMessages) {
      const openMesssages: RoomCardAction = {
        title: 'Messages',
        action: () => this.setState({ isMessagesOpen: true })
      };

      secondaryActions = [...secondaryActions ?? [], openMesssages];
    }

    const users = getUsers(room);
    const avatars = users.map(u => u.avatar);
    const levels = users.map(u => u.level).join(' & ');
    const names = users.map(u => u.name).join(' & ');

    return (
      <>
        <Tile style={{ padding: 16 }}>
          <Row justify='space-between'>
            <Col>
              <Row gutter={8}>
                <Col>
                  <Avatar.Group>
                    {avatars}
                  </Avatar.Group>
                </Col>
                <Col>
                  <b>{names}</b>
                  <div style={{ fontSize: 11 }}>{room.language} {levels}</div>
                </Col>
              </Row>
            </Col>
            {secondaryActions &&
              <Col>
                <a role='button'>
                  <Dropdown overlay={
                    <Menu mode='inline'>
                      {
                        secondaryActions.map(a =>
                          <Menu.Item key={a.title}>
                            <Button size='small' type='link' onClick={a.action}>{a.title}</Button>
                          </Menu.Item>
                        )
                      }
                    </Menu>
                  } trigger={['click']}>
                    <MoreOutlined />
                  </Dropdown>
                </a>
              </Col>
            }
          </Row>
          <Row className='primary-color' style={{ fontSize: 12, fontWeight: 600, marginTop: 8 }}>
            <Space>
              <CalendarOutlined />
              <div>{moment(room.startDate).format(DateFormat_DayOfWeek)}</div>
            </Space>
          </Row>
          {type !== 'shortcut' &&
            <>
              <Row className='primary-color' style={{ fontSize: 12, fontWeight: 600 }}>
                <Space>
                  <ClockCircleOutlined />
                  <div>{moment(room.startDate).format(TimeFormat)} - {moment(room.endDate).format(TimeFormat)}</div>
                </Space>
              </Row>
              <Row style={{ margin: '10px 0', height: 40 }}>
                <Col>
                  <span className="room-topic">
                    {room.topic || '<no topic>'}
                  </span>
                </Col>
              </Row>

              {primaryAction &&
                <Row>
                  <Tooltip title={primaryAction.tooltip}>
                    <Button disabled={primaryAction.disabled} onClick={() => primaryAction.action()} style={{ width: '100%', fontWeight: 600 }} type='default' size='large'>
                      {primaryAction.title}
                    </Button>
                  </Tooltip>
                </Row>
              }
            </>
          }

        </Tile>

        <Modal
          title="Messages"
          destroyOnClose={true}
          visible={isCommentsModalOpen}
          footer={null}
          onCancel={() => this.setState({ isMessagesOpen: false })}>
          <RoomMessages
            onMessage={message => roomsService.sendMessage(message, room.id)}
            room={room}
          />
        </Modal>
      </>
    );
  }
}
