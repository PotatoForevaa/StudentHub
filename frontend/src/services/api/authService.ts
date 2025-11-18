import api from './base';

export const authService = {
  login: (username: string, password: string) =>
    api.post('/api/account/login', { username, password }),

  register: (username: string, password: string, fullName: string) =>
    api.post('/api/account/register', { username, password, fullName }),

  getProfile: () =>
    api.post(''),

  logout: () =>
    api.post('/api/account/logout'),

  getCurrentUser: () =>
    api.post('api/account/me')
};

export default authService;