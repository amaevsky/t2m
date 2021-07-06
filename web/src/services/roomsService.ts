import axios from 'axios';

const baseUrl = 'https://localhost:44361/api/rooms';
class RoomsService {
  async start(roomId: string): Promise<Room> {
    return await (await axios.get<Room>(`${baseUrl}/start/${roomId}`, { withCredentials: true })).data;
  }

  async create(options: RoomCreateOptions): Promise<Room> {
    return (await axios.post<Room>(baseUrl, options, { withCredentials: true })).data;
  }

  async getAll(): Promise<Room[]> {
    return (await axios.get<Room[]>(baseUrl, { withCredentials: true })).data;
  }

  async getUpcoming(): Promise<Room[]> {
    return (await axios.get<Room[]>(`${baseUrl}/me/upcoming`, { withCredentials: true })).data;
  }

  async join(roomId: string) {
    await axios.get(`${baseUrl}/join/${roomId}`, { withCredentials: true });
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

export interface Room {
  id: string,
  startDate: Date,
  durationInMinutes: number;
  language: string,
  topic?: string,
  participants: string[],
  hostUserId?: string;
  joinUrl: string;
}

export const roomsService = new RoomsService();