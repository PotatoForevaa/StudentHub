import { useState, useCallback } from "react";
import { projectService } from "../services/projectService";
import type { Project } from "../types";
import { usePagination } from "../../../shared/hooks/usePagination";

interface UseProjectsReturn {
  projects: Project[] | null;
  loading: boolean;
  error: string | null;
  refetch: () => Promise<void>;
  paginatedProjects: Project[] | null;
  currentPage: number;
  totalPages: number;
  pageSize: number;
  setCurrentPage: (page: number) => void;
  setPageSize: (size: number) => void;
}

export function useProjects(initialPageSize: number = 6): UseProjectsReturn {
  const [projects, setProjects] = useState<Project[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const { paginatedItems: paginatedProjects, currentPage, pageSize, totalPages, setCurrentPage, setPageSize } = usePagination(projects, initialPageSize);

  const fetchProjects = useCallback(async () => {
    setLoading(true);
    setError(null);

    try {
      const response = await projectService.getProjects();
      if (response.isSuccess && response.data) {
        const mappedProjects: Project[] = response.data.map((project: any) => ({
          ...project,
          imagePaths: project.files || []
        }));
        setProjects(mappedProjects);
      } else {
        setError('Failed to load projects');
        setProjects([]);
      }
    } catch (err) {
      setError('Error fetching projects');
      setProjects([]);
    } finally {
      setLoading(false);
    }
  }, []);

  return {
    projects,
    loading,
    error,
    refetch: fetchProjects,
    paginatedProjects,
    currentPage,
    totalPages,
    pageSize,
    setCurrentPage,
    setPageSize
  };
}
