import { Button, DatePicker, Input, Select } from 'antd';
import Form from 'antd/lib/form';
import React from 'react';
import { Room } from '../services/roomsService';
import { configService } from '../services/configService';
import { userService } from '../services/userService';

import moment from 'moment';
interface Props {
  room: Partial<Room>,
  onEdit: (room: Room) => void
  submitBtnText: string
}

interface State {

}

export class RoomEdit extends React.Component<Props, State> {

  edit(values: any) {
    const room = { ...values } as Room;
    this.props.onEdit(room);
  }

  render() {
    const user = userService.user;
    const is12Hours = () => {
      var date = new Date();
      var dateString = date.toLocaleTimeString();

      return !!(dateString.match(/am|pm/i) || date.toString().match(/am|pm/i));
    }

    return (
      <Form
        labelCol={{ span: 5 }}
        wrapperCol={{ span: 19 }}
        initialValues={{ language: user?.targetLanguage }}
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
            disabledDate={(date) => date && date < moment().startOf('day')}
            disabledTime={(date) => {
              if (date && date < moment().endOf('day')) {
                return {
                  disabledHours: () => Array.from({ length: 24 }, (_, i) => i).filter(h => h < moment().hour()),
                  disabledMinutes: () => Array.from({ length: 6 }, (_, i) => i * 10).filter(m => date.hour() === moment().hour() && m < moment().minute())
                }
              }

              return ({});
            }}
            showTime={{ format: is12Hours() ? 'h:mm A' : 'HH:mm', minuteStep: 10, use12Hours: is12Hours() }}
            format="DD-MMM-YYYY LT"
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

        <Form.Item wrapperCol={{ offset: 8, span: 16 }}>
          <Button type="primary" htmlType="submit">
            {this.props.submitBtnText}
          </Button>
        </Form.Item>
      </Form >
    );
  }
}