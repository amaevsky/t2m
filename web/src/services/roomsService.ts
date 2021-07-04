import axios from 'axios';

const baseUrl = 'https://localhost:44361/api/rooms';
class RoomsService {

  async create(options: RoomCreateOptions) {
    await axios.post(baseUrl, options);
  }

  async getAll(): Promise<Room[]> {
    return await axios.get(baseUrl);
  }

  async join(roomId: string) {
    await axios.post(baseUrl, roomId);
  }
}

export interface RoomCreateOptions {
  startDate: Date,
  language: string,
  topic: string
}

export interface Room {
  id: string,
  startDate: Date,
  language: string,
  topic: string,
  participants: string[],
  host: string
}

export const roomsService = new RoomsService();