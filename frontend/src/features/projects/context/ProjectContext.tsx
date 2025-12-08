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
};

export const ProjectContext = createContext<ProjectContextType>({
    project: null,
    projects: [],
    loading: true,
    getProject: async (_id: string) => {},
    getProjects: async () => {}
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
        project,
        projects,
        loading
      }}
    >
      {children}
    </ProjectContext.Provider>
  );
};

