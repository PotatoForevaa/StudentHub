import { createContext, useEffect, useState, useContext, useCallback, type ReactNode } from "react";
import { projectService } from "../services/projectService";
import type { Project } from "../types";
import { AuthContext } from "../../auth/context/AuthContext";

export type ProjectContextType = {
    project: Project | null;
    projects: Project[] | null;
    loading: boolean;
    // Pagination
    currentPage: number;
    pageSize: number;
    totalPages: number;
    paginatedProjects: Project[] | null;
    setCurrentPage: (page: number) => void;
    setPageSize: (size: number) => void;
    getProject: (id: string) => Promise<void>;
    getProjects: () => Promise<void>;
    addProject: (formData: FormData) => Promise<boolean>;
};

export const ProjectContext = createContext<ProjectContextType>({
    project: null,
    projects: [],
    loading: true,
    currentPage: 1,
    pageSize: 6,
    totalPages: 0,
    paginatedProjects: [],
    setCurrentPage: () => {},
    setPageSize: () => {},
    getProject: async (_id: string) => {},
    getProjects: async () => {},
    addProject: async (_formData: FormData) => false
});

export const ProjectProvider = ({ children }: { children: ReactNode }) => {
  const [project, setProject] = useState<Project | null>(null);
  const [projects, setProjects] = useState<Project[]>([]);
  const [loading, setLoading] = useState(true);
  const [currentPage, setCurrentPageState] = useState(1);
  const [pageSize, setPageSizeState] = useState(6);
  const { isAuthenticated, loading: authLoading } = useContext(AuthContext);

  const totalPages = Math.ceil((projects?.length || 0) / pageSize);
  const paginatedProjects = projects?.slice((currentPage - 1) * pageSize, currentPage * pageSize) || [];

  const setCurrentPage = useCallback((page: number) => {
    setCurrentPageState(page);
  }, []);

  const setPageSize = useCallback((size: number) => {
    setPageSizeState(size);
    setCurrentPageState(1);
  }, []);

  const getProjects = useCallback(async () => {
    setLoading(true);
    try {
        const res = await projectService.getProjects();
        if (res && res.isSuccess && res.data) {
          const mappedProjects = res.data.map((p: unknown) => ({
            ...(p as Record<string, unknown>),
            imagePaths: (p as { files?: string[] }).files || []
          }));
          setProjects(mappedProjects as Project[]);
        } else {
          console.warn('Failed to load projects, ApiResponse:', res);
          setProjects([]);
        }
    } catch (err) {
        console.error('Error fetching projects:', err);
        setProjects([]);
    } finally {
        setLoading(false);
    }
  }, []);

  const getProject = async (id: string) => {
    try {
      const res = await projectService.getProject(id);
      if (res.isSuccess && res.data) {
        const projectData = { ...(res.data as Record<string, unknown>), imagePaths: (res.data as { files?: string[] }).files || [] };
        setProject(projectData as Project);
      } else {
        setProject(null);
      }
    } catch {
      setProject(null);
    }
  };

  const addProject = async (formData: FormData): Promise<boolean> => {
    try {
      const res = await projectService.addProject(formData);
      if (res.isSuccess) {
        await getProjects();
        return true;
      }
      return false;
    } catch (error) {
      console.error('Failed to add project:', error);
      return false;
    }
  };

  useEffect(() => {
    if (!authLoading && isAuthenticated) {
      getProjects();
    }
  }, [isAuthenticated, authLoading, getProjects]);

  return (
    <ProjectContext.Provider
      value={{
        getProject,
        getProjects,
        addProject,
        project,
        projects,
        loading,
        // Pagination
        currentPage,
        pageSize,
        totalPages,
        paginatedProjects,
        setCurrentPage,
        setPageSize
      }}
    >
      {children}
    </ProjectContext.Provider>
  );
};
