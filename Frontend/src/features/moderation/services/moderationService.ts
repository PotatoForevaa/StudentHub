import api from "../../../shared/services/base";
import type { ApiResponse, PaginatedResponse } from "../../../shared/types";
import type { Comment } from "../../projects/types";

export type ModerationQueue = "reported" | "ai-toxic" | "moderator-toxic" | "appeals";

export type MuteInfo = {
  isMuted: boolean;
  mutedUntil?: string;
  reason?: string;
  mutedByUsername?: string;
  createdAt?: string;
};

export const moderationService = {
  getComments: async (params: { queue: ModerationQueue; page: number; pageSize: number }): Promise<ApiResponse<PaginatedResponse<Comment>>> => {
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

  submitAppeal: async (id: string, message?: string): Promise<ApiResponse> => {
    const response = await api.post(`moderation/comments/${id}/appeal`, { message });
    return response.data;
  },

  resolveAppeal: async (id: string, approved: boolean): Promise<ApiResponse<Comment>> => {
    const response = await api.put(`moderation/comments/${id}/appeal`, { approved });
    return response.data;
  },

  muteUser: async (userId: string, duration: string, reason?: string): Promise<ApiResponse> => {
    const response = await api.post(`moderation/users/${userId}/mute`, { duration, reason });
    return response.data;
  },

  unmuteUser: async (userId: string): Promise<ApiResponse> => {
    const response = await api.delete(`moderation/users/${userId}/mute`);
    return response.data;
  },

  getMuteStatus: async (userId: string): Promise<ApiResponse<MuteInfo>> => {
    const response = await api.get(`moderation/users/${userId}/mute`);
    return response.data;
  },

  getActiveMutes: async (params: { page: number; pageSize: number }): Promise<ApiResponse<PaginatedResponse<MuteInfo>>> => {
    const response = await api.get("moderation/mutes", { params });
    return response.data;
  },
};
