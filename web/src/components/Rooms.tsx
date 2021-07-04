import { Card, Col, Divider, Drawer, Row } from "antd";
import { Button } from "antd";
import React from "react";
import { Room, roomsService } from "../services/roomsService";

interface State {
  rooms: Room[];
  searchOpen: boolean;
}

interface Props {

}

export class RoomList extends React.Component<Props, State> {
  constructor(props: Props) {
    super(props);

    this.state = {
      rooms: [],
      searchOpen: false
    };
  }

  async componentDidMount() {
    const rooms = await roomsService.getAll();
    this.setState({ rooms });
  }

  showDrawer() {
    this.setState({ searchOpen: true });
  };
  onClose() {
    this.setState({ searchOpen: false });
  };

  join(roomId: string) {
    roomsService.join(roomId);
  }

  create() {
    roomsService.create(null as any);
  }

  render() {
    const { rooms, searchOpen } = this.state;
    const cards = rooms.map(r =>
      <Col span={6}>
        <Card actions={[<Button onClick={() => this.join(r.id)}>Join</Button>]}>
          {/* <p>Date: {r.date}</p> */}
          <p>Topic: {r.topic}</p>
          <p>Language: {r.language}</p>
        </Card></Col>
    );

    return (
      <>
        <Row justify='space-between' style={{ padding: 16 }}>
          <Col>
            <Button type='primary' onClick={() => this.create()}>Create room</Button>
          </Col>
          <Col>
            <Button type='dashed' onClick={() => this.showDrawer()}>Search</Button>
          </Col>
        </Row>
        <Divider></Divider>
        <Row style={{ padding: 16 }} gutter={[16, 16]}>
          {cards}
        </Row>
        <Drawer
          title="Basic Drawer"
          placement="right"
          closable={false}
          onClose={() => this.onClose()}
          visible={searchOpen}
        >
          <p>Some contents...</p>
          <p>Some contents...</p>
          <p>Some contents...</p>
        </Drawer>
      </>
    )
  };
}