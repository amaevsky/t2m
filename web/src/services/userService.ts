import axios from 'axios';

const baseUrl = 'https://localhost:44361/api/user';

class UserService {
  public user?:  User | null;

  async initialize(): Promise<void> {
    this.user = (await axios.get<User>(`${baseUrl}/me`, { withCredentials: true })).data || null;
  }

  async update(user: User) {
    await axios.put<User>(`${baseUrl}`, user, { withCredentials: true });
    this.user = user;
  }
}

export interface User {
  id: string;
  firstname: string;
  lastname: string;
  email: string;
  targetLanguage: string;
  languageLevel: string;
  timezone: string;
}

export const userService = new UserService();