import { Col, Divider, Row, Space, Spin, Typography } from "antd";
import React from "react";
import { mapRooms, Room, roomsService } from "../services/roomsService";
import { userService } from "../services/userService";

import { connection } from "../realtime/roomsHub";
import { RoomCard } from "./RoomCard";
import { LastRooms } from "./LastRooms";
import { CreateRoomButton } from "./CreateRoomButton";
import { RoomsFilter } from "./RoomsFilter";

const { Title } = Typography

interface State {
  rooms: Room[];
  filtered: Room[],
  loading: boolean;
}

interface Props {

}

export class FindRooms extends React.Component<Props, State> {
  constructor(props: Props) {
    super(props);

    this.state = {
      rooms: [],
      filtered: [],
      loading: true
    };
  }

  async componentDidMount() {
    await this.getData();

    const user = userService.user;
    const isMy = (room: Room): boolean => room.participants.some(p => p.id === user?.id);
    const replace = (list: Room[], replace: Room): Room[] => {
      const i = list.findIndex(r => r.id === replace.id);
      list.splice(i, 1, replace);
      return list;
    };

    connection.on("OnAdd", (room: Room, by: string) => {
      [room] = mapRooms([room]);
      if (by !== user?.id) {
        this.setState(prev => ({ rooms: [...prev.rooms, room] }));
      }
    });

    connection.on("OnUpdate", (room: Room) => {
      [room] = mapRooms([room]);
      if (!isMy(room)) {
        this.setState(prev => ({ rooms: [...replace(prev.rooms, room)] }));
      }
    });

    connection.on("OnRemove", (room: Room) => {
      [room] = mapRooms([room]);
      if (!isMy(room)) {
        this.setState(prev => ({ rooms: [...prev.rooms.filter(r => r.id !== room.id)] }));
      }
    });

    connection.on("OnEnter", (room: Room, by: string) => {
      [room] = mapRooms([room]);
      if (isMy(room)) {
        if (by === user?.id) {
          this.setState(prev => ({
            rooms: [...prev.rooms.filter(r => r.id !== room.id)]
          }));
        }
      } else {
        this.setState(prev => ({ rooms: [...prev.rooms.filter(r => r.id !== room.id)] }));
      }
    });

    connection.on("OnLeave", (room: Room, by: string) => {
      [room] = mapRooms([room]);
      if (!isMy(room)) {
        if (by === user?.id) {
          this.setState(prev => ({
            rooms: [...prev.rooms, room]
          }));
        }
        else {
          this.setState(prev => ({ rooms: [...prev.rooms, room] }));
        }
      }
    });
  }

  componentWillUnmount() {
    connection.off("OnAdd");
    connection.off("OnUpdate");
    connection.off("OnRemove");
    connection.off("OnLeave");
    connection.off("OnEnter");
  }

  private async getData() {
    const rooms = await roomsService.getAvailable();
    this.setState({ rooms: rooms, loading: false });
  }

  private async enter(roomId: string) {
    await roomsService.enter(roomId);
  }

  render() {
    const { loading, filtered } = this.state;
    const filtredRooms = filtered
      .sort((a, b) => a.startDate.getTime() - b.startDate.getTime());
    const roomsCards = filtredRooms.map(r => {
      const action = { action: () => this.enter(r.id), title: 'Enter the room' };
      return (
        <Col xl={4} md={6} sm={8} xs={12}>
          <RoomCard room={r} type='full' primaryAction={action} />
        </Col >
      )
    });

    return (
      <>
        <Space size='large' direction='vertical'>
          <LastRooms />
          <div>
            <Title level={5}>Find a room</Title>

            <RoomsFilter
              rooms={this.state.rooms}
              onFiltered={(filtered) => this.setState({ filtered })}
            />

            <Divider></Divider>

            {loading ?
              <Row style={{ height: 240 }} align='middle' justify='center'>
                <Spin size='large'></Spin>
              </Row>
              :
              <Row gutter={[16, 16]}>
                <Col xl={4} md={6} sm={8} xs={12}>
                  <CreateRoomButton type='tile' />
                </Col>
                {!!roomsCards.length &&
                  roomsCards
                }
              </Row>
            }
          </div>
        </Space>
      </>
    )
  }
}
