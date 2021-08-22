import { Col, Row, Space, Typography } from "antd";
import React from "react";
import { mapRooms, Room, roomsService } from "../services/roomsService";
import { userService } from "../services/userService";

import { connection } from "../realtime/roomsHub";
import { RoomCard, RoomCardAction } from "./RoomCard";
import { Link } from "react-router-dom";
import { routes } from "./App";

const { Title } = Typography;

interface State {
  upcoming: Room[];
  past: Room[];
}

interface Props {

}

export class MyRooms extends React.Component<Props, State> {
  constructor(props: Props) {
    super(props);

    this.state = {
      upcoming: [],
      past: []
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
  }

  componentWillUnmount() {
    connection.off("OnAdd");
    connection.off("OnChange");
    connection.off("OnRemove");
    connection.off("OnLeave");
    connection.off("OnEnter");
  }

  private async getData() {
    const [upcoming, past] = await Promise.all([roomsService.getUpcoming(), roomsService.getPast()]);
    this.setState({ upcoming, past });
  }

  private async leave(roomId: string) {
    await roomsService.leave(roomId);
  }

  private async remove(roomId: string) {
    await roomsService.remove(roomId);
  }

  private async join(roomId: string) {
    const ref = window.open(undefined, '_blank') as any;
    const link = await roomsService.join(roomId);
    if (link) {
      ref.location = link;
    } else {
      ref.close();
    }
  }

  render() {
    const { upcoming, past } = this.state;
    const upcomingCards = upcoming
      .sort((a, b) => a.startDate.getTime() - b.startDate.getTime())
      .map(r => {
        const secondary = [];
        const isFull = r.participants.length > 1;
        const startable = new Date(r.startDate).getTime() - Date.now() < 1000 * 60 * 5;
        const primary: RoomCardAction = {
          action: () => this.join(r.id),
          title: 'Join the room',
          disabled: !(startable && isFull)
        };

        if (!isFull) {
          primary.tooltip = 'Nobody has entered the room yet.';
        } else if (!startable) {
          primary.tooltip = 'Room can be joined 5 min before start.';
        }

        if (r.hostUserId === userService.user?.id) {
          secondary.push({ action: () => this.remove(r.id), title: 'Remove' });
        } else {
          secondary.push({ action: () => this.leave(r.id), title: 'Leave' });
        }
        secondary.push({ action: () => roomsService.sendCalendarEvent(r.id), title: 'Add to calendar' })

        return (
          <Col xl={4} md={6} sm={8} xs={12}>
            <RoomCard room={r} primaryAction={primary} secondaryActions={secondary} />
          </Col >
        )
      });

    const pastCards = past.map(r =>
      <Col xl={4} md={6} sm={8} xs={12}>
        <RoomCard room={r} />
      </Col >
    );

    return (
      <>
        <Space size='large' direction='vertical'>
          <div>
            <Title level={5}>Upcoming</Title>
            {upcoming.length ?
              <Row gutter={[16, 16]}>
                {upcomingCards}
              </Row>
              :
              <Row style={{ height: 240 }} align='middle' justify='center'>
                <Col style={{ fontSize: 14 }}>
                  <Row style={{ fontSize: 26 }} justify='center'><p>🤷‍♀️</p></Row>
                  <Row justify='center'> Currently you don’t have any upcoming rooms.</Row>
                  <Row justify='center'>Go&nbsp;<Link className="primary-color" to={routes.app.findRoom}><b>here</b></Link>&nbsp;and enter any room or create your own.</Row>
                </Col>
              </Row>
            }
          </div>
          <div>
            <Title level={5}>Past</Title>
            {past.length ?
              <Row gutter={[16, 16]}>
                {pastCards}
              </Row>
              :
              <Row style={{ height: 240 }} align='middle' justify='center'>
                <Col style={{ fontSize: 14 }}>
                  <Row style={{ fontSize: 26 }} justify='center'><p>🤦‍♀️</p></Row>
                  <Row justify='center'> Currently you don’t have any past rooms.</Row>
                </Col>
              </Row>
            }
          </div>
        </Space>
      </>
    )
  }
}