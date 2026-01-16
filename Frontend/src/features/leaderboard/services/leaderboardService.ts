import api, { API_BASE_URL } from '../../../shared/services/base';
import type { ApiResponse } from '../../../shared/types';
import type { LeaderboardUser, LeaderboardType, LeaderboardPeriod } from '../types';

export const leaderboardService = {
    getLeaderboard: async (
        type: LeaderboardType,
        period: LeaderboardPeriod,
        page: number = 0,
        size: number = 10
    ): Promise<ApiResponse<LeaderboardUser[]>> => {
        const response = await api.get(`${API_BASE_URL}/leaderboards/${type}/${period}`, {
            params: { page, size }
        });
        return response.data;
    }
};
