import { CalendarOutlined, ClockCircleOutlined, MoreOutlined, UserOutlined } from '@ant-design/icons';
import { Avatar, Button, Col, Dropdown, Menu, Row, Space, Tooltip } from 'antd';
import React from 'react';
import { Room } from '../services/roomsService';

import moment from 'moment';
import { userService } from '../services/userService';
import { Tile } from './Tile';
import { DateFormat_DayOfWeek, TimeFormat } from '../utilities/date';

export interface RoomCardAction {
  title: string;
  action: () => void;
  disabled?: boolean;
  tooltip?: string;
}

interface Props {
  room: Room;
  primaryAction?: RoomCardAction;
  secondaryActions?: RoomCardAction[];
}

export class RoomCard extends React.Component<Props> {

  render() {

    const { room, secondaryActions, primaryAction } = this.props;
    const partner = room.participants.find(p => userService.user?.id !== p.id);
    const you = room.participants.find(p => userService.user?.id === p.id);

    const avatars = [];
    let levels = '';
    let names = '';
    if (!you && partner) {

      avatars.push(<Avatar size='large' src={partner.avatarUrl}></Avatar>);
      names = partner.firstname;
      levels = partner.languageLevel;
    } else if (you && partner) {
      avatars.push(
        <Avatar size='default' src={you.avatarUrl}></Avatar>,
        <Avatar size='default' src={partner.avatarUrl}></Avatar>
      );
      names = `You & ${partner.firstname}`;
      levels = `${partner.languageLevel} & ${partner.languageLevel}`;
    } else if (you) {
      avatars.push(
        <Avatar size='default' src={you.avatarUrl}></Avatar>,
        <Avatar size='default' icon={<UserOutlined />}></Avatar>
      );
      names = 'You & <???>';
      levels = `${you.languageLevel} & <?>`;
    }

    return (
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
      </Tile>
    );
  }

}