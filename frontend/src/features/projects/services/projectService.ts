import api from '../../../shared/services/base';
import type { ApiResponse } from '../../../shared/types';
import type { Project, Comment, ScoreFormData, CommentFormData, UpdateProjectFormData } from '../types';

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
        const response = await api.get(`/api/Projects/${id}/GetImageList`);
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
    },

    updateProject: async (id: string, data: UpdateProjectFormData): Promise<ApiResponse<Project>> => {
        const response = await api.put(`/api/Projects/${id}`, data);
        return response.data;
    },

    getComments: async (id: string): Promise<ApiResponse<Comment[]>> => {
        const response = await api.get(`/api/Projects/${id}/Comments`);
        return response.data;
    },

    addComment: async (id: string, data: CommentFormData): Promise<ApiResponse<Comment>> => {
        const response = await api.post(`/api/Projects/${id}/Comments`, data);
        return response.data;
    },

    addScore: async (id: string, data: ScoreFormData): Promise<ApiResponse<number>> => {
        const response = await api.post(`/api/Projects/${id}/Score`, data);
        return response.data;
    },

    getProjectsByUser: async (userId: string): Promise<ApiResponse<Project[]>> => {
        const response = await api.get(`/api/Projects/GetByUser/${userId}`);
        return response.data;
    }
};
