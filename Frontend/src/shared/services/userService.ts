import api, { API_BASE_URL } from './base';
import type { ApiResponse, User, ActivityDto } from '../types';

export const userService = {
  getAllUsers: async (): Promise<ApiResponse<User[]>> => {
    const res = await api.get(`users`);
    return res.data;
  },

  getById: async (id: string): Promise<ApiResponse<User>> => {
    const res = await api.get(`$users/${id}`);
    return res.data;
  },

  getByUsername: async (username: string): Promise<ApiResponse<User>> => {
    const res = await api.get(`users/by-username/${username}`);
    return res.data;
  },

  getProfilePicture: async (username: string): Promise<Response> => {
    const res = await api.get(`users/by-username/${username}/profile-picture`);
    return res.data;
  },

  getProfilePicturePath: (username: string): string => 
    `${API_BASE_URL}/users/by-username/${username}/profile-picture`,

  uploadProfilePicture: async (file: File): Promise<ApiResponse> => {
    const formData = new FormData();
    formData.append('file', file);
    return api.post('/users/profile-picture', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
  },

  getUserActivity: async (userId: string): Promise<ApiResponse<ActivityDto[]>> => {
    const res = await api.get(`users/${userId}/activity`);
    return res.data;
  },

  getUserActivityByUsername: async (username: string): Promise<ApiResponse<ActivityDto[]>> => {
    const res = await api.get(`users/activity/${username}`);
    return res.data;
  }
};

export default userService;
