import { Col, Row, Space, Spin, Typography } from "antd";
import React from "react";
import { mapRooms, Room, roomsService } from "../services/roomsService";
import { userService } from "../services/userService";

import { connection } from "../realtime/roomsHub";
import { RoomCard } from "./RoomCard";
import { CreateRoomButton } from "./CreateRoomButton";
import { UpcomingRoomCard } from "./UpcomingRoomCard";

const { Title } = Typography;

interface State {
  upcoming: Room[];
  past: Room[];
  loading: boolean;
}

interface Props {

}

export class MyRooms extends React.Component<Props, State> {
  constructor(props: Props) {
    super(props);

    this.state = {
      upcoming: [],
      past: [],
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
      if (by === user?.id) {
        this.setState(prev => ({ upcoming: [...prev.upcoming, room] }));
      }
    });

    connection.on("OnChange", (room: Room, by: string) => {
      [room] = mapRooms([room]);
      if (isMy(room)) {
        this.setState(prev => ({ upcoming: [...replace(prev.upcoming, room)] }));
      }
    });

    connection.on("OnRemove", (room: Room, by: string) => {
      [room] = mapRooms([room]);
      if (isMy(room)) {
        this.setState(prev => ({ upcoming: [...prev.upcoming.filter(r => r.id !== room.id)] }));
      }
    });

    connection.on("OnEnter", (room: Room, by: string) => {
      [room] = mapRooms([room]);
      if (isMy(room)) {
        if (by === user?.id) {
          this.setState(prev => ({
            upcoming: [...prev.upcoming, room]
          }));
        }
        else {
          this.setState(prev => ({ upcoming: [...replace(prev.upcoming, room)] }));
        }
      }
    });

    connection.on("OnLeave", (room: Room, by: string) => {
      [room] = mapRooms([room]);
      if (isMy(room)) {
        this.setState(prev => ({ upcoming: [...replace(prev.upcoming, room)] }));
      } else {
        if (by === user?.id) {
          this.setState(prev => ({
            upcoming: [...prev.upcoming.filter(r => r.id !== room.id)]
          }));
        }
      }
    });

    connection.on("OnMessage", (room: Room, messageId: string, by: string) => {
      [room] = mapRooms([room]);
      this.setState(prev => ({ upcoming: [...replace(prev.upcoming, room)] }));
    });
  }

  componentWillUnmount() {
    connection.off("OnAdd");
    connection.off("OnChange");
    connection.off("OnRemove");
    connection.off("OnLeave");
    connection.off("OnEnter");
    connection.off("OnMessage");
  }

  private async getData() {
    const [upcoming, past] = await Promise.all([roomsService.getUpcoming(), roomsService.getPast()]);
    this.setState({ upcoming, past, loading: false });
  }

  render() {
    const { upcoming, past, loading } = this.state;
    const upcomingCards = upcoming
      .sort((a, b) => a.startDate.getTime() - b.startDate.getTime())
      .map(r =>
        <Col xl={4} md={6} sm={8} xs={12}>
          <UpcomingRoomCard room={r} />
        </Col>
      );

    const pastCards = past.map(r =>
      <Col xl={4} md={6} sm={8} xs={12}>
        <RoomCard type='full' room={r} />
      </Col >
    );

    return (
      <>
        <Space size='large' direction='vertical'>
          <div>
            <Title level={5}>My rooms - Upcoming</Title>
            {loading ?
              <Row style={{ height: 240 }} align='middle' justify='center'>
                <Spin size='large'></Spin>
              </Row>
              :
              <Row gutter={[16, 16]}>
                <Col xl={4} md={6} sm={8} xs={12}>
                  <CreateRoomButton type='tile' />
                </Col>
                {!!upcoming.length &&
                  upcomingCards
                }
              </Row>
            }
          </div>
          <div>
            <Title level={5}>My rooms - Past</Title>
            {past.length ?
              <Row gutter={[16, 16]}>
                {pastCards}
              </Row>
              :
              <Row style={{ height: 240 }} align='middle' justify='center'>
                {loading ?
                  <Spin size='large'></Spin>
                  :
                  <Col style={{ fontSize: 14 }}>
                    <Row style={{ fontSize: 26 }} justify='center'><p>🤷‍♀️</p></Row>
                    <Row justify='center'> Currently you don’t have any past rooms.</Row>
                  </Col>
                }
              </Row>

            }
          </div>
        </Space>
      </>
    )
  }
}