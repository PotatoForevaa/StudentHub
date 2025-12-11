import type { AxiosInstance, InternalAxiosRequestConfig } from 'axios';
import axios from 'axios';

export const baseUrl = 'http://192.168.147.75:80';
export const baseUrl1 = 'http://localhost:5192';
export const api: AxiosInstance = axios.create({
  baseURL: baseUrl,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: true
});

api.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const token = localStorage.getItem('token');
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

export default api;

