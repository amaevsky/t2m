import axios from 'axios';
import { API_BASE_URL } from '../constants';
import { userService } from './userService';

const baseUrl = `${API_BASE_URL}/auth`;

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