import axios from 'axios';
import { userService } from './userService';

const baseUrl = 'https://localhost:44361/api/auth';

class AuthService {

  async zoomLogin(code: string): Promise<boolean> {
    const { isNewAccount } = (await axios.get(`${baseUrl}/login/zoom?authCode=${code}`, { withCredentials: true })).data;
    return isNewAccount;
  }

  async logout(): Promise<void> {
    await axios.get(`${baseUrl}/logout`, { withCredentials: true });
    userService.user = null;
  }
}

export const authService = new AuthService();