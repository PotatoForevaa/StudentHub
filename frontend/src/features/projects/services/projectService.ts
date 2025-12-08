import api from '../../../shared/services/base';
import type { ApiResponse } from '../../../shared/types';
import type { Project } from '../types';

export const projectService = {
    getProjects: async (): Promise<ApiResponse<Project[]>> => {
        const response = await api.get('/api/Projects');
        return response.data;
    },

    getProject: async (id: string): Promise<ApiResponse<Project>> => {
        const response = await api.get(`/api/Projects/${id}`);
        return response.data;
    },

    addProject: async (formData: FormData): Promise<ApiResponse<Project>> => {
        const response = await api.post('/api/Projects/Create', formData, {
            headers: {
                'Content-Type': 'multipart/form-data',
            },
        });
        return response.data;
    },

    deleteProject: async (id: string): Promise<ApiResponse> => {
        const response = await api.delete(`/api/Projects/Delete/${id}`);
        return response.data;
    },

    getImageList: async (id: string): Promise<ApiResponse> => {
        const response = await api.post(`/api/Projects/${id}/GetImageList`);
        return response.data;
    },

    getImage: async (id: string, path: string): Promise<Response> => {
        const token = localStorage.getItem('token');
        const headers: HeadersInit = {};
        if (token) {
            headers['Authorization'] = `Bearer ${token}`;
        }
        const url = `http://localhost:5192/api/Projects/${id}/${path}`;
        return fetch(url, { headers });
    }
};
