import { Avatar, Card, Col, Collapse, Divider, Row, Select, Slider, Tooltip } from "antd";
import { Button } from "antd";
import React from "react";
import { Room, RoomSearchOptions, roomsService } from "../services/roomsService";
import { userService } from "../services/userService";

import moment from 'moment';
import { Option } from "antd/lib/mentions";
import { connection } from "../realtime/roomsHub";
import { configService } from "../services/configService";

const { Panel } = Collapse;

interface State {
  availableRooms: Room[];
  myRooms: Room[];
  isAddRoomModalVisible: boolean,
  filter: RoomSearchOptions,
}

interface Props {

}

export class RoomList extends React.Component<Props, State> {
  constructor(props: Props) {
    super(props);

    this.state = {
      availableRooms: [],
      myRooms: [],
      isAddRoomModalVisible: false,
      filter: {}
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
      if (by === user?.id) {
        this.setState(prev => ({ myRooms: [...prev.myRooms, room] }));
      } else {
        this.setState(prev => ({ availableRooms: [...prev.availableRooms, room] }));
      }
    });

    connection.on("OnChange", (room: Room, by: string) => {
      if (isMy(room)) {
        this.setState(prev => ({ myRooms: [...replace(prev.myRooms, room)] }));
      } else {
        this.setState(prev => ({ availableRooms: [...replace(prev.availableRooms, room)] }));
      }

    });

    connection.on("OnRemove", (room: Room, by: string) => {
      if (isMy(room)) {
        this.setState(prev => ({ myRooms: [...prev.myRooms.filter(r => r.id !== room.id)] }));
      } else {
        this.setState(prev => ({ availableRooms: [...prev.availableRooms.filter(r => r.id !== room.id)] }));
      }
    });

    connection.on("OnEnter", (room: Room, by: string) => {
      if (isMy(room)) {
        if (by === user?.id) {
          this.setState(prev => ({
            myRooms: [...prev.myRooms, room],
            availableRooms: [...prev.availableRooms.filter(r => r.id !== room.id)]
          }));
        }
        else {
          this.setState(prev => ({ myRooms: [...replace(prev.myRooms, room)] }));
        }
      } else {
        this.setState(prev => ({ availableRooms: [...prev.availableRooms.filter(r => r.id !== room.id)] }));
      }
    });

    connection.on("OnLeave", (room: Room, by: string) => {
      if (isMy(room)) {
        this.setState(prev => ({ myRooms: [...replace(prev.myRooms, room)] }));
      } else {
        if (by === user?.id) {
          this.setState(prev => ({
            availableRooms: [...prev.availableRooms, room],
            myRooms: [...prev.myRooms.filter(r => r.id !== room.id)]
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
    const [rooms, upcoming] = await Promise.all([roomsService.getAvailable(this.state.filter), roomsService.getUpcoming()]);
    this.setState({ availableRooms: rooms, myRooms: upcoming });
  }

  private async enter(roomId: string) {
    await roomsService.enter(roomId);
    //await this.getData();
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

  private setAddRoomModalVisibility(visibility: boolean) {
    this.setState({ isAddRoomModalVisible: visibility });
  }

  render() {
    const { myRooms, isAddRoomModalVisible, filter } = this.state;
    const map = (rooms: Room[], upcoming: boolean) => rooms.map(r => {

      const actions = [];
      if (upcoming) {
        const startable = new Date(r.startDate).getTime() - Date.now() < 1000 * 60 * 5 && r.participants.length > 1;
        const startBtn = <Button disabled={!startable} type='link' size='small' onClick={() => this.join(r.id)}>Join</Button>
        actions.push(startable
          ? startBtn
          : <Tooltip style={{ fontSize: 8 }} placement='top' title="You can join the room 5 min before start time. And only if there is enough participants.">
            {startBtn}
          </Tooltip>
        );
        if (r.hostUserId === userService.user?.id) {
          actions.push(<Button type='link' size='small' onClick={() => this.remove(r.id)}>Remove</Button>);
        } else {
          actions.push(<Button type='link' size='small' onClick={() => this.leave(r.id)}>Leave</Button>);
        }
      } else {
        actions.push(<Button type='link' size='small' onClick={() => this.enter(r.id)}>Enter</Button>);
      }

      const avatars =
        <Row>
          {
            r.participants.map(p => {
              return (
                <Col>
                  <Tooltip title={`${p.firstname} ${p.lastname}`} placement='bottom'>
                    {p.avatarUrl
                      ? <Avatar size='small' src={p.avatarUrl} />
                      : <Avatar size='small' style={{ background: '#509dff' }}>{`${p.firstname[0]}${p.lastname[0]}`}</Avatar>
                    }
                  </Tooltip>
                </Col>
              )
            })}
        </Row>
      return (
        <Col>
          <Card
            size='small'
            actions={actions}
            hoverable={true}
            style={{ width: 160 }}
            cover={
              <div style={{ background: '#1890ff', color: '#fff', padding: '5px 12px 18px 12px' }}>
                <Row style={{ fontSize: 11 }} justify='space-between'>
                  <p >{r.language}-{r.participants.find(p => p.id === r.hostUserId)?.languageLevel}</p>
                  <p >{r.durationInMinutes} min</p>
                </Row>
                <Row style={{ paddingTop: 5 }} justify='center'>
                  <p><b>{moment(r.startDate).format('HH:mm')}</b></p>
                </Row>
                <Row justify='center'>
                  <p style={{ fontSize: 12 }}>{moment(r.startDate).format('ddd DD MMM YY')}</p>
                </Row>
              </div>
            }
          >
            <div style={{ height: 40, fontSize: 12 }}>
              {avatars}
              <Row style={{ paddingTop: 5 }}>
                <p style={{ visibility: r.topic ? 'visible' : 'hidden' }}><i>{r.topic}</i></p>
              </Row>
            </div>
          </Card>
        </Col >
      )
    });

    const filtredRooms = this.filter(filter);
    const roomsCards = map(filtredRooms, false);
    const upcomingCards = map(myRooms, true);


    return (
      <>
        <div style={{ padding: 16 }}>
          {!!myRooms.length &&
            <Collapse defaultActiveKey={['1']} ghost>
              <Panel header="My rooms" key="1">
                <Row style={{ overflow: 'auto' }} gutter={[16, 16]} wrap={false}>
                  {upcomingCards}
                </Row>
              </Panel>
            </Collapse>
          }
          <Divider></Divider>
          <Collapse defaultActiveKey={['1']} ghost>
            <Panel header="Find your room" key="1">

              <Row gutter={16}>
                <Col span={4}>
                  <span>Level:</span>
                  <Select mode="tags" style={{ width: '100%' }} placeholder="Select levels..." onChange={(values) => this.levelsChanged(values as string[])}>
                    {configService.config.languageLevels.map(l => <Option key={l.code} value={l.code}>{l.code}</Option>)}
                  </Select>
                </Col>
                <Col span={4}>
                  <span>Days:</span>
                  <Select mode="tags" style={{ width: '100%' }} placeholder="Select days of week..." onChange={(values) => this.daysChanged(values as string[])}>
                    {Object.keys(configService.config.days).map(d => <Option key={d}>{d}</Option>)}
                  </Select>
                </Col>
                <Col span={4}>
                  <span>Start time:</span>
                  <Slider tipFormatter={value => value === undefined ? null : moment(this.convertToTime(value)).format('HH:mm')} range max={1440} step={30} defaultValue={[0, 1440]} onChange={value => this.timeRangeChanged(value)} />
                </Col>
              </Row>

              <Divider></Divider>

              <Row gutter={[16, 16]}>
                {roomsCards}
              </Row>

            </Panel>
          </Collapse>
        </div>
      </>
    )
  }

  private timeRangeChanged(values: [number, number]) {
    const [from, to] = values;
    if (from !== 0 || to !== 24 * 60) {
      const timeFrom = this.convertToTime(from);
      const timeTo = this.convertToTime(to);
      this.setState((prev) => ({ filter: { ...prev.filter, timeFrom, timeTo } }));
    } else {
      this.setState((prev) => ({ filter: { ...prev.filter, timeFrom: undefined, timeTo: undefined } }));
    }
  }

  private convertToTime(minutes: number): Date {
    return moment().set({ hours: Math.floor(minutes / 60), minutes: minutes % 60 }).toDate();
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