import api, { API_BASE_URL } from '../../../shared/services/base';
import type { ApiResponse } from '../../../shared/types';
import type { Project, Comment, ScoreFormData, CommentFormData, UpdateProjectFormData } from '../types';

export const projectService = {
    getProjects: async (): Promise<ApiResponse<Project[]>> => {
        const response = await api.get('/api/projects');
        return response.data;
    },

    getProject: async (id: string): Promise<ApiResponse<Project>> => {
        const response = await api.get(`/api/projects/${id}`);
        return response.data;
    },

    addProject: async (formData: FormData): Promise<ApiResponse<Project>> => {
        const response = await api.post('/api/projects', formData, {
            headers: {
                'Content-Type': 'multipart/form-data',
            },
        });
        return response.data;
    },

    deleteProject: async (id: string): Promise<ApiResponse> => {
        const response = await api.delete(`/api/projects/${id}`);
        return response.data;
    },

    getImageList: async (id: string): Promise<ApiResponse> => {
        const response = await api.get(`/api/projects/${id}/images`);
        return response.data;
    },

    updateProject: async (id: string, formData: FormData): Promise<ApiResponse<Project>> => {
        const response = await api.put(`/api/projects/${id}`, formData, {
            headers: {
                'Content-Type': 'multipart/form-data',
            },
        });
        return response.data;
    },

    getComments: async (id: string): Promise<ApiResponse<Comment[]>> => {
        const response = await api.get(`/api/projects/${id}/comments`);
        return response.data;
    },

    addComment: async (id: string, data: CommentFormData): Promise<ApiResponse<Comment>> => {
        const response = await api.post(`/api/projects/${id}/comments`, data);
        return response.data;
    },

    addScore: async (id: string, data: ScoreFormData): Promise<ApiResponse<number>> => {
        const response = await api.post(`/api/projects/${id}/score`, data);
        return response.data;
    },

    getProjectsByUser: async (userId: string): Promise<ApiResponse<Project[]>> => {
        const response = await api.get(`/api/projects/author/${userId}`);
        return response.data;
    },

    getProjectImagePath: (id: string, path: string): string =>
        `${API_BASE_URL}/api/projects/${id}/images/${path}`
};
