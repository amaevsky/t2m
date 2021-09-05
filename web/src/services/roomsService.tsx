import { notification } from 'antd';
import { http, HttpResponse } from '../utilities/http';
import { User } from './userService';
import { routes } from '../components/App';
import { sendAmplitudeData } from './amplitude';
import { request } from 'https';

const baseUrl = `rooms`;

export const mapRooms = (rooms: Room[]): Room[] => {
  return rooms.map(r => mapRoom(r));
}

const mapRoom = (room: Room): Room => ({
  ...room,
  startDate: new Date(room.startDate),
  endDate: new Date(room.endDate)
});

class RoomsService {
  async join(roomId: string): Promise<string> {
    const resp = await http.get<string>(`${baseUrl}/${roomId}/join`);
    if (!resp.errors) {
      sendAmplitudeData('Room_Joined', { roomId });
    }

    return resp.data || '';
  }

  async create(options: RoomCreateOptions): Promise<HttpResponse> {
    const resp = await http.post<Room>(baseUrl, options);
    if (!resp.errors) {
      notification.success({
        placement: 'bottomRight',
        message: <span>The room was successfully created. You can find it on <a className="primary-color" href={routes.app.myRooms}>My rooms</a> page.</span>
      });

      sendAmplitudeData('Room_Created', { roomId: resp.data?.id });
    }
    return resp;
  }

  async getAvailable(options?: RoomSearchOptions): Promise<Room[]> {
    let query = null;
    if (options) {
      query = this.buildSearchQuery(options);
    }
    return mapRooms((await http.get<Room[]>(`${baseUrl}${query ? `?${query}` : ''}`)).data || []);
  }

  async getLast(): Promise<Room[]> {
    return mapRooms((await http.get<Room[]>(`${baseUrl}/last`)).data || []);
  }

  private buildSearchQuery(options: RoomSearchOptions): string {
    const { levels, days, timeFrom, timeTo } = options;
    const params = [];
    params.push(...(levels?.map(l => `levels=${l}`) || []));
    params.push(...(days?.map(d => `days=${d}`) || []));
    if (timeFrom) {
      params.push(timeFrom?.toISOString(), timeTo?.toISOString());
    }
    return params.join('&');
  }

  async getUpcoming(): Promise<Room[]> {
    return mapRooms((await http.get<Room[]>(`${baseUrl}/me/upcoming`)).data || []);
  }

  async getRequests(): Promise<RoomRequest[]> {
    const resp = await http.get<RoomRequest[]>(`${baseUrl}/me/requests`);
    const requests = resp.data || [];
    return requests.map(r => ({ ...r, room: mapRoom(r.room) }));
  }

  async getPast(): Promise<Room[]> {
    return mapRooms((await http.get<Room[]>(`${baseUrl}/me/past`)).data || []);
  }

  async enter(roomId: string) {
    const resp = await http.get(`${baseUrl}/${roomId}/enter`);
    if (!resp.errors) {
      notification.success({
        placement: 'bottomRight',
        message: <span>You successfully entered the room. You can find it on <a className="primary-color" href={routes.app.myRooms}>My rooms</a> page.</span>
      });

      sendAmplitudeData('Room_Entered', { roomId });
    }
  }

  async leave(roomId: string) {
    const resp = await http.get(`${baseUrl}/${roomId}/leave`);
    if (!resp.errors) {
      notification.success({
        placement: 'bottomRight',
        message: 'You successfully left the room.'
      });

      sendAmplitudeData('Room_Left', { roomId });
    }
  }

  async acceptRequest(roomId: string, requestId: string) {
    const resp = await http.get(`${baseUrl}/${roomId}/requests/${requestId}/accept`);
    if (!resp.errors) {
      notification.success({
        placement: 'bottomRight',
        message: 'You successfully accepted the room request.'
      });
    }
  }

  async declineRequest(roomId: string, requestId: string) {
    const resp = await http.get(`${baseUrl}/${roomId}/requests/${requestId}/decline`);
    if (!resp.errors) {
      notification.success({
        placement: 'bottomRight',
        message: 'You successfully declined the room request.'
      });
    }
  }

  async remove(roomId: string) {
    const resp = await http.delete(`${baseUrl}/${roomId}`);
    if (!resp.errors) {
      notification.success({
        placement: 'bottomRight',
        message: 'The room was successfully removed.'
      });

      sendAmplitudeData('Room_Removed', { roomId });
    }
  }

  async sendCalendarEvent(roomId: string) {
    const resp = await http.get(`${baseUrl}/${roomId}/send_calendar_event`);
    if (!resp.errors) {
      notification.success({
        placement: 'bottomRight',
        message: 'We have just sent you an email with calendar event.'
      });
    }
  }
}

export interface RoomCreateOptions {
  startDate: Date,
  durationInMinutes: number;
  language: string,
  topic?: string,
  participants: RoomParticipant[]
}

export interface RoomSearchOptions {
  levels?: string[];
  days?: number[];
  timeFrom?: Date;
  timeTo?: Date;
}

export interface Room {
  id: string;
  startDate: Date;
  endDate: Date;
  durationInMinutes: number;
  language: string;
  topic?: string;
  participants: RoomParticipant[],
  maxParticipants: number;
  hostUserId: string;
  joinUrl: string;
}


export interface RoomParticipant extends User {

}

export interface RoomRequest {
  id: string;
  to: User;
  from: User;
  status: RoomRequestStatus;
  room: Room;
}

export enum RoomRequestStatus {
  Requested,
  Accepted,
  Declined
}

export const roomsService = new RoomsService();