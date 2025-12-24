import { useEffect, useState } from 'react';
import { projectService } from '../../projects/services/projectService';
import { usePagination } from '../../../shared/hooks/usePagination';
import type { Project } from '../../projects/types';

export interface UseUserProjectsReturn {
  projects: Project[];
  paginatedProjects: Project[];
  loading: boolean;
  error: string | null;
  currentPage: number;
  totalPages: number;
  pageSize: number;
  setCurrentPage: (page: number) => void;
  setPageSize: (size: number) => void;
}

export function useUserProjects(userId: string, username: string): UseUserProjectsReturn {
  const [projects, setProjects] = useState<Project[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const { paginatedItems: paginatedProjects, currentPage, pageSize, totalPages, setCurrentPage, setPageSize } = usePagination(projects, 6);

  useEffect(() => {
    if (userId && username) {
      setLoading(true);
      setError(null);
      const fetchUserProjects = async () => {
        try {
          const res = await projectService.getProjectsByUser(userId);
          if (res?.isSuccess && res.data) {
            const mapped = res.data.map((p: unknown) => ({
              ...(p as Record<string, unknown>),
              imagePaths: (p as { files?: string[] }).files || []
            }));
            setProjects(mapped as Project[]);
          } else {
            console.warn('Failed to fetch user projects via API, falling back to filtering', res?.errors);
            const allRes = await projectService.getProjects();
            if (allRes?.isSuccess && allRes.data) {
              const filtered = allRes.data.filter(p => p.author === username);
              const mapped = filtered.map((p: unknown) => ({
                ...(p as Record<string, unknown>),
                imagePaths: (p as { files?: string[] }).files || []
              }));
              setProjects(mapped as Project[]);
            } else {
              setError('Не удалось загрузить проекты');
              setProjects([]);
            }
          }
        } catch (error) {
          console.error('Failed to fetch projects', error);
          setError('Ошибка загрузки проектов');
          setProjects([]);
        } finally {
          setLoading(false);
        }
      };

      fetchUserProjects();
    }
  }, [userId, username]);

  return {
    projects,
    paginatedProjects,
    loading,
    error,
    currentPage,
    totalPages,
    pageSize,
    setCurrentPage,
    setPageSize,
  };
}
