import api from './api/base'

export const authService = {
    login: (username, password) =>
        api.post('/api/account/login', { username, password }),

    register: (username, password, fullName) =>
        api.post('api/account/register', { username, password, fullName })
}