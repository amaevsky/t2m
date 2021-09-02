import { Button, DatePicker, Input, Select } from 'antd';
import Form from 'antd/lib/form';
import React from 'react';
import { Room } from '../services/roomsService';
import { configService } from '../services/configService';
import { User, userService } from '../services/userService';

import moment from 'moment';
import { DateTimeFormat, is12Hours, TimeFormat } from '../utilities/date';
import { connection } from '../realtime/roomsHub';
interface Props {
  room: Partial<Room>,
  onEdit: (room: Room) => void
  submitBtnText: string;
  connections: User[];
}

interface State {

}

export class RoomEdit extends React.Component<Props, State> {

  edit(values: any) {
    const room = {
      ...values,
      startDate: (values.startDate as moment.Moment).seconds(0).milliseconds(0),
      participants: values.roommate ? [values.roommate] : []
    } as Room;
    this.props.onEdit(room);
  }

  render() {
    const user = userService.user;
    const { connections } = this.props;
    const timeStep = 10;
    const startDate = moment().add('minute', timeStep - (moment().minutes() % timeStep));

    return (
      <Form
        labelCol={{ span: 5 }}
        wrapperCol={{ span: 19 }}
        initialValues={{ language: user?.targetLanguage, startDate }}
        onFinish={(values) => this.edit(values)}
      >
        <Form.Item
          label="Language"
          name="language"
          rules={[{ required: true, message: 'Please select a language.' }]}
        >
          <Select disabled>
            {configService.config.languages.map(l => <Select.Option value={l}>{l}</Select.Option>)}
          </Select>
        </Form.Item>

        <Form.Item
          label="Date"
          name="startDate"
          rules={[{ required: true, message: 'Please select a date.' }]}
        >
          <DatePicker
            style={{ width: '100%' }}
            showNow={false}
            inputReadOnly={true}
            disabledDate={(date) => date && date < moment().startOf('day')}
            disabledTime={(date) => {
              if (date && date < moment().endOf('day')) {
                return {
                  disabledHours: () => Array.from({ length: 24 }, (_, i) => i).filter(h => h < moment().hour()),
                  disabledMinutes: () => Array.from({ length: 60 / timeStep }, (_, i) => i * timeStep).filter(m => date.hour() === moment().hour() && m < moment().minute())
                }
              }

              return ({});
            }}
            showTime={{ format: TimeFormat, minuteStep: 10, use12Hours: is12Hours }}
            format={DateTimeFormat}
          />
        </Form.Item>

        <Form.Item
          label="Duration"
          name="durationInMinutes"
          rules={[{ required: true, message: 'Please select a duration.' }]}
        >
          <Select>
            <Select.Option value={60}>60 minutes</Select.Option>
            <Select.Option value={30}>30 minutes</Select.Option>
          </Select>
        </Form.Item>

        <Form.Item
          label="Topic"
          name="topic"
          rules={[{ max: 50, type: 'string' }]}
        >
          <Input />
        </Form.Item>

        <Form.Item
          label="Roommate"
          name="roommate"
        >
          <Select showSearch>
            {
              connections.map(c =>
                <Select.Option value={c.id}>{c.firstname} {c.lastname}</Select.Option>
              )
            }
          </Select>
        </Form.Item>

        <Form.Item wrapperCol={{ offset: 8, span: 16 }}>
          <Button type="primary" htmlType="submit">
            {this.props.submitBtnText}
          </Button>
        </Form.Item>
      </Form >
    );
  }
}