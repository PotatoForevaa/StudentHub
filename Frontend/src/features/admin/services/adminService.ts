import api from "../../../shared/services/base";
import type { ApiResponse, PaginatedResponse, User } from "../../../shared/types";
import type { Comment, CategoryDto, TagDto, CriterionDto } from "../../projects/types";

export type AdminProject = {
  id: string;
  name: string;
  description: string;
  authorUsername: string;
  authorName: string;
  createdAt: string;
};

export const adminService = {
  getUsers: async (params: { search?: string; role?: string; page: number; pageSize: number }): Promise<ApiResponse<PaginatedResponse<User>>> => {
    const response = await api.get("admin/users", { params });
    return response.data;
  },

  updateUserRole: async (id: string, role: string): Promise<ApiResponse> => {
    const response = await api.put(`admin/users/${id}/role`, { role });
    return response.data;
  },

  deleteUser: async (id: string): Promise<ApiResponse> => {
    const response = await api.delete(`admin/users/${id}`);
    return response.data;
  },

  getProjects: async (params: { search?: string; page: number; pageSize: number }): Promise<ApiResponse<PaginatedResponse<AdminProject>>> => {
    const response = await api.get("admin/projects", { params });
    return response.data;
  },

  deleteProject: async (id: string): Promise<ApiResponse> => {
    const response = await api.delete(`admin/projects/${id}`);
    return response.data;
  },

  getModerationComments: async (params: { queue: "reported" | "ai-toxic"; page: number; pageSize: number }): Promise<ApiResponse<PaginatedResponse<Comment>>> => {
    const response = await api.get("moderation/comments", { params });
    return response.data;
  },

  approveComment: async (id: string): Promise<ApiResponse<Comment>> => {
    const response = await api.put(`moderation/comments/${id}/approve`);
    return response.data;
  },

  markCommentToxic: async (id: string): Promise<ApiResponse<Comment>> => {
    const response = await api.put(`moderation/comments/${id}/toxic`);
    return response.data;
  },

  // --- Tags management ---

  getAdminTags: async (): Promise<ApiResponse<TagDto[]>> => {
    const response = await api.get("admin/tags");
    return response.data;
  },

  createTag: async (name: string): Promise<ApiResponse<TagDto>> => {
    const response = await api.post("admin/tags", { name });
    return response.data;
  },

  deleteTag: async (id: string): Promise<ApiResponse> => {
    const response = await api.delete(`admin/tags/${id}`);
    return response.data;
  },

  // --- Criteria management ---

  getAdminCriteria: async (categoryId?: string): Promise<ApiResponse<CriterionDto[]>> => {
    const response = await api.get("admin/criteria", { params: { categoryId } });
    return response.data;
  },

  createCriterion: async (name: string, categoryId: string): Promise<ApiResponse<CriterionDto>> => {
    const response = await api.post("admin/criteria", { name, categoryId });
    return response.data;
  },

  deleteCriterion: async (id: string): Promise<ApiResponse> => {
    const response = await api.delete(`admin/criteria/${id}`);
    return response.data;
  },

  // --- Categories ---

  getAdminCategories: async (): Promise<ApiResponse<CategoryDto[]>> => {
    const response = await api.get("admin/categories");
    return response.data;
  },

  createCategory: async (name: string): Promise<ApiResponse<CategoryDto>> => {
    const response = await api.post("admin/categories", { name });
    return response.data;
  },

  deleteCategory: async (id: string): Promise<ApiResponse> => {
    const response = await api.delete(`admin/categories/${id}`);
    return response.data;
  },
};
