import { notification } from 'antd';
import axios, { AxiosError } from 'axios';

export const BASE_URL = process.env.NODE_ENV === 'development'
  ? 'https://localhost:44361'
  : 'https://lingua-api.azurewebsites.net';

export const API_BASE_URL = `${BASE_URL}/api`;

export interface HttpResponse<T = any> {
  errors?: string[],
  data?: T
}

class Http {
  get<T = any>(url: string): Promise<HttpResponse<T>> {
    return axios.get<T>(`${API_BASE_URL}/${url}`, { withCredentials: true })
      .then((response) => ({ data: response.data }))
      .catch(this.handleError);
  }
  delete<T = any>(url: string): Promise<HttpResponse<T>> {
    return axios.delete<T>(`${API_BASE_URL}/${url}`, { withCredentials: true })
      .then((response) => ({ data: response.data }))
      .catch(this.handleError);
  }
  post<T = any>(url: string, data?: any): Promise<HttpResponse<T>> {
    return axios.post<T>(`${API_BASE_URL}/${url}`, data, { withCredentials: true })
      .then((response) => ({ data: response.data }))
      .catch(this.handleError);
  }
  put<T = any>(url: string, data?: any): Promise<HttpResponse<T>> {
    return axios.put<T>(`${API_BASE_URL}/${url}`, data, { withCredentials: true })
      .then((response) => ({ data: response.data }))
      .catch(this.handleError);
  }

  private handleError(ex: AxiosError): HttpResponse<any> {
    const message = ex.response?.status === 400
      ? ex.response?.data as string
      : 'Error occured.';

    if (ex.response?.status === 400) {
      notification.error({ message, placement: 'bottomRight' });
    }
    
    return { errors: [message] };
  }

}

export const http = new Http();