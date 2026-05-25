import { useState, useCallback, useEffect } from "react";
import { projectService } from "../services/projectService";
import type { Project, CategoryDto, TagDto } from "../types";
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
  searchTerm: string;
  setSearchTerm: (term: string) => void;
  selectedCategoryId: string | undefined;
  setSelectedCategoryId: (id: string | undefined) => void;
  selectedTagId: string | undefined;
  setSelectedTagId: (id: string | undefined) => void;
  categories: CategoryDto[];
  tags: TagDto[];
  loadingFilters: boolean;
}

export function useProjects(initialPageSize: number = 12): UseProjectsReturn {
  const [projects, setProjects] = useState<Project[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedCategoryId, setSelectedCategoryId] = useState<string | undefined>(undefined);
  const [selectedTagId, setSelectedTagId] = useState<string | undefined>(undefined);
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [tags, setTags] = useState<TagDto[]>([]);
  const [loadingFilters, setLoadingFilters] = useState(false);
  const [filtersLoaded, setFiltersLoaded] = useState(false);

  const { paginatedItems: paginatedProjects, currentPage, pageSize, totalPages, setCurrentPage, setPageSize } = usePagination(projects, initialPageSize);

  const loadFilters = useCallback(async () => {
    setLoadingFilters(true);
    setFiltersLoaded(true);
    try {
      const [catResult, tagResult] = await Promise.all([
        projectService.getCategories(),
        projectService.getTags(),
      ]);
      if (catResult.isSuccess && catResult.data) {
        setCategories(catResult.data);
      }
      if (tagResult.isSuccess && tagResult.data) {
        setTags(tagResult.data);
      }
    } catch (err) {
      console.error("Failed to load filters", err);
    } finally {
      setLoadingFilters(false);
    }
  }, []);

  // Auto-load categories and tags on mount
  useEffect(() => {
    if (!filtersLoaded) {
      loadFilters();
    }
  }, [filtersLoaded, loadFilters]);

  const fetchProjects = useCallback(async () => {
    setLoading(true);
    setError(null);

    try {
      const hasFilters = searchTerm || selectedCategoryId || selectedTagId;

      if (hasFilters) {
        const response = await projectService.searchProjects({
          search: searchTerm || undefined,
          categoryId: selectedCategoryId || undefined,
          tagId: selectedTagId || undefined,
        });
        if (response.isSuccess && response.data) {
          const items = response.data.items || [];
          const mappedProjects: Project[] = items.map((project: any) => ({
            ...project,
            imagePaths: project.files || []
          }));
          setProjects(mappedProjects);
        } else {
          setError('Проекты не найдены');
          setProjects([]);
        }
      } else {
        const response = await projectService.getProjects();
        if (response.isSuccess && response.data) {
          const mappedProjects: Project[] = response.data.map((project: any) => ({
            ...project,
            imagePaths: project.files || []
          }));
          setProjects(mappedProjects);
        } else {
          setError('Не удалось загрузить проекты');
          setProjects([]);
        }
      }
    } catch (err) {
      setError('Ошибка при загрузке проектов');
      setProjects([]);
    } finally {
      setLoading(false);
    }
  }, [searchTerm, selectedCategoryId, selectedTagId]);

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
    setPageSize,
    searchTerm,
    setSearchTerm,
    selectedCategoryId,
    setSelectedCategoryId,
    selectedTagId,
    setSelectedTagId,
    categories,
    tags,
    loadingFilters,
  };
}