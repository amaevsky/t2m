import { http, HttpResponse } from '../utilities/http';
import { User } from './userService';

const baseUrl = `rooms`;
class RoomsService {
  async join(roomId: string): Promise<string> {
    return (await http.get<string>(`${baseUrl}/join/${roomId}`)).data || '';
  }

  async create(options: RoomCreateOptions): Promise<HttpResponse> {
    return await http.post(baseUrl, options);
  }

  async getAvailable(options?: RoomSearchOptions): Promise<Room[]> {
    let query = null;
    if (options) {
      query = this.buildSearchQuery(options);
    }
    return (await http.get<Room[]>(`${baseUrl}${query ? `?${query}` : ''}`)).data || [];

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
    return (await http.get<Room[]>(`${baseUrl}/me/upcoming`)).data || [];
  }

  async enter(roomId: string) {
    await http.get(`${baseUrl}/enter/${roomId}`);
  }

  async leave(roomId: string) {
    await http.get(`${baseUrl}/leave/${roomId}`);
  }

  async remove(roomId: string) {
    await http.delete(`${baseUrl}/${roomId}`);
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