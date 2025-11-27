import type { AxiosInstance } from 'axios';
import axios from 'axios';

export const baseUrl = 'http://localhost:5192';
export const api: AxiosInstance = axios.create({
  baseURL: baseUrl,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: true
});

export default api;