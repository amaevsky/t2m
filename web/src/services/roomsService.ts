class RoomsService {

  async create(options: RoomCreateOptions) {

  }

  async getAll(): Promise<Room[]> {
    return new Promise((resolve) => resolve(
      new Array(12).fill({
        id: '123',
        date: new Date(),
        language: 'English',
        topic: 'string',
        participants: [],
        host: 'string'
      })));
  }

  async join(roomId: string) {

  }
}

export interface RoomCreateOptions {
  date: Date,
  language: string,
  topic: string
}

export interface Room {
  id: string,
  date: Date,
  language: string,
  topic: string,
  participants: string[],
  host: string
}

export const roomsService = new RoomsService();