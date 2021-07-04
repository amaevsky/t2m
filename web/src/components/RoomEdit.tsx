import { Button, DatePicker, Input, Select } from 'antd';
import Form from 'antd/lib/form';
import React from 'react';
import { Room } from '../services/roomsService';

interface Props {
  room: Partial<Room>,
  onEdit: (room: Room) => void
  submitBtnText: string
}

interface State {

}

export class RoomEdit extends React.Component<Props, State> {

  edit(values: any) {
    const room = { ... values} as Room;
    this.props.onEdit(room);
  }

  render() {
    return (
      <Form
        name="basic"
        labelCol={{ span: 8 }}
        wrapperCol={{ span: 16 }}
        initialValues={{ remember: true }}
        onFinish={(values) => this.edit(values)}
      >
        <Form.Item
          label="Topic"
          name="topic"
          rules={[{ required: true, message: 'Please input topic.' }]}
        >
          <Input />
        </Form.Item>

        <Form.Item
          label="Langugae"
          name="language"
          rules={[{ required: true, message: 'Please select a language.' }]}
        >
          <Select>
            <Select.Option value="English">English</Select.Option>
            <Select.Option value="Russian">Russian</Select.Option>
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

        <Form.Item wrapperCol={{ offset: 8, span: 16 }}>
          <Button type="primary" htmlType="submit">
            {this.props.submitBtnText}
          </Button>
        </Form.Item>
      </Form>
    );
  }
}