import { Avatar, Button, Tooltip, Comment, Form } from 'antd';
import React from 'react';
import { Room } from '../services/roomsService';

import moment from 'moment';
import { User } from '../services/userService';
import TextArea from 'antd/lib/input/TextArea';

interface Props {
  onMessage: (message: string) => void;
  room: Room;
}

export class RoomMessages extends React.Component<Props> {

  render() {

    const findUser = (room: Room, id: string) => room.participants.find(p => p.id === id) as User;
    const { room, onMessage } = this.props;
    const messages = room.messages
      ?.sort((a, b,) => new Date(b.created).getTime() - new Date(a.created).getTime())
      ?.map(m => {
        const user = findUser(room, m.authorId);
        return (
          <Comment
            author={<a>{user.firstname} {user.lastname}</a>}
            avatar={
              <Avatar
                src={user.avatarUrl}
              />
            }
            content={
              <p>{m.content}</p>
            }
            datetime={
              <Tooltip title={moment(m.created).format('YYYY-MM-DD HH:mm:ss')}>
                <span>{moment(m.created).fromNow()}</span>
              </Tooltip>
            }
          />
        );
      });

    return (
      <>
        <Editor onSubmit={message => onMessage(message)} />
        {messages}
      </>
    );
  }
}

const Editor = (props: { onSubmit: (message: string) => void }) => {
  const [form] = Form.useForm();
  return (
    <Form
      form={form}
      onFinish={(values) => { props.onSubmit(values.message); form.resetFields(); }}
    >
      <Form.Item name="message">
        <TextArea placeholder="Type you message here..." rows={4} />
      </Form.Item>
      <Form.Item>
        <Button htmlType="submit" type="primary">
          Send message
        </Button>
      </Form.Item>
    </Form>
  );
}