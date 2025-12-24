import api, { API_BASE_URL } from '../../../shared/services/base';
import type { ApiResponse } from '../../../shared/types';
import type { User } from '../../../shared/types';

export const authService = {
  login: async (username: string, password: string): Promise<ApiResponse<{ token?: string }>> => {
    const res = await api.post('/api/auth/login', { username, password });
    return res.data;
  },

  register: async (username: string, password: string, fullName: string): Promise<ApiResponse<User>> => {
    const res = await api.post('/api/auth/register', { username, password, fullName });
    return res.data;
  },

  logout: async (): Promise<ApiResponse> => {
    const res = await api.post('/api/auth/logout');
    return res.data;
  },

  getCurrentUser: async (): Promise<ApiResponse<User>> => {
    const res = await api.get('/api/auth/me');
    return res.data;
  },

  getImagePath: (username: string): string => 
    `${API_BASE_URL}/api/users/by-username/${username}/profile-picture`,
  
};

export default authService;

