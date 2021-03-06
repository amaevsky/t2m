import { http } from '../utilities/http';

const baseUrl = `config`;

interface LanguageLevel {
  code: string,
  description: string
}
class ConfigService {

  config: {
    languages: string[];
    zoomAuthUrl: string;
    amplitudeApiKey: string;
    languageLevels: LanguageLevel[]
    days: { [key: string]: number }
  }

  constructor() {
    this.config = {
      languages: [],
      zoomAuthUrl: '',
      amplitudeApiKey: '',
      languageLevels: [],
      days: {
        Sun: 0,
        Mon: 1,
        Tue: 2,
        Wed: 3,
        Thu: 4,
        Fri: 5,
        Sat: 6
      }
    }

  }

  async initialize() {
    const [languages, zoomAuthUrl, languageLevels, amplitudeApiKey] = await Promise.all([
      this.getLanguages(),
      this.getZoomAuthUrl(),
      this.getLanguageLevels(),
      this.getAmplitudeApiKey()
    ]);

    this.config = {
      ...this.config,
      languages,
      zoomAuthUrl,
      languageLevels,
      amplitudeApiKey
    };
  }

  private async getLanguages(): Promise<string[]> {
    return (await http.get<string[]>(`${baseUrl}/languages`)).data || [];
  }

  private async getZoomAuthUrl(): Promise<string> {
    return (await http.get<string>(`${baseUrl}/zoom`)).data || '';
  }

  private async getAmplitudeApiKey(): Promise<string> {
    return (await http.get<string>(`${baseUrl}/amplitude`)).data || '';
  }

  private async getLanguageLevels(): Promise<LanguageLevel[]> {
    return (await http.get<LanguageLevel[]>(`${baseUrl}/languagelevels`)).data || [];
  }
}

export const configService = new ConfigService();