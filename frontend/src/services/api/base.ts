import type { AxiosInstance } from 'axios';
import axios from 'axios';

export const api: AxiosInstance = axios.create({
  baseURL: 'http://localhost:5192',
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: true
});

export default api;