import api, { API_BASE_URL } from '../../../shared/services/base';
import type { ApiResponse } from '../../../shared/types';
import type { Project, Comment, ScoreFormData, CommentFormData, UpdateProjectFormData } from '../types';

export const projectService = {
    getProjects: async (): Promise<ApiResponse<Project[]>> => {
        const response = await api.get('projects');
        return response.data;
    },

    getProject: async (id: string): Promise<ApiResponse<Project>> => {
        const response = await api.get(`projects/${id}`);
        return response.data;
    },

    addProject: async (formData: FormData): Promise<ApiResponse<Project>> => {
        const response = await api.post('projects', formData, {
            headers: {
                'Content-Type': 'multipart/form-data',
            },
        });
        return response.data;
    },

    deleteProject: async (id: string): Promise<ApiResponse> => {
        const response = await api.delete(`projects/${id}`);
        return response.data;
    },

    getImageList: async (id: string): Promise<ApiResponse> => {
        const response = await api.get(`projects/${id}/images`);
        return response.data;
    },

    updateProject: async (id: string, formData: FormData): Promise<ApiResponse<Project>> => {
        const response = await api.put(`projects/${id}`, formData, {
            headers: {
                'Content-Type': 'multipart/form-data',
            },
        });
        return response.data;
    },

    getComments: async (id: string): Promise<ApiResponse<Comment[]>> => {
        const response = await api.get(`projects/${id}/comments`);
        return response.data;
    },

    addComment: async (id: string, data: CommentFormData): Promise<ApiResponse<Comment>> => {
        const response = await api.post(`projects/${id}/comments`, data);
        return response.data;
    },

    addScore: async (id: string, data: ScoreFormData): Promise<ApiResponse<number>> => {
        const response = await api.post(`projects/${id}/score`, data);
        return response.data;
    },

    getProjectsByUser: async (userId: string): Promise<ApiResponse<Project[]>> => {
        const response = await api.get(`projects/author/${userId}`);
        return response.data;
    },

    getProjectImagePath: (id: string, path: string): string =>
        `${API_BASE_URL}/projects/${id}/images/${path}`
};
