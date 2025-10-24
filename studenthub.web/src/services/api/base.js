import axios from 'axios';

export const api = axios.create({
    baseURL: process.env.REACT_APP_API_URL || 'https://api.yoursite.com',
    timeout: 10000,
    headers: {
        'Content-Type': 'application/json',
    },
    withCredential: true
});

export default api;