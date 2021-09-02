import { notification } from 'antd';
import { http } from '../utilities/http';
import { setAmplitudeUserId } from './amplitude';

const baseUrl = `user`;

class UserService {

  public user?: User | null;

  public get isAuthenticated(): boolean {
    return !!this.user;
  }

  public get isAccountReady(): boolean {
    return !!this.user?.languageLevel;
  }

  async initialize() {
    this.user = (await http.get<User>(`${baseUrl}/me`)).data || null;

    if(this.isAccountReady) { setAmplitudeUserId(this.user?.id || '') }
  }

  async getConnections(): Promise<User[]> {
    return (await http.get<User[]>(`${baseUrl}/me/connections`)).data || [];
  }

  async update(user: User, silent: boolean = false) {
    const resp = await http.put<User>(`${baseUrl}`, user);
    this.user = user;
    if (!resp.errors && !silent) {
      notification.success({
        placement: 'bottomRight',
        message: 'The changes were successfully saved.'
      });
    }
  }
}

export interface User {
  id: string;
  firstname: string;
  lastname: string;
  country: string;
  dateOfBirth: string;
  email: string;
  avatarUrl: string;
  targetLanguage: string;
  languageLevel: string;
  timezone: string;
}

export interface RoomParticipant extends User {
  Status: RoomPartcipantStatus
}

export enum RoomPartcipantStatus {
  Accepted,
  Requested,
  Declined
}

export const userService = new UserService();