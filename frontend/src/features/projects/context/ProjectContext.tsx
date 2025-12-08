import { createContext, useEffect, useState, useContext, useCallback, type ReactNode } from "react";
import { projectService } from "../services/projectService";
import type { Project } from "../types";
import { AuthContext } from "../../auth/context/AuthContext";

export type ProjectContextType = {
    project: Project | null;
    projects: Project[] | null;
    loading: boolean;
    getProject: (id: string) => Promise<void>;
    getProjects: () => Promise<void>;
    addProject: (formData: FormData) => Promise<boolean>;
    getProjectImages: (projectId: string) => Promise<string[]>;
};

export const ProjectContext = createContext<ProjectContextType>({
    project: null,
    projects: [],
    loading: true,
    getProject: async (_id: string) => {},
    getProjects: async () => {},
    addProject: async (_formData: FormData) => false,
    getProjectImages: async (_projectId: string) => []
});

export const ProjectProvider = ({ children }: { children: ReactNode }) => {
  const [project, setProject] = useState<Project | null>(null);
  const [projects, setProjects] = useState<Project[]>([]);
  const [loading, setLoading] = useState(true);
  const { isAuthenticated, loading: authLoading } = useContext(AuthContext);

  const getProjects = useCallback(async () => {
    setLoading(true);
    try {
        const res = await projectService.getProjects();
        console.debug('projectService.getProjects response:', res);
        if (res && res.isSuccess && res.data) {
          setProjects(res.data as Project[]);
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
        setProject(res.data as Project);
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
        await getProjects(); // Refresh projects list
        return true;
      }
      return false;
    } catch (error) {
      console.error('Failed to add project:', error);
      return false;
    }
  };

  const getProjectImages = async (projectId: string): Promise<string[]> => {
    try {
      const res = await projectService.getImageList(projectId);
      if (res.isSuccess && res.data && (res.data as any).imagePaths) {
        return (res.data as any).imagePaths;
      }
      return [];
    } catch (error) {
      console.error('Failed to get project images:', error);
      return [];
    }
  };

  // Fetch projects when authenticated (and not loading)
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
        getProjectImages,
        project,
        projects,
        loading
      }}
    >
      {children}
    </ProjectContext.Provider>
  );
};
