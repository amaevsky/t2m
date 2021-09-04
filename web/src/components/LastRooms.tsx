import { Col, Row, Typography } from "antd";
import React from "react";
import { isDesktop } from "react-device-detect";
import { Room, roomsService } from "../services/roomsService";

import { RoomCard } from "./RoomCard";

const { Title } = Typography

interface State {
  rooms: Room[];
  loading: boolean;
}

interface Props {

}

export class LastRooms extends React.Component<Props, State> {
  constructor(props: Props) {
    super(props);

    this.state = {
      rooms: [],
      loading: true
    };
  }

  async componentDidMount() {
    await this.getData();
    document.getElementById('last-row')?.scrollTo({ left: 40 });
  }

  private async getData() {
    const rooms = await roomsService.getLast();
    this.setState({ rooms: rooms, loading: false });
  }

  render() {
    const { rooms } = this.state;
    const roomsCards = rooms.map(r => {
      return (
        <Col style={{ minWidth: 220, flexShrink: 0, padding: '0 8px 8px 8px' }}>
          <RoomCard room={r} type='shortcut' />
        </Col >
      )
    });

    return (
      <>
        {!!roomsCards?.length &&
          <>
            <Title level={5}>Recently entered</Title>
            <Row className={isDesktop ? 'scrollable' : ''} id='last-row' wrap={false} style={{ overflow: 'auto', marginLeft: '-8px' }}>
              {roomsCards}
            </Row>
          </>
        }
      </>
    );
  }
}
