import { http } from '../utilities/http';
import { userService } from './userService';

const baseUrl = `auth`;

class AuthService {

  async zoomLogin(code: string): Promise<boolean> {
    const { isNewAccount } = (await http.get(`${baseUrl}/login/zoom?authCode=${code}`)).data;
    return isNewAccount;
  }

  async logout(): Promise<void> {
    await http.get(`${baseUrl}/logout`);
    userService.user = null;
  }
}

export const authService = new AuthService();