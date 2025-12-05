import api from './base';
import type { ApiResponse } from '../../types/Api';
import type { User } from '../../types/User';

export const authService = {
  login: async (username: string, password: string): Promise<ApiResponse<{ token?: string }>> => {
    const res = await api.post('/api/Account/Login', { username, password });
    return res.data;
  },

  register: async (username: string, password: string, fullName: string): Promise<ApiResponse<User>> => {
    const res = await api.post('/api/Account/Register', { username, password, fullName });
    return res.data;
  },

  logout: async (): Promise<ApiResponse> => {
    const res = await api.post('/api/Account/Logout');
    return res.data;
  },

  getCurrentUser: async (): Promise<ApiResponse<User>> => {
    const res = await api.get('/api/Account/Me');
    return res.data;
  }
};

export default authService;