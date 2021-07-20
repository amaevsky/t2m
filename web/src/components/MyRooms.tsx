import { Col, Row } from "antd";
import React from "react";
import { mapRooms, Room, roomsService } from "../services/roomsService";
import { userService } from "../services/userService";

import { connection } from "../realtime/roomsHub";
import { RoomCard } from "./Card";

interface State {
  myRooms: Room[];
}

interface Props {

}

export class MyRooms extends React.Component<Props, State> {
  constructor(props: Props) {
    super(props);

    this.state = {
      myRooms: []
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
        this.setState(prev => ({ myRooms: [...prev.myRooms, room] }));
      }
    });

    connection.on("OnChange", (room: Room, by: string) => {
      [room] = mapRooms([room]);
      if (isMy(room)) {
        this.setState(prev => ({ myRooms: [...replace(prev.myRooms, room)] }));
      }
    });

    connection.on("OnRemove", (room: Room, by: string) => {
      [room] = mapRooms([room]);
      if (isMy(room)) {
        this.setState(prev => ({ myRooms: [...prev.myRooms.filter(r => r.id !== room.id)] }));
      }
    });

    connection.on("OnEnter", (room: Room, by: string) => {
      [room] = mapRooms([room]);
      if (isMy(room)) {
        if (by === user?.id) {
          this.setState(prev => ({
            myRooms: [...prev.myRooms, room]
          }));
        }
        else {
          this.setState(prev => ({ myRooms: [...replace(prev.myRooms, room)] }));
        }
      }
    });

    connection.on("OnLeave", (room: Room, by: string) => {
      [room] = mapRooms([room]);
      if (isMy(room)) {
        this.setState(prev => ({ myRooms: [...replace(prev.myRooms, room)] }));
      } else {
        if (by === user?.id) {
          this.setState(prev => ({
            myRooms: [...prev.myRooms.filter(r => r.id !== room.id)]
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
    const upcoming = await roomsService.getUpcoming();
    this.setState({ myRooms: upcoming });
  }

  private async leave(roomId: string) {
    await roomsService.leave(roomId);
  }

  private async remove(roomId: string) {
    await roomsService.remove(roomId);
  }

  private async join(roomId: string) {
    const link = await roomsService.join(roomId);
    window.open(link, '_blank')
  }

  render() {
    const { myRooms } = this.state;
    const upcomingCards = myRooms
      .sort((a, b) => a.startDate.getTime() - b.startDate.getTime())
      .map(r => {
        const secondary = [];
        //const startable = new Date(r.startDate).getTime() - Date.now() < 1000 * 60 * 5 && r.participants.length > 1;
        const primary = { action: () => this.join(r.id), title: 'Join a room' };
        if (r.hostUserId === userService.user?.id) {
          secondary.push({ action: () => this.remove(r.id), title: 'Remove' });
        } else {
          secondary.push({ action: () => this.leave(r.id), title: 'Leave' });
        }

        return (
          <Col xl={4} md={6} sm={8} xs={12}>
            <RoomCard room={r} primaryAction={primary} secondaryActions={secondary} />
          </Col >
        )
      });

    return (
      <>
        <div style={{ padding: 16 }}>
          {!!myRooms.length &&
            <Row gutter={[16, 16]}>
              {upcomingCards}
            </Row>
          }
        </div>
      </>
    )
  }
}