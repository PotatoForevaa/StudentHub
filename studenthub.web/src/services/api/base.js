import axios from 'axios';

export const api = axios.create({
    baseURL: 'http://localhost:5192',
    timeout: 10000,
    headers: {
        'Content-Type': 'application/json',
    },
    withCredential: true
});

export default api;