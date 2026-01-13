import api, { API_BASE_URL } from '../../../shared/services/base';
import type { ApiResponse } from '../../../shared/types';
import type { User } from '../../../shared/types';

export const authService = {
  login: async (username: string, password: string): Promise<ApiResponse<{ token?: string }>> => {
    const res = await api.post('/auth/login', { username, password });
    return res.data;
  },

  register: async (username: string, password: string, fullName: string): Promise<ApiResponse<User>> => {
    const res = await api.post('/auth/register', { username, password, fullName });
    return res.data;
  },

  logout: async (): Promise<ApiResponse> => {
    const res = await api.post('/auth/logout');
    return res.data;
  },

  getCurrentUser: async (): Promise<ApiResponse<User>> => {
    const res = await api.get('/auth/me');
    return res.data;
  },

  getProfilePicturePath: (username: string): string => 
    `${API_BASE_URL}/users/by-username/${username}/profile-picture`,
  
};

export default authService;

