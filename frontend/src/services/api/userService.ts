import api from './base';
import type { ApiResponse } from '../../types/Api';
import type { User } from '../../types/User';

export const userService = {
  getById: async (id: string): Promise<ApiResponse<User>> => {
    const res = await api.get(`/api/Users/${id}`);
    return res.data;
  },

  getProfilePicture: async (username: string): Promise<Response> => {
    const token = localStorage.getItem('token');
    const headers: HeadersInit = {};
    if (token) headers['Authorization'] = `Bearer ${token}`;
    const url = `${api.defaults.baseURL}/api/Users/ProfilePicture/${username}`;
    return fetch(url, { headers });
  }
};

export default userService;
