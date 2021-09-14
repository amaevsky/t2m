import { Col, Row, Select, Typography } from "antd";
import React from "react";
import { Room, RoomSearchOptions } from "../services/roomsService";

import { Option } from "antd/lib/mentions";
import { configService } from "../services/configService";
import { TimeRange } from "./TimeRange";

interface Props {
  rooms: Room[],
  onFiltered: (rooms: Room[]) => void
}

export class RoomsFilter extends React.Component<Props> {
  constructor(props: Props) {
    super(props);

    this.filter = {};
  }

  componentDidUpdate(prevProps: Props) {
    if (prevProps.rooms !== this.props.rooms)
      this.props.onFiltered(this.doFilter(this.filter));
  }

  private filter: RoomSearchOptions;

  render() {
    return (
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
    )
  }

  private timeRangeChanged(value?: { from: Date, to: Date }) {
    if (value) {
      const { from: timeFrom, to: timeTo } = value;
      this.filter = { ...this.filter, timeFrom, timeTo };
    } else {
      this.filter = { ...this.filter, timeFrom: undefined, timeTo: undefined };
    }

    this.props.onFiltered(this.doFilter(this.filter));
  }

  private levelsChanged(levels: string[]) {
    if (levels) {
      this.filter = { ...this.filter, levels };
    } else {
      this.filter = { ...this.filter, levels: undefined };
    }

    this.props.onFiltered(this.doFilter(this.filter));
  }

  private daysChanged(values: string[]) {
    const days = values.map(v => configService.config.days[v]);
    if (days) {
      this.filter = { ...this.filter, days };
    } else {
      this.filter = { ...this.filter, days: undefined };
    }

    this.props.onFiltered(this.doFilter(this.filter));
  }

  private doFilter(filter: RoomSearchOptions): Room[] {
    const { rooms } = this.props;
    if (Object.values(filter || {}).some(v => v)) {
      return this.filterClientSide(filter, rooms);
    } else {
      return rooms;
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
