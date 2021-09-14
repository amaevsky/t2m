import { Modal } from "antd";
import { useState } from "react";
import { Room, roomsService, RoomUpdateOptions } from "../services/roomsService";
import { userService } from "../services/userService";

import { RoomCard, RoomCardAction } from "./RoomCard";
import { RoomEdit } from "./RoomEdit";
import { RoomMessages } from "./RoomMessages";

interface Props {
  room: Room;
}

const leave = async (roomId: string) => {
  await roomsService.leave(roomId);
}

const remove = async (roomId: string) => {
  await roomsService.remove(roomId);
}

const join = async (roomId: string) => {
  const ref = window.open(undefined, '_blank') as any;
  const link = await roomsService.join(roomId);
  if (link) {
    ref.location = link;
  } else {
    ref.close();
  }
}

export const UpcomingRoomCard = ({ room }: Props) => {

  const [messagesOpen, setMessagesOpen] = useState(false);
  const [editRoomOpen, setEditRoomOpen] = useState(false);

  const update = async (roomId: string, options: RoomUpdateOptions) => {
    var resp = await roomsService.update(roomId, options);
    if (!resp.errors) {
      setEditRoomOpen(false);
    }
  }

  const secondary = [];
  const isFull = room.participants.length > 1;
  const startable = new Date(room.startDate).getTime() - Date.now() < 1000 * 60 * 5;
  const primary: RoomCardAction = {
    action: () => join(room.id),
    title: 'Join the room',
    disabled: !(startable && isFull)
  };

  if (!isFull) {
    primary.tooltip = 'Nobody has entered the room yet.';
  } else if (!startable) {
    primary.tooltip = 'Room can be joined 5 min before start.';
  }

  secondary.push({ title: 'Edit', action: () => setEditRoomOpen(true) });
  secondary.push({ action: () => roomsService.sendCalendarEvent(room.id), title: 'Add to calendar' });
  secondary.push({ title: 'Messages', action: () => setMessagesOpen(true) });

  if (room.hostUserId === userService.user?.id) {
    secondary.push({ action: () => remove(room.id), title: 'Remove' });
  } else {
    secondary.push({ action: () => leave(room.id), title: 'Leave' });
  }

  return (
    <>
      <RoomCard room={room} type='full' primaryAction={primary} secondaryActions={secondary} />

      <Modal
        title="Messages"
        destroyOnClose={true}
        visible={messagesOpen}
        footer={null}
        onCancel={() => setMessagesOpen(false)}>
        <RoomMessages
          onMessage={message => roomsService.sendMessage(message, room.id)}
          room={room}
        />
      </Modal>

      <Modal
        title="Edit room"
        destroyOnClose={true}
        visible={editRoomOpen}
        footer={null}
        onCancel={() => setEditRoomOpen(false)}>
        <RoomEdit
          room={room}
          onEdit={(r) => update(room.id, r)}
          submitBtnText="Update" />
      </Modal>

    </>
  )
}