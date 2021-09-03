import { notification } from 'antd';
import { http } from '../utilities/http';
import { sendAmplitudeData, setAmplitudeUserId, setAmplitudeUserProperties } from './amplitude';

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

    if (this.isAuthenticated) {
      setAmplitudeUserId(this.user?.id || '');
      setAmplitudeUserProperties({ email: this.user?.email })
    }

    sendAmplitudeData('User_StartSession');
  }


  async update(user: User, initialSetup: boolean = false) {
    const resp = await http.put<User>(`${baseUrl}`, user);
    this.user = user;
    if (!resp.errors) {
      if (!initialSetup) {
        notification.success({
          placement: 'bottomRight',
          message: 'The changes were successfully saved.'
        });
      }

      sendAmplitudeData('User_AccountSetUp')
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

export const userService = new UserService();