import axios from 'axios';
import { API_BASE_URL } from '../constants';
import { User } from './userService';

const baseUrl = `${API_BASE_URL}/rooms`;
class RoomsService {
  async join(roomId: string): Promise<string> {
    return (await axios.get<string>(`${baseUrl}/join/${roomId}`, { withCredentials: true })).data;
  }

  async create(options: RoomCreateOptions): Promise<Room> {
    return (await axios.post<Room>(baseUrl, options, { withCredentials: true })).data;
  }

  async getAvailable(options?: RoomSearchOptions): Promise<Room[]> {
    let query = null;
    if (options) {
      query = this.buildSearchQuery(options);
    }
    return (await axios.get<Room[]>(`${baseUrl}${query ? `?${query}` : ''}`, { withCredentials: true })).data;
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
    return (await axios.get<Room[]>(`${baseUrl}/me/upcoming`, { withCredentials: true })).data;
  }

  async enter(roomId: string) {
    await axios.get(`${baseUrl}/enter/${roomId}`, { withCredentials: true });
  }

  async leave(roomId: string) {
    await axios.get(`${baseUrl}/leave/${roomId}`, { withCredentials: true });
  }

  async remove(roomId: string) {
    await axios.delete(`${baseUrl}/${roomId}`, { withCredentials: true });
  }

}

export interface RoomCreateOptions {
  startDate: Date,
  durationInMinutes: number;
  language: string,
  topic?: string
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
  durationInMinutes: number;
  language: string;
  topic?: string;
  participants: User[],
  maxParticipants: number;
  hostUserId: string;
  joinUrl: string;
}

export const roomsService = new RoomsService();