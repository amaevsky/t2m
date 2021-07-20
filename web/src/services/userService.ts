import { http } from '../utilities/http';

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
  }

  async update(user: User) {
    await http.put<User>(`${baseUrl}`, user);
    this.user = user;
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
}

export const userService = new UserService();