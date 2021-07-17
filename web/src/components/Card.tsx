import { ClockCircleOutlined, MoreOutlined } from '@ant-design/icons';
import { Avatar, Button, Col, Dropdown, Menu, Row, Space } from 'antd';
import React from 'react';
import { Room } from '../services/roomsService';

import moment from 'moment';

interface Props {
  room: Room;
  primaryAction: { title: string, action: () => void };
  secondaryActions?: { title: string, action: () => void }[];
}

export class RoomCard extends React.Component<Props> {

  render() {

    const { room, secondaryActions, primaryAction } = this.props;
    const host = room.participants[0];
    return (
      <Tile style={{ width: 210, padding: 18 }}>
        <Row justify='space-between'>
          <Col>
            <Row gutter={8}>
              <Col>
                <Avatar size='large' src={host.avatarUrl}></Avatar>
              </Col>
              <Col>
                <b>{host.firstname} {host.lastname}</b>
                <p style={{ fontSize: 11 }}>{room.language} {host.languageLevel}</p>
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
            <ClockCircleOutlined />
            <p>{moment(room.startDate).format('MMM DD')}, {moment(room.startDate).format('HH:mm')} - {moment(room.startDate).format('HH:mm')}</p>
          </Space>
        </Row>
        <Row style={{ margin: '10px 0' }}>
          <Col>
            <span style={{ fontSize: 12 }}>
              {room.topic} dasjldah askdh dgas jggd ak dgagsdkags sd dkha khdk adha khdak
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


export const Tile = (props: any) => {
  return (
    <div {...props} className="tile">
      {props.children}
    </div>
  );
}