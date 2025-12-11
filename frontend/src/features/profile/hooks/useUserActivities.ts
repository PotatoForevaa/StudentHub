import { useEffect, useState } from 'react';
import userService from '../../../shared/services/userService';
import type { ActivityDto } from '../../../shared/types';

export interface UseUserActivitiesReturn {
  activities: ActivityDto[];
  loading: boolean;
  error: string | null;
}

/**
 * Hook to fetch recent user activities
 * @param username - Username to fetch activities for
 * @param limit - Maximum number of activities to fetch (default: 10)
 */
export function useUserActivities(username: string, limit: number = 10): UseUserActivitiesReturn {
  const [activities, setActivities] = useState<ActivityDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (username) {
      setLoading(true);
      setError(null);
      userService.getUserActivityByUsername(username)
        .then(res => {
          if (res?.isSuccess && res.data) {
            // Sort by createdAt descending (newest first) and limit results
            const sorted = res.data
              .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
              .slice(0, limit);
            setActivities(sorted);
          } else {
            setError(res?.errors?.[0]?.message || 'Не удалось загрузить активности');
            setActivities([]);
          }
        })
        .catch(error => {
          console.error('Failed to fetch user activities:', error);
          setError('Ошибка загрузки активности');
          setActivities([]);
        })
        .finally(() => {
          setLoading(false);
        });
    }
  }, [username, limit]);

  return {
    activities,
    loading,
    error
  };
}
