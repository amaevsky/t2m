import { Button, DatePicker, Input, Select } from 'antd';
import Form from 'antd/lib/form';
import React from 'react';
import { Room } from '../services/roomsService';

import 'moment-timezone';
import moment from 'moment';
import { configService } from '../services/configService';
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
    return (
      <Form
        name="basic"
        labelCol={{ span: 5 }}
        wrapperCol={{ span: 19 }}
        initialValues={{ language: 'English', timezone: moment.tz.guess() }}
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
          label="Start date"
          name="startDate"
          rules={[{ required: true, message: 'Please select a date.' }]}
        >
          <DatePicker
            showTime={{ format: 'HH:mm' }}
            format="YYYY-MM-DD HH:mm"
          />
        </Form.Item>

        <Form.Item
          label="Timezone"
          name="timezone"
          rules={[{ required: true, message: 'Please select a timezone.' }]}
        >
          <Select>
            {
              moment.tz.names().map(t => <Select.Option value={t}>{t}</Select.Option>)
            }
          </Select>
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