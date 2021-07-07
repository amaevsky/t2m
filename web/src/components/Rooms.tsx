import { Card, Col, Divider, Modal, Row } from "antd";
import { Button } from "antd";
import Title from "antd/lib/typography/Title";
import React from "react";
import { Room, RoomCreateOptions, roomsService } from "../services/roomsService";
import { userService } from "../services/userService";
import { RoomEdit } from "./RoomEdit";

import moment from 'moment';

interface State {
  rooms: Room[];
  upcoming: Room[];
  isAddRoomModalVisible: boolean
}

interface Props {

}

export class RoomList extends React.Component<Props, State> {
  constructor(props: Props) {
    super(props);

    this.state = {
      rooms: [],
      upcoming: [],
      isAddRoomModalVisible: false
    };
  }

  async componentDidMount() {
    await this.getData();
  }

  private async getData() {
    const [rooms, upcoming] = await Promise.all([roomsService.getAll(), roomsService.getUpcoming()]);
    this.setState({ rooms, upcoming });
  }

  private async enter(roomId: string) {
    await roomsService.enter(roomId);
    await this.getData();
  }

  private async createRoom(room: Room) {
    const options: RoomCreateOptions = { ...room }
    const created = await roomsService.create(options);
    this.setState({ isAddRoomModalVisible: false });
    await this.getData();
  }

  private async quit(roomId: string) {
    await roomsService.quit(roomId);
    await this.getData();
  }

  private async remove(roomId: string) {
    await roomsService.remove(roomId);
    await this.getData();
  }

  private async start(roomId: string) {
    await roomsService.start(roomId);
    await this.getData();
  }

  private setAddRoomModalVisibility(visibility: boolean) {
    this.setState({ isAddRoomModalVisible: visibility });
  }

  render() {
    const { rooms, upcoming, isAddRoomModalVisible } = this.state;
    const map = (rooms: Room[], upcoming: boolean) => rooms.map(r => {

      const actions = [];
      if (upcoming) {
        const startable = new Date(r.startDate).getTime() - Date.now() < 1000 * 60 * 5
        actions.push(<Button disabled={!startable} type='link' size='small' onClick={() => this.start(r.id)}>Start</Button>);
        if (r.hostUserId === userService.user?.id) {
          actions.push(<Button type='link' size='small' onClick={() => this.remove(r.id)}>Remove</Button>);
        } else {
          actions.push(<Button type='link' size='small' onClick={() => this.quit(r.id)}>Quit</Button>);
        }
      } else {
        actions.push(<Button type='link' size='small' onClick={() => this.enter(r.id)}>Enter</Button>);
      }
      return (
        <Col>
          <Card
            size='small'
            actions={actions}
            hoverable={true}
            style={{ width: 180 }}
            cover={
              <div style={{ background: '#1890ff', color: '#fff', padding: 18 }}>
                <Row justify='center'>
                  <Title style={{ color: '#fff' }} level={4}>{moment(r.startDate).format('DD MMM YY')}</Title>
                </Row>
                <Row justify='center'>
                  <span><b>{moment(r.startDate).format('HH:mm')}</b></span>
                </Row>
              </div>
            }
          >
            <p>Duration: {r.durationInMinutes} minutes</p>
            <p>Language: {r.language} ({r.participants.find(p => p.id === r.hostUserId)?.languageLevel})</p>
            <p style={{ visibility: r.topic ? 'visible' : 'hidden' }}>Topic: {r.topic}</p>
          </Card>
        </Col>
      )
    });

    const roomsCards = map(rooms, false);
    const upcomingCards = map(upcoming, true);

    return (
      <>
        <div style={{ padding: 16 }}>
          <Row justify='space-between'>
            <Button style={{ textTransform: 'uppercase' }} type="primary" onClick={() => this.setAddRoomModalVisibility(true)}>
              Create room
            </Button>
          </Row>

          {!!upcoming.length &&
            <>
              <Divider></Divider>
              <Title level={4}>My rooms</Title>
              <Row gutter={[16, 16]}>
                {upcomingCards}
              </Row>
            </>
          }
          <Divider></Divider>
          <Title level={4}>Find your room</Title>
          <Row gutter={[16, 16]}>
            {roomsCards}
          </Row>

          <Modal title="Create new room"
            visible={isAddRoomModalVisible}
            footer={null}
            onCancel={() => this.setAddRoomModalVisibility(false)}>
            <RoomEdit
              room={{}}
              onEdit={(room) => this.createRoom(room)}
              submitBtnText="Create" />
          </Modal>
        </div>
      </>
    )
  }
}