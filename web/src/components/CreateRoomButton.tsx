import React from 'react';
import { PlusOutlined } from "@ant-design/icons";
import { Button, Modal, Space } from "antd";
import { RoomEdit } from './RoomEdit';
import { Room, RoomCreateOptions, roomsService } from '../services/roomsService';
import { User, userService } from '../services/userService';

interface Props {
  type: 'button' | 'icon' | 'tile';
}

interface State {
  isAddRoomOpen: boolean;
  connections?: User[];
}

export class CreateRoomButton extends React.Component<Props, State> {

  constructor(props: any) {
    super(props);
    this.state = { isAddRoomOpen: false, connections: [] }
  }

  private async createRoom(room: Room) {
    const options: RoomCreateOptions = { ...room }
    var resp = await roomsService.create(options);
    if (!resp.errors) {
      this.setState({ isAddRoomOpen: false });
    }
  }

  private async openModal() {
    this.setState({ isAddRoomOpen: true });
    const connections = await userService.getConnections();
    this.setState({ connections });
  }

  render() {
    const { type } = this.props;
    const { connections } = this.state;

    let button =
      <Button
        type='primary'
        size='middle'
        onClick={() => this.openModal()}
      >
        Create a room
      </Button>;

    if (type === 'icon') {
      button =
        <Button
          type='primary'
          size='middle'
          shape='circle'
          onClick={() => this.openModal()}
        >
          <PlusOutlined />
        </Button >
    }

    if (type === 'tile') {
      button =
        <div
          className="tile create-btn"
          onClick={() => this.openModal()}
        >
          <Space direction='vertical' align='center'>
            <PlusOutlined style={{ fontSize: 35 }} />
            <div style={{ fontSize: 15, fontWeight: 600 }}>Create a room</div>
          </Space>
        </div>
    }

    return (
      <>
        {button}
        <Modal
          title="Create new room"
          destroyOnClose={true}
          visible={this.state.isAddRoomOpen}
          footer={null}
          onCancel={() => this.setState({ isAddRoomOpen: false })}>
          <RoomEdit
            room={{}}
            onEdit={(room) => this.createRoom(room)}
            submitBtnText="Create"
            connections={connections || []}
          />
        </Modal>
      </>
    );
  }
}