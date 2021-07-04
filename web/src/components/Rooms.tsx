import { Card, Col, Divider, Modal, Row } from "antd";
import { Button } from "antd";
import React from "react";
import { Room, RoomCreateOptions, roomsService } from "../services/roomsService";
import { RoomEdit } from "./RoomEdit";

interface State {
  rooms: Room[];
  isAddRoomModalVisible: boolean
}

interface Props {

}

export class RoomList extends React.Component<Props, State> {
  constructor(props: Props) {
    super(props);

    this.state = {
      rooms: [],
      isAddRoomModalVisible: false
    };
  }

  async componentDidMount() {
    const rooms = await roomsService.getAll();
    this.setState({ rooms });
  }

  join(roomId: string) {
    roomsService.join(roomId);
  }

  createRoom(room: Room): void {
    const options: RoomCreateOptions = { ...room }
    roomsService.create(options);
  }
  setAddRoomModalVisibility(visibility: boolean): void {
    this.setState({ isAddRoomModalVisible: visibility });
  }

  render() {
    const { rooms, isAddRoomModalVisible } = this.state;
    const cards = rooms.map(r =>
      <Col span={6}>
        <Card actions={[<Button onClick={() => this.join(r.id)}>Join</Button>]}>
          <p>Date: {r.startDate.toDateString()}</p>
          <p>Topic: {r.topic}</p>
          <p>Language: {r.language}</p>
        </Card></Col>
    );

    return (
      <>
        <Row justify='space-between' style={{ padding: 16 }}>
          <Button style={{ textTransform: 'uppercase' }} type="primary" onClick={() => this.setAddRoomModalVisibility(true)}>
            Create room
          </Button>
        </Row>
        <Divider></Divider>
        <Row style={{ padding: 16 }} gutter={[16, 16]}>
          {cards}
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
      </>
    )
  }
}