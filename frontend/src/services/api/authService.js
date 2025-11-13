import api from './base.js'

export const authService = {
    login: (username, password) =>
        api.post('/api/account/login', { username, password }),

    register: (username, password, fullName) =>
        api.post('api/account/register', { username, password, fullName }),

    getProfile: () =>
        api.get('api/account')
}