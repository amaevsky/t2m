import axios from 'axios';

const baseUrl = 'https://localhost:44361/api/config';
class ConfigService {

  config: {
    languages: string[];
    zoomAuthUrl: string;
    languageLelels: { code: string, description: string }[]
  }

  constructor() {
    this.config = {
      languages: [],
      zoomAuthUrl: '',
      languageLelels: []
    }

  }

  async initialize() {
    const [languages, zoomAuthUrl, languageLelels] = await Promise.all([
      this.getLanguages(),
      this.getZoomAuthUrl(),
      this.getLanguageLevels()
    ]);

    this.config = {
      languages,
      zoomAuthUrl,
      languageLelels
    };
  }

  private async getLanguages(): Promise<string[]> {
    return (await axios.get<string[]>(`${baseUrl}/languages`, { withCredentials: true })).data;
  }

  private async getZoomAuthUrl(): Promise<string> {
    return (await axios.get<string>(`${baseUrl}/zoom`)).data;
  }

  private async getLanguageLevels(): Promise<any> {
    return (await axios.get(`${baseUrl}/languagelevels`)).data;
  }
}

export const configService = new ConfigService();