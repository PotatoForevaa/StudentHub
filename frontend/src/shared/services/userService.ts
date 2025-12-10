import api from './base';
import type { ApiResponse, User, ActivityDto } from '../types';

export const userService = {
  getAllUsers: async (): Promise<ApiResponse<User[]>> => {
    const res = await api.get('/api/Users');
    return res.data;
  },

  getById: async (id: string): Promise<ApiResponse<User>> => {
    const res = await api.get(`/api/Users/${id}`);
    return res.data;
  },

  getByUsername: async (username: string): Promise<ApiResponse<User>> => {
    const res = await api.get(`/api/Users/by-username/${username}`);
    return res.data;
  },

  getProfilePicture: async (username: string): Promise<Response> => {
    const token = localStorage.getItem('token');
    const headers: HeadersInit = {};
    if (token) headers['Authorization'] = `Bearer ${token}`;
    const url = `${api.defaults.baseURL}/api/Users/ProfilePicture/${username}`;
    return fetch(url, { headers });
  },

  uploadProfilePicture: async (file: File): Promise<ApiResponse> => {
    const formData = new FormData();
    formData.append('file', file);
    return api.post('/api/Users/ProfilePicture', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
  },

  getUserActivity: async (userId: string): Promise<ApiResponse<ActivityDto[]>> => {
    const res = await api.get(`/api/Users/${userId}/Activity`);
    return res.data;
  },

  getUserActivityByUsername: async (username: string): Promise<ApiResponse<ActivityDto[]>> => {
    const res = await api.get(`/api/Users/Activity/${username}`);
    return res.data;
  }
};

export default userService;
