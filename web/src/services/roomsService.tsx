import { notification } from 'antd';
import { http, HttpResponse } from '../utilities/http';
import { RoomParticipant } from './userService';
import { routes } from '../components/App';
import { sendAmplitudeData } from './amplitude';

const baseUrl = `rooms`;

export const mapRooms = (rooms: Room[]): Room[] => {
  return rooms.map(r => ({
    ...r,
    startDate: new Date(r.startDate),
    endDate: new Date(r.endDate)
  }));
}
class RoomsService {
  async join(roomId: string): Promise<string> {
    const resp = await http.get<string>(`${baseUrl}/join/${roomId}`);
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

  async getRequests(): Promise<Room[]> {
    return mapRooms((await http.get<Room[]>(`${baseUrl}/me/requests`)).data || []);
  }

  async getPast(): Promise<Room[]> {
    return mapRooms((await http.get<Room[]>(`${baseUrl}/me/past`)).data || []);
  }

  async enter(roomId: string) {
    const resp = await http.get(`${baseUrl}/enter/${roomId}`);
    if (!resp.errors) {
      notification.success({
        placement: 'bottomRight',
        message: <span>You successfully entered the room. You can find it on <a className="primary-color" href={routes.app.myRooms}>My rooms</a> page.</span>
      });

      sendAmplitudeData('Room_Entered', { roomId });
    }
  }

  async leave(roomId: string) {
    const resp = await http.get(`${baseUrl}/leave/${roomId}`);
    if (!resp.errors) {
      notification.success({
        placement: 'bottomRight',
        message: 'You successfully left the room.'
      });

      sendAmplitudeData('Room_Left', { roomId });
    }
  }

  async accept(roomId: string) {
    const resp = await http.get(`${baseUrl}/accept/${roomId}`);
    if (!resp.errors) {
      notification.success({
        placement: 'bottomRight',
        message: 'You successfully accepted the room request.'
      });
    }
  }

  async decline(roomId: string) {
    const resp = await http.get(`${baseUrl}/decline/${roomId}`);
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
    const resp = await http.get(`${baseUrl}/send_calendar_event/${roomId}`);
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

export const roomsService = new RoomsService();