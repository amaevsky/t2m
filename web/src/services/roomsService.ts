import axios from 'axios';
import { API_BASE_URL } from '../constants';
import { User } from './userService';

const baseUrl = `${API_BASE_URL}/rooms`;
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

  async enter(roomId: string) {
    await axios.get(`${baseUrl}/enter/${roomId}`, { withCredentials: true });
  }

  async quit(roomId: string) {
    await axios.get(`${baseUrl}/quit/${roomId}`, { withCredentials: true });
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
  participants: User[],
  hostUserId?: string;
  joinUrl: string;
}

export const roomsService = new RoomsService();