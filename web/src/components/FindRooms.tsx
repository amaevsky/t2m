import { Col, Divider, Row, Select, Space, Spin, Typography } from "antd";
import React from "react";
import { mapRooms, Room, RoomSearchOptions, roomsService } from "../services/roomsService";
import { userService } from "../services/userService";

import { Option } from "antd/lib/mentions";
import { connection } from "../realtime/roomsHub";
import { configService } from "../services/configService";
import { RoomCard } from "./RoomCard";
import { TimeRange } from "./TimeRange";
import { LastRooms } from "./LastRooms";
import { Tile } from "./Tile";
import { PlusOutlined } from "@ant-design/icons";
import { CreateRoomButton } from "./CreateRoomButton";

const { Title } = Typography

interface State {
  availableRooms: Room[];
  filter: RoomSearchOptions;
  loading: boolean;
  timeFrom?: Date;
  timeTo?: Date;
}

interface Props {

}

export class FindRooms extends React.Component<Props, State> {
  constructor(props: Props) {
    super(props);

    this.state = {
      availableRooms: [],
      filter: {},
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
        this.setState(prev => ({ availableRooms: [...prev.availableRooms, room] }));
      }
    });

    connection.on("OnChange", (room: Room) => {
      [room] = mapRooms([room]);
      if (!isMy(room)) {
        this.setState(prev => ({ availableRooms: [...replace(prev.availableRooms, room)] }));
      }
    });

    connection.on("OnRemove", (room: Room) => {
      [room] = mapRooms([room]);
      if (!isMy(room)) {
        this.setState(prev => ({ availableRooms: [...prev.availableRooms.filter(r => r.id !== room.id)] }));
      }
    });

    connection.on("OnEnter", (room: Room, by: string) => {
      [room] = mapRooms([room]);
      if (isMy(room)) {
        if (by === user?.id) {
          this.setState(prev => ({
            availableRooms: [...prev.availableRooms.filter(r => r.id !== room.id)]
          }));
        }
      } else {
        this.setState(prev => ({ availableRooms: [...prev.availableRooms.filter(r => r.id !== room.id)] }));
      }
    });

    connection.on("OnLeave", (room: Room, by: string) => {
      [room] = mapRooms([room]);
      if (!isMy(room)) {
        if (by === user?.id) {
          this.setState(prev => ({
            availableRooms: [...prev.availableRooms, room]
          }));
        }
        else {
          this.setState(prev => ({ availableRooms: [...prev.availableRooms, room] }));
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
    const rooms = await roomsService.getAvailable(this.state.filter);
    this.setState({ availableRooms: rooms, loading: false });
  }

  private async enter(roomId: string) {
    await roomsService.enter(roomId);
  }

  render() {
    const { filter, loading } = this.state;
    const filtredRooms = this.filter(filter)
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

            <Row style={{ padding: '8px 0', overflow: 'auto' }} gutter={16} wrap={false}>
              <Col>
                <Select maxTagCount={1} mode="tags" style={{ width: '150px' }} placeholder="Levels..." onChange={(values) => this.levelsChanged(values as string[])}>
                  {configService.config.languageLevels.map(l => <Option key={l.code} value={l.code}>{l.code}</Option>)}
                </Select>
              </Col>
              <Col>
                <Select maxTagCount={1} mode="tags" style={{ width: '150px' }} placeholder="Days of week..." onChange={(values) => this.daysChanged(values as string[])}>
                  {Object.keys(configService.config.days).map(d => <Option key={d}>{d}</Option>)}
                </Select>
              </Col>
              <Col>
                <TimeRange onChange={(value) => this.timeRangeChanged(value)} />
              </Col>
            </Row>

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

  private timeRangeChanged(value?: { from: Date, to: Date }) {
    if (value) {
      const { from: timeFrom, to: timeTo } = value;
      this.setState((prev) => ({ filter: { ...prev.filter, timeFrom, timeTo } }));
    } else {
      this.setState((prev) => ({ filter: { ...prev.filter, timeFrom: undefined, timeTo: undefined } }));
    }
  }

  private levelsChanged(levels: string[]) {
    if (levels) {
      this.setState((prev) => ({ filter: { ...prev.filter, levels } }));
    } else {
      this.setState((prev) => ({ filter: { ...prev.filter, levels: undefined } }));
    }
  }

  private daysChanged(values: string[]) {
    const days = values.map(v => configService.config.days[v]);
    if (days) {
      this.setState((prev) => ({ filter: { ...prev.filter, days } }));
    } else {
      this.setState((prev) => ({ filter: { ...prev.filter, days: undefined } }));
    }
  }

  private filter(filter: RoomSearchOptions): Room[] {
    const { availableRooms } = this.state;
    if (Object.values(filter || {}).some(v => v)) {
      return this.filterClientSide(filter, availableRooms);
    } else {
      return availableRooms;
    }
  }

  private filterClientSide(filter: RoomSearchOptions, rooms: Room[]): Room[] {
    const { levels, days, timeFrom, timeTo } = filter;
    if (levels?.length) {
      rooms = rooms.filter(r => levels.some(l => l === r.participants[0].languageLevel));
    }

    if (days?.length) {
      rooms = rooms.filter(r => days.some(d => d === new Date(r.startDate).getDay()));
    }

    if (timeFrom && timeTo) {
      const getMinutes = (date: Date) => date.getHours() * 60 + date.getMinutes();
      rooms = rooms.filter(r => getMinutes(new Date(r.startDate)) >= getMinutes(new Date(timeFrom))
        && getMinutes(new Date(r.startDate)) <= (getMinutes(new Date(timeTo)) || 1440));
    }

    return rooms;
  }
}
