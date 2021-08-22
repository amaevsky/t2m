import { Button, Col, Dropdown, Row, Slider } from 'antd';
import React from 'react';
import moment from 'moment';
import { TimeFormat } from '../utilities/date';

interface Props {
  onChange: (value?: { from: Date, to: Date }) => void
}

interface State {
  from: Date,
  to: Date
}

export class TimeRange extends React.Component<Props, State> {

  constructor(props: Props) {
    super(props);

    this.state = {
      from: this.convertToTime(0),
      to: this.convertToTime(1440)
    };

  }

  render() {
    const { from, to } = this.state;

    return (
      <Dropdown
        trigger={['click']}
        overlay={
          <Row>
            <Col style={{ padding: '0 16px' }}>
              <Slider
                tooltipVisible={false}
                style={{ width: 200 }}
                range
                max={1440}
                step={30}
                defaultValue={[0, 1440]}
                onChange={value => this.timeRangeChanged(value)}
              />
            </Col>
          </Row>
        }>
        <Button type='default'>{moment(from).format(TimeFormat)} - {moment(to).format(TimeFormat)}</Button>
      </Dropdown>
    );
  }

  private convertToTime(minutes: number): Date {
    return moment().set({ hours: Math.floor(minutes / 60), minutes: minutes % 60 }).toDate();
  }

  private timeRangeChanged(values: [number, number]) {
    const [from, to] = values;
    const timeFrom = this.convertToTime(from);
    const timeTo = this.convertToTime(to);

    if (from !== 0 || to !== 24 * 60) {
      this.props.onChange({ from: timeFrom, to: timeTo });
    } else {
      this.props.onChange();
    }

    this.setState({ from: timeFrom, to: timeTo });
  }
}