import { CalendarOutlined, ClockCircleOutlined, MoreOutlined, UserOutlined } from '@ant-design/icons';
import { Avatar, Button, Col, Dropdown, Menu, Row, Space } from 'antd';
import React from 'react';
import { Room } from '../services/roomsService';

import moment from 'moment';
import { userService } from '../services/userService';
import { Tile } from './Tile';

interface Props {
  room: Room;
  primaryAction: { title: string, action: () => void };
  secondaryActions?: { title: string, action: () => void }[];
}

export class RoomCard extends React.Component<Props> {

  render() {

    const { room, secondaryActions, primaryAction } = this.props;
    const partner = room.participants.find(p => userService.user?.id !== p.id);

    return (
      <Tile style={{ padding: 16 }}>
        <Row justify='space-between'>
          <Col>
            <Row gutter={8}>
              {partner &&
                <>
                  <Col>
                    <Avatar size='large' src={partner.avatarUrl}></Avatar>
                  </Col>
                  <Col>
                    <b>{partner.firstname} {partner.lastname[0]}.</b>
                    <p style={{ fontSize: 11 }}>{room.language} {partner.languageLevel}</p>
                  </Col>
                </>
              }
              {!partner &&
                <>
                  <Col>
                    <Avatar size='large' icon={<UserOutlined />}></Avatar>
                  </Col>
                  <Col>
                    <b>{'<roommate>'}</b>
                    <p style={{ fontSize: 11 }}>{room.language}</p>
                  </Col>
                </>
              }
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
            <p>{moment(room.startDate).format('ddd, MMM DD')}</p>
          </Space>
        </Row>
        <Row className='primary-color' style={{ fontSize: 12, fontWeight: 600 }}>
          <Space>
            <ClockCircleOutlined />
            <p>{moment(room.startDate).format('LT')} - {moment(room.endDate).format('LT')}</p>
          </Space>
        </Row>
        <Row style={{ margin: '10px 0', height: 40 }}>
          <Col>
            <span className="room-topic">
              {room.topic || '<no topic>'}
            </span>
          </Col>
        </Row>
        <Row>
          <Button onClick={primaryAction.action} style={{ width: '100%', fontWeight: 600 }} type='default' size='large'>{primaryAction.title}</Button>
        </Row>
      </Tile>
    );
  }

}