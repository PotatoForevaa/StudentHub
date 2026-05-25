import api, { API_BASE_URL } from '../../../shared/services/base';
import type { ApiResponse, PaginatedResponse } from '../../../shared/types';
import type { Project, Comment, ScoreFormData, CommentFormData, UpdateProjectFormData, CategoryDto, TagDto, CriterionDto, SubmitCriterionScoresData } from '../types';

export const projectService = {
    getProjects: async (): Promise<ApiResponse<Project[]>> => {
        const response = await api.get('projects');
        return response.data;
    },

    getProject: async (id: string): Promise<ApiResponse<Project>> => {
        const response = await api.get(`projects/${id}`);
        return response.data;
    },

    searchProjects: async (params: { search?: string; categoryId?: string; tagId?: string; page?: number; pageSize?: number }): Promise<ApiResponse<PaginatedResponse<Project>>> => {
        const response = await api.get('projects/search', { params });
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

    getComments: async (id: string, page = 1, pageSize = 10): Promise<ApiResponse<PaginatedResponse<Comment>>> => {
        const response = await api.get(`projects/${id}/comments`, { params: { page, pageSize } });
        return response.data;
    },

    addComment: async (id: string, data: CommentFormData): Promise<ApiResponse<Comment>> => {
        const response = await api.post(`projects/${id}/comments`, data);
        return response.data;
    },

    reportComment: async (projectId: string, commentId: string): Promise<ApiResponse> => {
        const response = await api.post(`projects/${projectId}/comments/${commentId}/report`);
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
        `${API_BASE_URL}/projects/${id}/images/${path}`,

    // --- New methods ---

    getCategories: async (): Promise<ApiResponse<CategoryDto[]>> => {
        const response = await api.get('projects/categories');
        return response.data;
    },

    getTags: async (): Promise<ApiResponse<TagDto[]>> => {
        const response = await api.get('projects/tags');
        return response.data;
    },

    getCriteriaByCategory: async (categoryId: string): Promise<ApiResponse<CriterionDto[]>> => {
        const response = await api.get(`projects/categories/${categoryId}/criteria`);
        return response.data;
    },

    submitScores: async (projectId: string, data: SubmitCriterionScoresData): Promise<ApiResponse> => {
        const response = await api.post(`projects/${projectId}/scores`, data);
        return response.data;
    },

    getScores: async (projectId: string): Promise<ApiResponse<import('../types').CriterionScoreDto[]>> => {
        const response = await api.get(`projects/${projectId}/scores`);
        return response.data;
    },
};