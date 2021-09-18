import { notification } from 'antd';
import axios, { AxiosError } from 'axios';

let base = 'https://api.t2m.app';
if (process.env.NODE_ENV === 'development') {
  base = 'https://localhost:5001';
} else if (process.env.REACT_APP_ENV === 'staging') {
  base = 'https://staging-api.t2m.app';
}

export const BASE_URL = base;

export const API_BASE_URL = `${BASE_URL}/api`;

export interface HttpResponse<T = any> {
  errors?: string[],
  data?: T
}

class Http {
  get<T = any>(url: string): Promise<HttpResponse<T>> {
    return axios.get<T>(`${API_BASE_URL}/${url}`, { headers: this.getDefaultHeaders() })
      .then((response) => ({ data: response.data }))
      .catch(this.handleError);
  }
  delete<T = any>(url: string): Promise<HttpResponse<T>> {
    return axios.delete<T>(`${API_BASE_URL}/${url}`, { headers: this.getDefaultHeaders() })
      .then((response) => ({ data: response.data }))
      .catch(this.handleError);
  }
  post<T = any>(url: string, data?: any, config?: any): Promise<HttpResponse<T>> {
    return axios.post<T>(`${API_BASE_URL}/${url}`, data, { ...config, headers: { ...config?.headers || {},  ...this.getDefaultHeaders() } })
      .then((response) => ({ data: response.data }))
      .catch(this.handleError);
  }
  put<T = any>(url: string, data?: any): Promise<HttpResponse<T>> {
    return axios.put<T>(`${API_BASE_URL}/${url}`, data, { headers: this.getDefaultHeaders() })
      .then((response) => ({ data: response.data }))
      .catch(this.handleError);
  }

  private handleError(ex: AxiosError): HttpResponse<any> {
    const message: string = ex.response?.data;

    if (ex.response?.status === 400) {
      notification.error({ message, placement: 'bottomRight' });
    }

    if (ex.response?.status === 500) {
      console.error(message);
      notification.error({ message: 'Oops. Something went wrong.', placement: 'bottomRight' });
    }

    return { errors: [message] };
  }

  private getDefaultHeaders() {
    const token = localStorage.getItem('token');
    return token ?  { Authorization: `Bearer ${token}` } : {};
  }

}

export const http = new Http();