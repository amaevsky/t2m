import { Card, Col, Divider, Modal, Row } from "antd";
import { Button } from "antd";
import Title from "antd/lib/typography/Title";
import React from "react";
import { Room, RoomCreateOptions, roomsService } from "../services/roomsService";
import { RoomEdit } from "./RoomEdit";

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
    const [rooms, upcoming] = await Promise.all([roomsService.getAll(), roomsService.getUpcoming()]);
    this.setState({ rooms, upcoming });
  }

  join(roomId: string) {
    roomsService.join(roomId);
  }

  async createRoom(room: Room): Promise<void> {
    const options: RoomCreateOptions = { ...room }
    const created = await roomsService.create(options);
    this.setState((prev: State) => ({ rooms: [...prev.rooms, created], isAddRoomModalVisible: false }));
  }

  setAddRoomModalVisibility(visibility: boolean): void {
    this.setState({ isAddRoomModalVisible: visibility });
  }

  render() {
    const { rooms, upcoming, isAddRoomModalVisible } = this.state;
    const map = (rooms: Room[]) => rooms.map(r =>
      <Col span={6}>
        <Card
          size='small'
          actions={[
            <Button type='link' size='small' onClick={() => this.join(r.id)}>Join</Button>,
            <Button type='link' size='small' onClick={() => this.start(r.id)}>Start</Button>,
            <Button type='link' size='small' onClick={() => this.remove(r.id)}>Remove</Button>
          ]}>
          <p>Date: {r.startDate}</p>
          <p>Topic: {r.topic}</p>
          <p>Language: {r.language}</p>
          <p>Join: {r.joinUrl}</p>
        </Card>
      </Col>
    );


    const roomsCards = map(rooms);
    const upcomingCards = map(upcoming);

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

  async remove(roomId: string): Promise<void> {
    await roomsService.remove(roomId);
    this.setState((prev: State) => ({ rooms: [...prev.rooms.filter(r => r.id !== roomId)] }));
  }

  async start(roomId: string): Promise<void> {
    const room = await roomsService.start(roomId);

    this.setState((prev: State) => {
      const i = prev.rooms.findIndex(r => r.id === roomId);
      prev.rooms.splice(i, 1, room)
      return { rooms: [...prev.rooms] };
    });
  }
}