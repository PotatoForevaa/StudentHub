import type { AxiosInstance, InternalAxiosRequestConfig } from 'axios';
import axios from 'axios';

// Constants
export const API_BASE_URL = 'http://localhost:5192';
export const API_TIMEOUT = 10000;

// Create axios instance with consistent configuration
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

// Response interceptor for automatic error handling
api.interceptors.response.use(
  (response) => {
    // Success response - pass through
    return response;
  },
  (error) => {
    // Handle common errors automatically
    if (error.response?.status === 401) {
      // Unauthorized - clear token and redirect to login
      localStorage.removeItem('token');
      console.warn('Session expired, redirecting to login');
      // Note: We don't redirect here to avoid circular imports
      // Let components handle the authentication flow
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
