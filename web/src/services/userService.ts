import axios from 'axios';

const baseUrl = 'https://localhost:44361/api/user';

class UserService {
  public user?:  User | null;

  async initialize(): Promise<void> {
    this.user = (await axios.get<User>(`${baseUrl}/me`, { withCredentials: true })).data || null;
  }

  async selectTargetLanguage(language: string) {
    const toUpdate: User = { ...this.user as User, targetLanguage: language };
    await axios.put<User>(`${baseUrl}`, toUpdate, { withCredentials: true });
    this.user = toUpdate;
  }
}

export interface User {
  id: string;
  firstname: string;
  lastname: string;
  email: string;
  targetLanguage: string;
}

export const userService = new UserService();