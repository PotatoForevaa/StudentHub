import type { AxiosInstance, InternalAxiosRequestConfig } from 'axios';
import axios from 'axios';

export const API_BASE_URL1 = 'http://localhost:5192';
export const baseUrl = 'http://192.168.147.75:80';
export const API_BASE_URL = 'http://192.168.147.75:80';
export const API_TIMEOUT = 10000;

export const api: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  timeout: API_TIMEOUT,
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
    console.error('Request error:', error);
    return Promise.reject(error);
  }
);

api.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      console.warn('Session expired, redirecting to login');
    } else if (error.response?.status >= 500) {
      console.error('Server error:', error.response.status, error.response.statusText);
    } else if (error.response?.status >= 400 && error.response?.status < 500) {
      console.warn('Client error:', error.response.status, error.response.statusText);
    } else if (error.code === 'ECONNABORTED') {
      console.error('Request timeout - please try again');
    } else if (!error.response) {
      console.error('Network error - check your connection');
    }

    return Promise.reject(error);
  }
);

export default api;
